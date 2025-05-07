using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask groundMask;

    [Header("Mouse & Click Settings")]
    [SerializeField] private float stopThreshold = 0.1f; // для статичных целей
    [SerializeField] private float clickRunThreshold = 0.3f;  // двойной клик
    [SerializeField] private float holdThreshold = 0.2f;  // удержание для follow

    private bool isHoldMove = false;
    private bool isClickRun = false;
    private float lastClickTime = -1f;
    private float mouseDownTime = 0f;
    private Vector3 clickTarget;

    // динамическая цель (бегущая курица и т.п.)
    private Transform dynamicTarget = null;
    private float dynamicStopDist = 1f;

    private Action _onArriveAction;

    [Header("Footstep Settings")]
    private readonly Dictionary<string, List<AudioClip>> sceneFootstepSounds = new();
    private AudioClip leftFootstepClip, rightFootstepClip;

    [Header("Pitch Settings")]
    [SerializeField] private float walkingPitch = 1f;
    [SerializeField] private float runningPitch = 1.5f;

    private CharacterController characterController;
    private NavMeshAgent agent;
    private AudioSource footstepSource;
    private NavMeshPath navMeshPath;

    private Vector3 moveDirection;
    private float verticalVelocity;
    private Vector3 lastPosition;
    private bool isLeftFootStep = true;

    [Inject]
    private void Construct(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        footstepSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
        navMeshPath = new NavMeshPath();

        // NavMeshAgent только для расчёта пути
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.stoppingDistance = stopThreshold;

        // пример инициализации звуков
        sceneFootstepSounds.Add("MainRoom", new()
        {
            Resources.Load<AudioClip>("Footsteps/wood1"),
            Resources.Load<AudioClip>("Footsteps/wood2")
        });
        sceneFootstepSounds.Add("Forge", new()
        {
            Resources.Load<AudioClip>("Footsteps/gravel1"),
            Resources.Load<AudioClip>("Footsteps/gravel2")
        });
        sceneFootstepSounds.Add("Storage", new()
        {
            Resources.Load<AudioClip>("Footsteps/stone1"),
            Resources.Load<AudioClip>("Footsteps/stone2")
        });

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateFootstepSounds(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        HandleMouseInput();
        HandleMovement();
        UpdateAnimator();
        lastPosition = transform.position;
    }

    #region Mouse Input
    private void HandleMouseInput()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
            return;

        // 1) фиксируем нажатие ЛКМ и проверяем двойной клик
        if (Input.GetMouseButtonDown(0))
        {
            float now = Time.time;
            isClickRun = (now - lastClickTime) <= clickRunThreshold;
            lastClickTime = now;
            mouseDownTime = now;
        }

        // 2) удержание ЛКМ переключает нас в follow‑режим
        if (Input.GetMouseButton(0))
        {
            if (!isHoldMove && Time.time - mouseDownTime >= holdThreshold)
                isHoldMove = true;
        }

        // 3) отпуск ЛКМ — клик или завершение удержания
        if (Input.GetMouseButtonUp(0))
        {
            float held = Time.time - mouseDownTime;
            dynamicTarget = null; // кликом по земле сбрасываем преследование

            if (held < holdThreshold)
            {
                // RaycastAll, чтобы сперва ловить IInteractable
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray, 100f);
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                bool actionTaken = false;
                foreach (var hit in hits)
                {
                    // 1) Попали во взаимодействуемый предмет?
                    if (hit.collider.TryGetComponent<InteractableItem>(out var item) && !item.HasBeenUsed)
                    {
                        // Преследуем живой предмет (Transform) или идём к точке
                        if (item.GetComponent<NavMeshAgent>())
                        {
                            MoveToAndCallback(
                                item.transform,
                                isClickRun,
                                () => item.Interact(),
                                1.2f);
                        }
                        else
                        {
                            MoveToAndCallback(
                                hit.point,
                                isClickRun,
                                () => item.Interact(),
                                1.2f);
                        }
                        actionTaken = true;
                        break;
                    }
                    // 2) Клик по земле
                    else if (((1 << hit.collider.gameObject.layer) & groundMask) != 0)
                    {
                        if (agent.CalculatePath(hit.point, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                        {
                            MoveToAndCallback(hit.point, isClickRun, null);
                        }
                        actionTaken = true;
                        break;
                    }
                }
                if (!actionTaken)
                    isHoldMove = false;
            }
            isHoldMove = false;
        }
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
            characterController.Move(moveDirection * Time.deltaTime);
            agent.isStopped = true;
            return;
        }

        // прерываем мышиный режим при вводе WASD
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new(h, 0, v);
        if (input.sqrMagnitude > 0.01f)
        {
            isHoldMove = false;
            isClickRun = false;
            dynamicTarget = null;
            agent.isStopped = true;
            agent.ResetPath();
        }

        Vector3 desiredMove = Vector3.zero;

        // A) преследуем динамическую цель
        if (dynamicTarget != null)
        {
            if (!dynamicTarget.gameObject.activeInHierarchy)
            {
                StopMovement();
            }
            else
            {
                Vector3 tgtPos = dynamicTarget.position;
                if (agent.destination != tgtPos)
                    agent.SetDestination(tgtPos);

                desiredMove = agent.desiredVelocity.WithY(0).normalized;

                if (!agent.pathPending && agent.remainingDistance <= dynamicStopDist + 0.05f)
                {
                    agent.isStopped = true;
                    isClickRun = false;
                    var cb = _onArriveAction;
                    ClearDynamicTarget();
                    cb?.Invoke();
                }
            }
        }
        // B) follow‑режим (удержание ЛКМ)
        else if (isHoldMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, groundMask))
            {
                if (agent.CalculatePath(hit.point, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    clickTarget = hit.point;
                    agent.SetDestination(clickTarget);
                    agent.isStopped = false;
                    desiredMove = agent.desiredVelocity.WithY(0).normalized;
                }
            }
        }
        // C) click‑to‑point
        else if (agent.hasPath && !agent.isStopped)
        {
            desiredMove = agent.desiredVelocity.WithY(0).normalized;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                agent.isStopped = true;
                isClickRun = false;
                _onArriveAction?.Invoke();
                _onArriveAction = null;
            }
        }
        // D) WASD
        else if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();
            desiredMove = (camF * v + camR * h).normalized;
        }

        // поворот
        if (desiredMove != Vector3.zero)
        {
            Quaternion tgt = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, tgt, rotationSpeed * Time.deltaTime);
        }

        // скорость
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || isClickRun;
        float speed = moveSpeed * (isRunning ? runMultiplier : 1f);
        moveDirection = desiredMove * speed;

        // гравитация
        verticalVelocity = characterController.isGrounded ? -1f : verticalVelocity - gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
        agent.nextPosition = transform.position;
    }
    #endregion

    #region Public API
    // статическая цель (точка на земле / рядом с предметом)
    public void MoveToAndCallback(Vector3 target, bool run, Action onArrive, float stopDistance = 0.1f)
    {
        dynamicTarget = null;
        clickTarget = target;
        isClickRun = run;
        isHoldMove = false;

        agent.stoppingDistance = stopDistance;
        agent.SetDestination(clickTarget);
        agent.isStopped = false;

        _onArriveAction = onArrive;
    }

    // динамическая цель (бегущая курица и т.д.)
    public void MoveToAndCallback(Transform target, bool run, Action onArrive, float stopDistance = 1f)
    {
        dynamicTarget = target;
        dynamicStopDist = stopDistance;
        isClickRun = run;
        isHoldMove = false;
        _onArriveAction = onArrive;

        agent.stoppingDistance = stopDistance;
        agent.SetDestination(target.position);
        agent.isStopped = false;
    }

    public void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();
        isHoldMove = false;
        isClickRun = false;
        clickTarget = Vector3.zero;
        ClearDynamicTarget();
    }
    #endregion

    #region Helpers
    private void ClearDynamicTarget()
    {
        dynamicTarget = null;
        _onArriveAction = null;
    }

    private void UpdateAnimator()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true) return;
        if (Vector3.Distance(transform.position, lastPosition) > 0.001f && !footstepSource.isPlaying)
        {
            PlayFootstep();
        }
    }

    private void PlayFootstep()
    {
        bool running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || isClickRun;
        footstepSource.pitch = running ? runningPitch : walkingPitch;

        if (leftFootstepClip != null && rightFootstepClip != null)
        {
            footstepSource.PlayOneShot(isLeftFootStep ? leftFootstepClip : rightFootstepClip);
            isLeftFootStep = !isLeftFootStep;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => UpdateFootstepSounds(scene.name);

    private void UpdateFootstepSounds(string sceneName)
    {
        if (sceneFootstepSounds.TryGetValue(sceneName, out var list) && list.Count >= 2)
        {
            leftFootstepClip = list[0];
            rightFootstepClip = list[1];
        }
        else
        {
            leftFootstepClip = rightFootstepClip = null;
        }
    }
    #endregion
    // Extension‑method inside same file for convenience
}
public static class Vector3Extensions
{
    public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z); }