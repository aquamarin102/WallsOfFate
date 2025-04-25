using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
public class CatAI : MonoBehaviour
{
    #region ── Inspector ──────────────────────────────────────────────────────
    [Header("Player reaction")]
    [SerializeField] float curiosityRange = 6f;
    [SerializeField] float comfortMin = 2f;
    [SerializeField] float comfortMax = 3.5f;
    [SerializeField] float approachReenterDistance = 8f;
    [SerializeField] float panicRange = 1.2f;

    [SerializeField] float fleeEndDistance = 7f;

    [Header("Move speeds (m/s)")]
    [SerializeField] float walkSpeed = 1.6f;
    [SerializeField] float runSpeed = 3.5f;

    [Header("Cat routine")]
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float idleMinTime = 2f;
    [SerializeField] float idleMaxTime = 4f;
    [SerializeField] float curiosityTime = 5f;
    [SerializeField] float curiosityCooldown = 8f;
    [SerializeField] float retreatDistance = 5f;
    [SerializeField] float sleepChance = 0.1f;
    [SerializeField] float sleepTime = 10f;

    [Header("Anti-stuck & edges")]
    [SerializeField] float stuckSpeed = .05f;
    [SerializeField] float stuckTime = 2f;
    [SerializeField] float edgeAvoidDistance = .25f;

    [Header("Turn-in-place")]
    [SerializeField] float turnSpeed = 420f;
    [SerializeField] float turnAngleThreshold = 5f;
    #endregion

    enum State { Idle, Turn, Wander, Approach, Retreat, Flee, Sleep }
    State _state;

    /* components */
    NavMeshAgent _agent;
    Animator _anim;
    Transform _player;

    /* timers */
    float _stateTimer, _curiosityTimer, _cooldownTimer, _stuckTimer;

    /* turn helpers */
    Vector3 _pendingDestination;
    float _pendingSpeed;
    State _afterTurnState;

    /* DI */
    [Inject] void Construct(PlayerMoveController pc) => _player = pc.transform;

