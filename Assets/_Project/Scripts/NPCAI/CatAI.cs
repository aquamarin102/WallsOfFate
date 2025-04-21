using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class CatAI : MonoBehaviour
{
    #region Serialized
    [Header("Player reaction")]
    [SerializeField] private float curiosityRange = 6f;   // «интересно»
    [SerializeField] private float comfortMin = 2f;   // «очень близко»
    [SerializeField] private float comfortMax = 3.5f; // «рядом»
    [SerializeField] private float approachReenterDistance = 8f;   // когда снова можно заинтересоваться
    [SerializeField] private float panicRange = 1.2f; // убегаем

    [Header("Move speeds (m/s)")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 3.2f;

    [Header("Cat routine")]
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float idleMinTime = 2f;
    [SerializeField] private float idleMaxTime = 5f;
    [SerializeField] private float curiosityTime = 5f;
    [SerializeField] private float curiosityCooldown = 8f;
    [SerializeField] private float retreatDistance = 5f;  // насколько далеко отойти
    [SerializeField] private float sleepChance = 0.1f;
    [SerializeField] private float sleepTime = 10f;

    [Header("Anti‑stuck")]
    [SerializeField] private float stuckSpeed = 0.05f;
    [SerializeField] private float stuckTime = 2f;
    #endregion

    private enum State { Idle, Wander, Approach, Retreat, Flee, Sleep }
    private State _state;

    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _player;

    private float _stateTimer;
    private float _curiosityTimer;
    private float _cooldownTimer;
    private float _stuckTimer;

    #region DI
    [Inject]
    private void Construct(PlayerMoveController player) =>
        _player = player.transform;
    #endregion

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.acceleration = runSpeed * 2f;
        _anim = GetComponent<Animator>();
    }

    private void OnEnable() => SwitchState(State.Idle);

    private void Update()
    {
        float dt = Time.deltaTime;
        float dist = Vector3.Distance(transform.position, _player.position);

        if (_cooldownTimer > 0f) _cooldownTimer -= dt;

        /* ────── глобальные переходы ────── */
        if (_state != State.Flee && dist < panicRange)
        {
            SwitchState(State.Flee);
        }
        else if (_state is State.Idle or State.Wander or State.Sleep)
        {
            bool inInterestZone = dist < curiosityRange && dist > comfortMax;
            bool cooledDown = _cooldownTimer <= 0f && dist > approachReenterDistance;
            if (inInterestZone && cooledDown) SwitchState(State.Approach);
        }

        /* ────── локальная логика ────── */
        switch (_state)
        {
            case State.Idle:
                if (_stateTimer <= 0f)
                    SwitchState(Random.value < sleepChance ? State.Sleep : State.Wander);
                break;

            case State.Wander:
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                    SwitchState(State.Idle);
                break;

            case State.Approach:
                ApproachLogic(dist);
                break;

            case State.Retreat:
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                    SwitchState(State.Wander);              // дальше обычные бродилки
                break;

            case State.Flee:
                if (dist > comfortMax * 2f)
                    EndPlayerInteraction();
                break;

            case State.Sleep:
                if (_stateTimer <= 0f)
                    SwitchState(State.Idle);
                break;
        }

        _stateTimer -= dt;

        /* ────── анти‑застревание ────── */
        DetectStuck(dt);

        /* ────── анимация ────── */
        float speedNorm = _agent.velocity.magnitude / runSpeed;
        _anim.SetFloat("Speed", speedNorm, 0.1f, dt);
    }

    /* ────────────── FSM ────────────── */

    private void SwitchState(State next)
    {
        _state = next;
        _stateTimer = 0f;
        _agent.ResetPath();
        _agent.isStopped = false;

        switch (next)
        {
            case State.Idle:
                _stateTimer = Random.Range(idleMinTime, idleMaxTime);
                _agent.speed = 0f;
                break;

            case State.Wander:
                _agent.speed = walkSpeed;
                _agent.SetDestination(RandomNavSphere(transform.position, wanderRadius));
                break;

            case State.Approach:
                _agent.speed = walkSpeed;
                _curiosityTimer = curiosityTime;
                break;

            case State.Retreat:
                _agent.speed = walkSpeed;
                Vector3 dir = (transform.position - _player.position).normalized;
                Vector3 tgt = transform.position + dir * retreatDistance;
                _agent.SetDestination(ClampToNavMesh(tgt));
                break;

            case State.Flee:
                _agent.speed = runSpeed;
                Vector3 fleeDir = (transform.position - _player.position).normalized;
                Vector3 fleeDest = transform.position + fleeDir * comfortMax * 2f;
                _agent.SetDestination(ClampToNavMesh(fleeDest));
                break;

            case State.Sleep:
                _stateTimer = sleepTime;
                _agent.isStopped = true;
                _anim.SetFloat("Speed", 0f);
                break;
        }
    }

    private void ApproachLogic(float dist)
    {
        if (dist > comfortMin)
            _agent.SetDestination(_player.position);
        else
        {
            _agent.ResetPath();
            LookAtPlayer();
        }

        _curiosityTimer -= Time.deltaTime;
        if (_curiosityTimer <= 0f || dist > curiosityRange * 1.2f)
            EndPlayerInteraction();
    }

    private void EndPlayerInteraction()
    {
        _cooldownTimer = curiosityCooldown;
        SwitchState(State.Retreat);           // сначала уходим, потом Wander
    }

    /* ────────────── защита от застревания ────────────── */
    private void DetectStuck(float dt)
    {
        bool pathBad = !_agent.hasPath || _agent.pathStatus != NavMeshPathStatus.PathComplete;
        bool noMove = _agent.velocity.sqrMagnitude < stuckSpeed * stuckSpeed;

        if ((_state == State.Wander || _state == State.Approach ||
             _state == State.Retreat || _state == State.Flee) &&
            (pathBad || noMove))
        {
            _stuckTimer += dt;
            if (_stuckTimer >= stuckTime)
            {
                _stuckTimer = 0f;
                SwitchState(State.Idle);
            }
        }
        else _stuckTimer = 0f;
    }

    /* ────────────── helpers ────────────── */

    private void LookAtPlayer()
    {
        Vector3 dir = _player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(dir), 180f * Time.deltaTime);
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float radius)
    {
        Vector3 rand = Random.insideUnitSphere * radius + origin;
        NavMesh.SamplePosition(rand, out NavMeshHit hit, radius, NavMesh.AllAreas);
        return hit.position;
    }

    /// <summary>Коротко: та же выборка, но гарантированно возвращает точку на навмеш‑карте.</summary>
    private static Vector3 ClampToNavMesh(Vector3 pos, float probe = 4f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, probe, NavMesh.AllAreas))
            return hit.position;
        return pos;        // fallback — хоть куда‑то
    }
}
