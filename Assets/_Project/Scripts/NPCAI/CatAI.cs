using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CatAI : MonoBehaviour
{
    [Header("Параметры отношения к игроку")]
    public float approachRange = 5f;       // Если игрок в пределах 5 м – кот начинает подходить
    public float tooCloseRange = 1.5f;       // Если игрок слишком близко (< 1.5 м) – кот инициирует отступление (через Turn)
    public float retreatExitRange = 3f;      // При расстоянии > 3 м из режима Retreat кот возвращается в Idle

    [Header("Параметры подхода (комфортный диапазон)")]
    public float minApproachDistance = 2f;   // Если кот оказывается ближе 2 м – считаем, что он слишком близко
    public float maxApproachDistance = 4f;   // Комфортное расстояние, при котором коту нравится находиться рядом с игроком

    [Header("Параметры перемещения")]
    public float wanderRadius = 10f;       // Радиус для случайного блуждания
    public float idleTime = 2f;            // Время простоя в состоянии Idle
    public float wanderSpeed = 1.5f;       // Скорость блуждания
    public float approachSpeed = 2.5f;     // Скорость при подходе к игроку
    public float retreatSpeed = 3.5f;      // Скорость отступления при слишком близком приближении

    [Header("Параметры усталости")]
    public float followMaxTime = 10f;      // Максимальное время, которое кот будет следовать за игроком

    [Header("Параметры поворота")]
    public float turnDuration = 0.5f;      // Время, которое кот поворачивается на месте

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    [Inject]
    private void Construct(PlayerMoveController _player)
    {
        Debug.Log("Player injected: " + _player);
        player = _player.gameObject.transform;
    }

    // Определяем состояния кота
    private enum CatState { Idle, Wander, Approach, Turn, Retreat }
    private CatState currentState;

    private float idleTimer;
    private float followTimer;  // Таймер нахождения в режиме Approach
    private float turnTimer;    // Таймер для поворота в состоянии Turn
    private Vector3 wanderTarget;
    private Quaternion targetRotation; // Желательная ориентация при повороте

    /// <summary>
    /// Устанавливает параметры аниматора по той же схеме, что и у курицы:
    /// - Idle: Vert = 0, State = 0
    /// - Ходьба: Vert = 1, State = 0
    /// - Бег (отступление): Vert = 1, State = 1
    /// </summary>
    private void SetAnimatorParameters(float vert, int state)
    {
        animator.SetFloat("Vert", vert);
        animator.SetInteger("State", state);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentState = CatState.Idle;
        idleTimer = idleTime;
        followTimer = 0f;
        turnTimer = 0f;
        SetAnimatorParameters(0f, 0);
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Логика переключения состояний с учётом комфортного диапазона и поворота
        switch (currentState)
        {
            case CatState.Idle:
            case CatState.Wander:
                if (distToPlayer < approachRange && distToPlayer > maxApproachDistance)
                {
                    currentState = CatState.Approach;
                    followTimer = 0f; // Сброс таймера при входе в режим Approach
                }
                if (distToPlayer < tooCloseRange)
                {
                    // Вместо прямого перехода в Retreat переходим в Turn
                    PrepareTurn();
                }
                break;

            case CatState.Approach:
                if (distToPlayer < tooCloseRange)
                {
                    PrepareTurn();
                }
                else if (distToPlayer > approachRange)
                {
                    currentState = CatState.Idle;
                    idleTimer = idleTime;
                    followTimer = 0f;
                }
                else
                {
                    followTimer += Time.deltaTime;
                    if (followTimer > followMaxTime)
                    {
                        // Коту надоело следовать за игроком – переходим в режим блуждания
                        currentState = CatState.Wander;
                        followTimer = 0f;
                    }
                }
                break;

            case CatState.Turn:
                // В состоянии Turn просто ждем, пока не пройдет заданное время поворота или не достигнем нужной ориентации
                turnTimer -= Time.deltaTime;
                // Плавное поворотное движение с использованием Quaternion.RotateTowards
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
                if (turnTimer <= 0f || Quaternion.Angle(transform.rotation, targetRotation) < 5f)
                {
                    // Когда поворот завершен, переходим в режим Retreat
                    currentState = CatState.Retreat;
                }
                break;

            case CatState.Retreat:
                if (distToPlayer > retreatExitRange)
                {
                    currentState = CatState.Idle;
                    idleTimer = idleTime;
                }
                break;
        }

        // Выполнение действий по текущему состоянию
        switch (currentState)
        {
            case CatState.Idle:
                HandleIdleState();
                break;
            case CatState.Wander:
                HandleWanderState();
                break;
            case CatState.Approach:
                HandleApproachState();
                break;
            case CatState.Turn:
                // В состоянии Turn движение не производится – агент останавливается
                agent.speed = 0f;
                SetAnimatorParameters(0f, 0); // Может использоваться Idle-анимация
                break;
            case CatState.Retreat:
                HandleRetreatState();
                break;
        }
    }

    /// <summary>
    /// Метод подготовки к повороту: вычисляет желаемую ориентацию (targetRotation) так, чтобы кот лицом смотрел в сторону от игрока,
    /// устанавливает состояние Turn и запускает таймер поворота.
    /// </summary>
    void PrepareTurn()
    {
        // Вычисляем направление от игрока к коту
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        targetRotation = Quaternion.LookRotation(retreatDirection);
        currentState = CatState.Turn;
        turnTimer = turnDuration;
        // Сброс followTimer, если необходимо
        followTimer = 0f;
    }

    /// <summary>
    /// Состояние Idle – кот стоит на месте.
    /// </summary>
    void HandleIdleState()
    {
        SetAnimatorParameters(0f, 0);
        agent.speed = 0f;
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            currentState = CatState.Wander;
            ChooseNewWanderTarget();
        }
    }

    /// <summary>
    /// Состояние Wander – кот случайно блуждает по локации.
    /// </summary>
    void HandleWanderState()
    {
        SetAnimatorParameters(1f, 0);
        agent.speed = wanderSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = CatState.Idle;
            idleTimer = idleTime;
        }
    }

    /// <summary>
    /// Состояние Approach – кот идёт к игроку (любопытство, привязанность).
    /// </summary>
    void HandleApproachState()
    {
        SetAnimatorParameters(1f, 0);
        agent.speed = approachSpeed;
        agent.SetDestination(player.position);
    }

    /// <summary>
    /// Состояние Retreat – кот быстро отступает, если игрок слишком близко.
    /// </summary>
    void HandleRetreatState()
    {
        SetAnimatorParameters(1f, 1);
        agent.speed = retreatSpeed;
        // Рассчитываем направление для отступления (оно уже было определено в PrepareTurn)
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatDestination = transform.position + retreatDirection * 5f;
        agent.SetDestination(retreatDestination);
    }

    /// <summary>
    /// Выбирает случайную точку для блуждания в пределах wanderRadius.
    /// </summary>
    void ChooseNewWanderTarget()
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
}