    /* ──────────────────────────────────────────────────────────────── */

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _agent.acceleration = runSpeed * 2f;
        _agent.angularSpeed = 720f;
        _agent.autoBraking = false;
        _agent.updateRotation = false;   // моделью крутим сами
    }

    void OnEnable() => SwitchState(State.Idle);

    void Update()
    {
        float dt = Time.deltaTime;

        DetectThreats();

        switch (_state)
        {
            case State.Idle: TickIdle(dt); break;
            case State.Turn: TickTurn(dt); break;
            case State.Wander: TickWander(dt); break;
            case State.Approach: TickApproach(dt); break;
            case State.Retreat: TickRetreat(dt); break;
            case State.Flee: TickFlee(dt); break;
            case State.Sleep: TickSleep(dt); break;
        }

        Animate(dt);

        if (_state != State.Turn && _agent.velocity.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.RotateTowards(
                                    transform.rotation, look,
                                    turnSpeed * dt);            // тот же turnSpeed
        }
    }

    /* ====================== STATE MACHINE ====================== */

    void SwitchState(State next)
    {
        _state = next;
        _stateTimer = _curiosityTimer = _stuckTimer = 0f;

        switch (next)
        {
            case State.Idle:
                _stateTimer = Random.Range(idleMinTime, idleMaxTime);
                _agent.ResetPath();
                break;

            case State.Wander:
                BeginMove(RandomSafePoint(transform.position, wanderRadius), walkSpeed, State.Wander);
                break;

            case State.Approach:
                _curiosityTimer = curiosityTime;
                BeginMove(SafePointNear(_player.position), walkSpeed, State.Approach);
                break;

            case State.Retreat:
                BeginMove(SafePointNear(transform.position - DirToPlayer() * retreatDistance),
                          walkSpeed, State.Retreat);
                break;

            case State.Flee:
                PickFleeDestination();
                break;

            case State.Sleep:
                _stateTimer = sleepTime;
                _agent.ResetPath();
                break;

            case State.Turn: /* managed internally */ break;
        }
    }

    /* ── individual ticks ───────────────────────────────────────── */

    void TickIdle(float dt)
    {
        if ((_stateTimer -= dt) <= 0f)
            SwitchState(Random.value < sleepChance ? State.Sleep : State.Wander);
    }

    void TickTurn(float dt)
    {
        RotateTowards(_pendingDestination, dt);

        if (FacingTarget(_pendingDestination))
        {
            _agent.speed = _pendingSpeed;
            _agent.isStopped = false;
            _state = _afterTurnState;
        }
    }

    void TickWander(float dt)
    {
        AntiStuck(dt);
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            SwitchState(State.Idle);
    }

    void TickApproach(float dt)
    {
        float dist = DistToPlayer();

        if (dist > comfortMin)
            _agent.SetDestination(SafePointNear(_player.position));
        else
            _agent.ResetPath(); // остановка, наблюдаем

        _curiosityTimer -= dt;
        if (_curiosityTimer <= 0f || dist > curiosityRange * 1.2f)
            EndPlayerInteraction();

        AntiStuck(dt);
    }

    void TickRetreat(float dt)
    {
        AntiStuck(dt);
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            SwitchState(State.Wander);
    }

    void TickFlee(float dt)
    {
        AntiStuck(dt);

        // ▸ 1. как только добежали до предыдущей точки – строим новую
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            PickFleeDestination();          // ← выберет новую точку «подальше от игрока»

        // ▸ 2. прекращаем убегать, только если игрок окончательно отстал
        if (DistToPlayer() > fleeEndDistance)
            EndPlayerInteraction();
    }

    void TickSleep(float dt)
    {
        if ((_stateTimer -= dt) <= 0f)
            SwitchState(State.Idle);
    }

    /* ====================== HELPERS ====================== */

    /* -- threats & curiosity ------------------------------------- */
    void DetectThreats()
    {
        float dist = DistToPlayer();

        if (_state != State.Flee && dist < panicRange)
        {
            SwitchState(State.Flee);
            return;
        }

        if (_state is State.Idle or State.Wander or State.Sleep)
        {
            bool interesting = dist < curiosityRange && dist > comfortMax;
            bool cooled = _cooldownTimer <= 0f && dist > approachReenterDistance;
            if (interesting && cooled) SwitchState(State.Approach);
        }

        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;
    }

    void EndPlayerInteraction()
    {
        _cooldownTimer = curiosityCooldown;
        SwitchState(State.Retreat);
    }

    /* -- movement ------------------------------------------------ */
    void BeginMove(Vector3 dest, float speed, State afterTurn)
    {
        _pendingDestination = dest;
        _pendingSpeed = speed;
        _afterTurnState = afterTurn;

        _agent.SetDestination(dest); // пусть рассчитывает путь
        _agent.speed = 0f;       // тормозим на месте
        _agent.isStopped = true;

        _state = State.Turn;
    }

    void RotateTowards(Vector3 target, float dt)
    {
        Vector3 dir = target - transform.position; dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion tgt = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tgt, turnSpeed * dt);
    }

    bool FacingTarget(Vector3 tgt) =>
        Quaternion.Angle(transform.rotation,
                         Quaternion.LookRotation((tgt - transform.position).normalized)) <= turnAngleThreshold;

    /* -- anti-stuck ---------------------------------------------- */
    void AntiStuck(float dt)
    {
        bool slow = _agent.velocity.sqrMagnitude < stuckSpeed * stuckSpeed;
        bool bad = _agent.pathStatus != NavMeshPathStatus.PathComplete;

        if (slow || bad) _stuckTimer += dt; else _stuckTimer = 0f;

        if (_stuckTimer >= stuckTime)
        {
            _stuckTimer = 0f;
            Replan();
        }
    }

    void Replan()
    {
        switch (_state)
        {
            case State.Wander:
                BeginMove(RandomSafePoint(transform.position, wanderRadius), walkSpeed, State.Wander);
                break;
            case State.Approach:
                BeginMove(SafePointNear(_player.position), walkSpeed, State.Approach);
                break;
            case State.Retreat:
            case State.Flee:
                PickFleeDestination();
                break;
        }
    }

    /* -- flee with fan search ------------------------------------ */
    void PickFleeDestination()
    {
        Vector3 away = DirToPlayer();
        float[] angs = { 0, -30, 30, -60, 60, -90, 90 };

        foreach (float a in angs)
        {
            Vector3 dir = Quaternion.AngleAxis(a, Vector3.up) * away;
            Vector3 dst = SafePointNear(transform.position + dir * retreatDistance);
            if (dst != Vector3.zero)
            {
                BeginMove(dst, runSpeed, State.Flee);
                return;
            }
        }
        // fallback – просто блуждаем
        BeginMove(RandomSafePoint(transform.position, wanderRadius), walkSpeed, State.Wander);
    }

    /* -- utilities ----------------------------------------------- */
    float DistToPlayer() => Vector3.Distance(transform.position, _player.position);
    Vector3 DirToPlayer() => (transform.position - _player.position).normalized;

    Vector3 RandomSafePoint(Vector3 origin, float radius, int attempts = 8)
    {
        for (int i = 0; i < attempts; i++)
        {
            Vector3 cand = origin + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(cand, out var hit, radius, NavMesh.AllAreas) &&
                !NearEdge(hit.position))
                return hit.position;
        }
        return ClampToNavMesh(origin);
    }

    Vector3 SafePointNear(Vector3 desired) =>
        NearEdge(desired) ? RandomSafePoint(desired, wanderRadius * .5f)
                          : ClampToNavMesh(desired);

    bool NearEdge(Vector3 pos) =>
        NavMesh.FindClosestEdge(pos, out var hit, NavMesh.AllAreas) && hit.distance < edgeAvoidDistance;

    Vector3 ClampToNavMesh(Vector3 pos, float probe = 4f) =>
        NavMesh.SamplePosition(pos, out var hit, probe, NavMesh.AllAreas) ? hit.position : pos;

    /* -- animation ----------------------------------------------- */
    void Animate(float dt)
    {
        float speedNorm = _agent.velocity.magnitude / runSpeed;
        _anim.SetFloat("Speed", speedNorm, 0.1f, dt);
    }
}
