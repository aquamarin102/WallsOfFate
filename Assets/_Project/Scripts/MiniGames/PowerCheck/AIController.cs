using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Zenject;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MiniGamePlayer))]
public class AIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MineSpawner _mineSpawner;
    [SerializeField] private MiniGamePlayer _thisCharacteristics;
    [SerializeField] private MiniGamePlayer _playerCharacteristics;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _playerTransform;

    [Header("Global Modifiers")]
    [SerializeField] private float _damageGlobalModifier = 1f;
    [SerializeField] private float _healGlobalModifier = 1f;
    [SerializeField] private float _buffGlobalModifier = 1f;
    [SerializeField] private float _slowness = 1f;

    [Header("Risk Calculation")]
    [SerializeField] private float _riskDistanceThreshold = 5.0f;
    [SerializeField] private float _playerRiskFactor = 15f;
    [SerializeField] private float _mineRiskFactor = 5f;

    private Queue<Vector3> _targetsQueue = new Queue<Vector3>();
    private float _baseAgentSpeed;
    private bool _underDebuff;
    private float _lockedY;
    private Vector3 _currentTarget;
    private Rigidbody _rb;

    private int _currentNumOfDamage = 0;
    private int _currentNumOfHeal = 0;
    private int _currentNumOfBuff = 0;

    [Inject]
    private void Construct([Inject(Id = "Player")] PlayerMove player)
    {
        _playerTransform = player.gameObject.transform;
        _playerCharacteristics = player.gameObject.GetComponent<MiniGamePlayer>();
    }

    private void Start()
    {
        _mineSpawner = GameObject.FindGameObjectWithTag("MineSpawner").GetComponent<MineSpawner>();
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            // Включаем агента
            _agent.enabled = true;
        }
        if (_thisCharacteristics == null) _thisCharacteristics = GetComponent<MiniGamePlayer>();

        _baseAgentSpeed = _thisCharacteristics.Speed;
        _lockedY = transform.position.y;
        if (!HasTargetQueue())
            StartSelectingNextTarget(_slowness);
        //_rb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!HasTargetQueue() || CheckIsPickUpsUpdate())
            StartSelectingNextTarget(_slowness);

        if (!_agent.pathPending && HasTargetQueue() && (!MineExist(_currentTarget) || _agent.remainingDistance <= _agent.stoppingDistance))
        {
            SetNextDestination();
        }


        transform.position = new Vector3(transform.position.x, _lockedY, transform.position.z);
        if (_agent.speed == 0)
        {
            transform.position = transform.position;
        }
    }

    private IEnumerator SelectNextTargetCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay); // Ожидание перед выполнением

        List<Mine> allMines = new List<Mine>();

        if (_mineSpawner.HealMines.Count > 0) allMines.Add(FindBestMine(_mineSpawner.HealMines));
        if (_mineSpawner.DamageMines.Count > 0) allMines.Add(FindBestMine(_mineSpawner.DamageMines));
        if (_mineSpawner.BuffMines.Count > 0) allMines.Add(FindBestMine(_mineSpawner.BuffMines));

        allMines = allMines.Where(m => m != null).OrderBy(m => EvaluateMine(m)).ToList();

        _targetsQueue.Clear();
        foreach (var mine in allMines)
        {
            _targetsQueue.Enqueue(mine.MineGameObject.transform.position);
        }
    }

    // Запуск корутины
    public void StartSelectingNextTarget(float delay)
    {
        StartCoroutine(SelectNextTargetCoroutine(delay));
    }

    private Mine FindBestMine(IReadOnlyList<Mine> mines)
    {
        return mines
            .Where(m => m.MineGameObject.activeSelf)
            .OrderBy(m => EvaluateMine(m))
            .FirstOrDefault();
    }

    private float EvaluateMine(Mine mine)
    {
        float evaluation = 0;
        float playerHP = _playerCharacteristics.Health / _playerCharacteristics.MaxHealth;
        float thisHP = _thisCharacteristics.Health / _thisCharacteristics.MaxHealth;

        if (mine is DamageMine) evaluation = EvaluationDamagePickUp(playerHP, thisHP);
        else if (mine is HealMine) evaluation = EvaluationHealPickUp(thisHP);
        else if (mine is BuffSpeedMine buffMine) evaluation = EvaluationBuffPickUp(buffMine);

        float distance = Vector3.Distance(transform.position, mine.MineGameObject.transform.position);
        float riskPenalty = CalculateRiskPenalty(mine);

        return (evaluation - riskPenalty) / (distance / 10);
    }

    private float EvaluationBuffPickUp(BuffSpeedMine buffMine)
    {
        float modifier = 1;
        float evaluatin;
        int numOfMines = _mineSpawner.DebuffMines.Count;
        float mineSquare = 0;
        if (_mineSpawner.DebuffMines.Count > 0)
        {
            mineSquare = _mineSpawner.DebuffMines[0].MineGameObject.transform.localScale.x * _mineSpawner.DebuffMines[0].MineGameObject.transform.localScale.z;
        }
        float ariaSize = _mineSpawner.spawnAreaSize.x * _mineSpawner.spawnAreaSize.y;
        float squareOfAllMines = ariaSize / (numOfMines * mineSquare);


        if (squareOfAllMines < 0.3f) modifier = 1.5f;
        else if (0.3f < squareOfAllMines && squareOfAllMines < 0.7f) modifier = 1f;
        if (squareOfAllMines > 0.7f) modifier = 0.1f;
        evaluatin = 5 * buffMine.GetSpeedBuff();
        return evaluatin * modifier * _buffGlobalModifier;
    }

    private float EvaluationHealPickUp(float thisHP)
    {
        float modifier = 1;
        float evaluatin;
        if (thisHP < 0.3f) modifier = 2f;
        else if (0.3f < thisHP && thisHP < 0.7f) modifier = 1f;
        if (thisHP > 0.7f) modifier = 0.5f;
        evaluatin = 10 * _thisCharacteristics.HealingAmount;
        return evaluatin * modifier * _damageGlobalModifier;
    }

    private float EvaluationDamagePickUp(float playerHP, float thisHP)
    {
        float modifier = 1;
        float evaluatin;
        if (playerHP < 0.3f) modifier = 1.5f;
        else if (playerHP > 0.7f) modifier = 0.8f;
        if (thisHP < 0.2f) modifier = 0.5f;
        evaluatin = 10 * _thisCharacteristics.Damage;
        return evaluatin * modifier * _healGlobalModifier;
    }

    private float CalculateRiskPenalty(Mine mine)
    {
        float penalty = _playerRiskFactor / Vector3.Distance(mine.MineGameObject.transform.position, _playerTransform.position);

        foreach (var debuffMine in _mineSpawner.DebuffMines)
        {
            float dist = Vector3.Distance(mine.MineGameObject.transform.position, debuffMine.MineGameObject.transform.position);
            if (dist < _riskDistanceThreshold) penalty += _mineRiskFactor / dist;
        }
        return penalty;
    }

    private void SetNextDestination()
    {
        if (_targetsQueue.Count > 0)
        {
            Vector3 nextTarget = _targetsQueue.Dequeue();
            _currentTarget = nextTarget;
            if (NavMesh.SamplePosition(nextTarget, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                //Debug.Log(nextTarget);
            }
            else
            {
                Debug.LogWarning("AIController: Не удалось найти навигационную точку.");
            }
        }
    }

    private bool HasTargetQueue()
    {
        return _targetsQueue.Count > 0 && _targetsQueue.All(target => target != Vector3.zero);
    }

    private bool MineExist(Vector3 position)
    {
        return _mineSpawner.HealMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, position) <= 0.1) ||
               _mineSpawner.DamageMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, position) <= 0.1) ||
               _mineSpawner.BuffMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, position) <= 0.1);
    }

    private bool CheckIsPickUpsUpdate()
    {
        int activeDamageCount = _mineSpawner.DamageMines.Count(mine => mine.MineGameObject.activeSelf);
        int activeHealCount = _mineSpawner.HealMines.Count(mine => mine.MineGameObject.activeSelf);
        int activeBuffCount = _mineSpawner.BuffMines.Count(mine => mine.MineGameObject.activeSelf);

        int deltaDamage = activeDamageCount - _currentNumOfDamage;
        int deltaHeal = activeHealCount - _currentNumOfHeal;
        int deltaBuff = activeBuffCount - _currentNumOfBuff;

        _currentNumOfDamage = activeDamageCount;
        _currentNumOfHeal = activeHealCount;
        _currentNumOfBuff = activeBuffCount;

        return deltaDamage > 0 || deltaBuff > 0 || deltaHeal > 0;
    }


    public void ChangeSpeed(float speed, bool isDebuff)
    {
        if (_underDebuff && isDebuff)
        {
            _underDebuff = false;
            _agent.speed = _baseAgentSpeed * speed;
        }
        else if (!_underDebuff && isDebuff)
        {
            _underDebuff = true;
            _agent.speed = _baseAgentSpeed * speed;
        }
        else if (!_underDebuff && !isDebuff)
        {
            _agent.speed = _baseAgentSpeed * speed;
        }
    }
}
