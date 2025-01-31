using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MiniGamePlayer))]
public class EnemyMove : MonoBehaviour
{
    [Header("Movement Params")]
    [SerializeField] private MineSpawner _mineSpawner;
    [SerializeField] private MiniGamePlayer _characteristics;

    private Queue<Vector3> _targetsQueue = new Queue<Vector3>(); // Очередь целей

    private Rigidbody _rb;
    private MiniGamePlayer _playerChar;
    private NavMeshAgent _Agent;
    private float _baseAgentSpeed;
    private bool _underDebuff;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _Agent = GetComponent<NavMeshAgent>();

        _characteristics = this.GetComponent<MiniGamePlayer>();
        _baseAgentSpeed = _characteristics.Speed;
    }

    private void Start()
    {
        if (!HasTargetQueue())
        {
            UpdateTargetPoints();
        }
    }

    private void FixedUpdate()
    {

        UpdateTargetPoints();
        if (!_Agent.pathPending && HasTargetQueue())
        {
            SetNextDestination();
        }

        Vector3 pos = _Agent.transform.position;
        pos.y = 0; // Принудительно фиксируем Y
        _Agent.transform.position = pos;
    }

    private void UpdateTargetPoints()
    {
        List<Mine> allMines = new List<Mine>();

        // Добавляем ближайшую мину из каждой категории
        if (_mineSpawner.HealMines.Count > 0) allMines.Add(FindClosestMine(_mineSpawner.HealMines));
        if (_mineSpawner.DamageMines.Count > 0) allMines.Add(FindClosestMine(_mineSpawner.DamageMines));
        if (_mineSpawner.BuffMines.Count > 0) allMines.Add(FindClosestMine(_mineSpawner.BuffMines));


        // Сортируем точки по расстоянию от агента
        var sortedMines = allMines
            .Where(m => m != null)
            .OrderBy(m => Vector3.Distance(transform.position, m.MineGameObject.transform.position));
        _targetsQueue.Clear(); // Очищаем старые цели
        foreach (var mine in sortedMines)
        {
            _targetsQueue.Enqueue(mine.MineGameObject.transform.position);
        }

        //SetNextDestination();
    }

    private Mine FindClosestMine(IReadOnlyList<Mine> mines)
    {
        return mines
            .Where(m => m.MineGameObject.activeSelf)
            .OrderBy(m => Vector3.Distance(transform.position, m.MineGameObject.transform.position))
            .FirstOrDefault();
    }

    private void SetNextDestination()
    {
        if (_targetsQueue.Count > 0)
        {
            Vector3 nextTarget = _targetsQueue.Dequeue();
            _Agent.SetDestination(nextTarget);
        }
    }

    private bool HasMines()
    {
        return _mineSpawner.HealMines.Any(m => m.MineGameObject.activeSelf) ||
               _mineSpawner.DamageMines.Any(m => m.MineGameObject.activeSelf) ||
               _mineSpawner.BuffMines.Any(m => m.MineGameObject.activeSelf);
    }

    private bool HasTargetQueue()
    {
        return _targetsQueue.Count > 0 && _targetsQueue.Any(target => target != Vector3.zero);
    }

    public void ChangeSpeed(float speed, bool isDebuff)
    {
        if(_underDebuff && isDebuff)
        {
            _underDebuff = false;
            _Agent.speed = _baseAgentSpeed * speed;
        }
        else if(!_underDebuff && isDebuff)
        {
            _underDebuff = true;
            _Agent.speed = _baseAgentSpeed * speed;
        }
        else if(!_underDebuff && !isDebuff)
        {
            _Agent.speed = _baseAgentSpeed * speed;
        }
    }

}
