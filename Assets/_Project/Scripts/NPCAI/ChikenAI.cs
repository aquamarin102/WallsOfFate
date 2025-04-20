using UnityEngine;
using UnityEngine.AI;

public class ChickenAI : MonoBehaviour
{
    public float detectionRange = 5f;    // Радиус, при котором курица начинает убегать
    public float fleeSpeed = 3.5f;         // Скорость при убегании
    public float wanderSpeed = 1.5f;       // Скорость при блуждании
    public float idleTime = 2f;            // Время простоя в состоянии Idle
    public float wanderRadius = 10f;       // Радиус выбора случайной точки для блуждания

    private NavMeshAgent agent;
    private Animator animator;

    // Ссылка на игрока. Можно присвоить через инспектор или установить по тегу "Player" в методе Start.
    public Transform player;

    // Определяем состояния курицы
    private enum ChickenState { Idle, Wander, Flee }
    private ChickenState currentState;

    private float idleTimer;
    private Vector3 wanderTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Если ссылка на игрока не установлена через инспектор, ищем объект с тегом "Player"
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Игрок не найден! Установите тег 'Player' для объекта игрока или укажите ссылку в инспекторе.");
            }
        }

        // Начальное состояние – Idle
        currentState = ChickenState.Idle;
        idleTimer = idleTime;
        SetAnimatorParameters(0f, 0);
    }

    void Update()
    {
        // Проверяем угрозу от игрока
        CheckForThreats();

        switch (currentState)
        {
            case ChickenState.Idle:
                HandleIdleState();
                break;
            case ChickenState.Wander:
                HandleWanderState();
                break;
            case ChickenState.Flee:
                HandleFleeState();
                break;
        }
    }

    /// <summary>
    /// Устанавливаем параметры аниматора согласно системе:
    /// - Idle: Vert = 0, State = 0
    /// - Walk: Vert = 1, State = 0
    /// - Run:  Vert = 1, State = 1
    /// </summary>
    private void SetAnimatorParameters(float vert, int state)
    {
        animator.SetFloat("Vert", vert);
        animator.SetInteger("State", state);
    }

    private void HandleIdleState()
    {
        // Idle: курица стоит на месте
        SetAnimatorParameters(0f, 0);
        agent.speed = 0f; // Агент не двигается в Idle

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            // Переход к блужданию
            currentState = ChickenState.Wander;
            ChooseNewWanderTarget();
        }
    }

    private void HandleWanderState()
    {
        // Блуждание (Walk):
        SetAnimatorParameters(1f, 0);
        agent.speed = wanderSpeed;

        // Если курица достигла цели блуждания
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = ChickenState.Idle;
            idleTimer = idleTime;
        }
    }

    private void HandleFleeState()
    {
        // Убегание (Run):
        SetAnimatorParameters(1f, 1);
        agent.speed = fleeSpeed;

        // Вычисляем направление для убегания от угрозы (игрока)
        Vector3 fleeDirection = GetFleeDirection();
        Vector3 fleeDestination = transform.position + fleeDirection * 5f; // Длина рывка
        agent.SetDestination(fleeDestination);

        // Если игрок отходит достаточно далеко, возвращаемся в Idle
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            currentState = ChickenState.Idle;
            idleTimer = idleTime;
        }
    }

    /// <summary>
    /// Выбираем случайную точку для блуждания в пределах заданного радиуса.
    /// </summary>
    private void ChooseNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
    }

    /// <summary>
    /// Проверка угрозы: если игрок находится ближе, чем detectionRange, переключаем в режим Flee.
    /// </summary>
    private void CheckForThreats()
    {
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer < detectionRange)
        {
            currentState = ChickenState.Flee;
        }
    }

    /// <summary>
    /// Возвращает нормализованное направление от игрока к курице.
    /// </summary>
    private Vector3 GetFleeDirection()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 direction = transform.position - player.position;
        direction.Normalize();
        return direction;
    }
}
