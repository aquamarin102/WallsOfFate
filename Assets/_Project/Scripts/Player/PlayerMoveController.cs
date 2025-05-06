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

    [Header("Mouse-Follow & Click Settings")]
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private float clickRunThreshold = 0.3f;  // макс. интервал для double-click
    [SerializeField] private float holdThreshold = 0.2f;  // время удержания, чтобы перейти в follow-режим

    private bool isHoldMove = false;  // удерживаем правую кнопку
    private bool isClickRun = false;  // бег по двойному клику
    private float lastClickTime = -1f;
    private float mouseDownTime = 0f;
    private Vector3 clickTarget;

    [Header("Footstep Settings")]
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new();
    private AudioClip leftFootstepClip, rightFootstepClip;

    [Header("Pitch Settings")]
    [SerializeField] private float walkingPitch = 1f;
    [SerializeField] private float runningPitch = 1.5f;

    // ссылки на компоненты
    private CharacterController characterController;
    private NavMeshAgent agent;
    private AudioSource footstepSource;
    private NavMeshPath navMeshPath;

    // внутренняя логика движения
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

        // NavMeshAgent — только для расчёта пути
        agent.updatePosition = false;
        agent.updateRotation = false;

        navMeshPath = new NavMeshPath();

        // Инициализация звуков
        sceneFootstepSounds.Add("MainRoom", new List<AudioClip>()
        {
            Resources.Load<AudioClip>("Footsteps/wood1"),
            Resources.Load<AudioClip>("Footsteps/wood2")
        });
        sceneFootstepSounds.Add("Forge", new List<AudioClip>()
        {
            Resources.Load<AudioClip>("Footsteps/gravel1"),
            Resources.Load<AudioClip>("Footsteps/gravel2")
        });
        sceneFootstepSounds.Add("Storage", new List<AudioClip>()
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

    private void HandleMouseInput()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
            return;

        // 1) Нажатие правой кнопки:
        if (Input.GetMouseButtonDown(1))
        {
            float now = Time.time;
            // проверка на двойной клик
            isClickRun = (now - lastClickTime) <= clickRunThreshold;
            lastClickTime = now;
            mouseDownTime = now;
        }

        // 2) Удержание кнопки:
        if (Input.GetMouseButton(1))
        {
            // если уже держим достаточно долго — переходим в follow-режим
            if (!isHoldMove && Time.time - mouseDownTime >= holdThreshold)
            {
                isHoldMove = true;
            }
        }

        // 3) Отпуск правой кнопки:
        if (Input.GetMouseButtonUp(1))
        {
            float holdTime = Time.time - mouseDownTime;

            // если отпустили раньше, чем holdThreshold — это щелчок
            if (holdTime < holdThreshold)
            {
                // пробиваем цель по щелчку
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100f, groundMask))
                {
                    // проверяем, что путь полный
                    bool valid = agent.CalculatePath(hit.point, navMeshPath)
                                 && navMeshPath.status == NavMeshPathStatus.PathComplete;
                    if (valid)
                    {
                        clickTarget = hit.point;
                        isHoldMove = false;
                        agent.SetDestination(clickTarget);
                        agent.isStopped = false;
                    }
                }
            }

            // сбрасываем follow-режим, но не isClickRun — он понадобится для скорости
            isHoldMove = false;
        }
    }

    private void HandleMovement()
    {
        // если идёт диалог — только гравитация
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
            characterController.Move(moveDirection * Time.deltaTime);
            agent.isStopped = true;
            return;
        }

        // 0) Если есть ручной WASD — сразу прерываем любой клик-режим
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);
        if (input.sqrMagnitude > 0.01f)
        {
            isHoldMove = false;
            isClickRun = false;
            agent.isStopped = true;
            agent.ResetPath();
        }

        Vector3 desiredMove = Vector3.zero;

        // 1) Follow-режим (удержание кнопки)
        if (isHoldMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, groundMask))
            {
                if (agent.CalculatePath(hit.point, navMeshPath)
                    && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    clickTarget = hit.point;
                    agent.SetDestination(clickTarget);
                    agent.isStopped = false;
                    Vector3 vel = agent.desiredVelocity;
                    desiredMove = new Vector3(vel.x, 0, vel.z).normalized;
                }
            }
        }
        // 2) Click-to-point (статичная цель послe щелчка)
        else if (agent.hasPath && !agent.isStopped)
        {
            Vector3 vel = agent.desiredVelocity;
            desiredMove = new Vector3(vel.x, 0, vel.z).normalized;
            if (Vector3.Distance(transform.position, clickTarget) <= stopThreshold)
            {
                agent.isStopped = true;
                isClickRun = false;
            }
        }
        // 3) Обычное WASD/камерное движение
        else if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();
            desiredMove = (camF * v + camR * h).normalized;
        }

        // Поворот
        if (desiredMove != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime
            );
        }

        // Скорость (учитываем Shift или double-click run)
        bool isRunning = Input.GetKey(KeyCode.LeftShift)
                         || Input.GetKey(KeyCode.RightShift)
                         || isClickRun;
        float speed = moveSpeed * (isRunning ? runMultiplier : 1f);
        moveDirection = desiredMove * speed;

        // Гравитация
        verticalVelocity = characterController.isGrounded
            ? -1f
            : verticalVelocity - gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;

        // Перемещаем CharacterController
        characterController.Move(moveDirection * Time.deltaTime);

        // Синхронизация позиции агента
        agent.nextPosition = transform.position;
    }

    private void UpdateAnimator()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
            return;

        if (Vector3.Distance(transform.position, lastPosition) > 0.001f
            && !footstepSource.isPlaying)
        {
            PlayFootstep();
        }
    }

    private void PlayFootstep()
    {
        bool running = Input.GetKey(KeyCode.LeftShift)
                       || Input.GetKey(KeyCode.RightShift)
                       || isClickRun;
        footstepSource.pitch = running ? runningPitch : walkingPitch;

        if (leftFootstepClip != null && rightFootstepClip != null)
        {
            footstepSource.PlayOneShot(
                isLeftFootStep ? leftFootstepClip : rightFootstepClip
            );
            isLeftFootStep = !isLeftFootStep;
        }
        else
        {
            Debug.LogWarning("Звуки шагов не настроены!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateFootstepSounds(scene.name);
    }

    private void UpdateFootstepSounds(string sceneName)
    {
        if (sceneFootstepSounds.TryGetValue(sceneName, out var list)
            && list.Count >= 2)
        {
            leftFootstepClip = list[0];
            rightFootstepClip = list[1];
        }
        else
        {
            leftFootstepClip = rightFootstepClip = null;
            Debug.LogWarning($"Для сцены «{sceneName}» звуки шагов не заданы.");
        }
    }
}
