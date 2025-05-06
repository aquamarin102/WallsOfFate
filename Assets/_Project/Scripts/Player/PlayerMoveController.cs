using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;         // Базовая скорость
    [SerializeField] private float runMultiplier = 1.5f;     // Множитель скорости при беге (Shift)
    [SerializeField] private float rotationSpeed = 10f;      // Скорость поворота
    [SerializeField] private float gravity = 9.81f;          // Гравитация (если уровень плоский – можно уменьшить)
    [SerializeField] private Transform cameraTransform;      // Ссылка на камеру (обычно Main Camera)
    [SerializeField] private LayerMask groundMask;

    [Header("Click-to-Move Settings")]
    [SerializeField] private float stopThreshold = 0.1f;
    [SerializeField] private float clickRunThreshold = 0.3f; // max interval для двойного клика, сек.
    private bool isClickMove = false;
    private bool isClickRun = false;
    private Vector3 clickTarget;
    private float lastClickTime = -1f;

    [Header("Footstep Settings")]
    // Звуки для конкретных сцен (заполняются из Resources) – для каждой сцены два аудиоклипа (левый и правый)
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    [Header("Pitch Settings")]
    [SerializeField] private float walkingPitch = 1.0f;   // Базовый pitch при ходьбе
    [SerializeField] private float runningPitch = 1.5f;   // Pitch при беге (ускоренное воспроизведение звука)

    private CharacterController characterController;
    private NavMeshAgent agent;
    private AudioSource footstepSource;

    // Клипы для левой и правой ноги
    private AudioClip leftFootstepClip;
    private AudioClip rightFootstepClip;

    private Vector3 moveDirection;
    private float verticalVelocity;

    // Для отслеживания движения игрока
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

        // Отключаем автоматическое обновление позиции/вращения у агента:
        agent.updatePosition = false;
        agent.updateRotation = false;

        // Инициализация звуков шагов для разных сцен.
        // В Resources должны быть аудиоклипы с именами, например:
        // "Footsteps/WoodLeft", "Footsteps/WoodRight", "Footsteps/GrassLeft", "Footsteps/GrassRight", "Footsteps/StoneLeft", "Footsteps/StoneRight"
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
        HandleClickInput();
        HandleMovement();
        UpdateAnimator();
        lastPosition = transform.position;
    }

    private void HandleClickInput()
    {
        if (Input.GetMouseButtonDown(1) &&
            DialogueManager.GetInstance()?.DialogueIsPlaying == false)
        {
            float time = Time.time;
            // Детект двойного клика
            if (time - lastClickTime <= clickRunThreshold)
                isClickRun = true;
            else
                isClickRun = false;
            lastClickTime = time;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f, groundMask))
            {
                clickTarget = hit.point;
                isClickMove = true;
                agent.SetDestination(clickTarget);
                agent.isStopped = false;
            }
        }
    }

    private void HandleMovement()
    {
        if (DialogueManager.GetInstance()?.DialogueIsPlaying == true)
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
            characterController.Move(moveDirection * Time.deltaTime);
            agent.isStopped = true;
            return;
        }

        // WASD ввод
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        if (input.sqrMagnitude > 0.01f)
        {
            isClickMove = false;
            isClickRun = false;
            agent.isStopped = true;
            agent.ResetPath();
        }

        Vector3 desiredMove = Vector3.zero;

        if (isClickMove && !agent.pathPending)
        {
            // NavMeshAgent.desiredVelocity обходят препятствия
            Vector3 vel = agent.desiredVelocity;
            desiredMove = new Vector3(vel.x, 0, vel.z).normalized;

            if (Vector3.Distance(transform.position, clickTarget) <= stopThreshold)
            {
                isClickMove = false;
                isClickRun = false;
                agent.isStopped = true;
            }
        }
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Скорость (учитываем double-click run)
        float speed = moveSpeed;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || isClickRun;
        if (isRunning) speed *= runMultiplier;
        moveDirection = desiredMove * speed;

        // Гравитация
        if (characterController.isGrounded) verticalVelocity = -1f;
        else verticalVelocity -= gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);

        // Синхронизируем агента с позицией игрока
        agent.nextPosition = transform.position;
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().DialogueIsPlaying)
            return;

        if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {
            if (!footstepSource.isPlaying)
            {
                PlayFootstep();
            }
        }
    }

    private void PlayFootstep()
    {
        // Перед воспроизведением выставляем pitch в зависимости от режима (бег/ходьба)
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        footstepSource.pitch = isRunning ? runningPitch : walkingPitch;

        if (leftFootstepClip != null && rightFootstepClip != null)
        {
            if (isLeftFootStep)
            {
                footstepSource.PlayOneShot(leftFootstepClip);
            }
            else
            {
                footstepSource.PlayOneShot(rightFootstepClip);
            }
            isLeftFootStep = !isLeftFootStep;
        }
        else
        {
            Debug.LogWarning("Звуки шагов для текущей сцены не настроены!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateFootstepSounds(scene.name);
    }

    private void UpdateFootstepSounds(string sceneName)
    {
        if (sceneFootstepSounds.ContainsKey(sceneName) && sceneFootstepSounds[sceneName].Count >= 2)
        {
            leftFootstepClip = sceneFootstepSounds[sceneName][0];
            rightFootstepClip = sceneFootstepSounds[sceneName][1];
        }
        else
        {
            leftFootstepClip = null;
            rightFootstepClip = null;
            Debug.LogWarning($"Для сцены \"{sceneName}\" не заданы два звука шагов.");
        }
    }
}
