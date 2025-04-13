using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Footstep Settings")]
    // Звуки для конкретных сцен (заполняются из Resources) – для каждой сцены два аудиоклипа (левый и правый)
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    [Header("Pitch Settings")]
    [SerializeField] private float walkingPitch = 1.0f;   // Базовый pitch при ходьбе
    [SerializeField] private float runningPitch = 1.5f;   // Pitch при беге (ускоренное воспроизведение звука)

    private CharacterController characterController;
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
        footstepSource = GetComponent<AudioSource>();
        lastPosition = transform.position;

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
        HandleMovement();
        UpdateAnimator();
        lastPosition = transform.position;
    }

    private void HandleMovement()
    {
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().DialogueIsPlaying)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            characterController.Move(new Vector3(0, moveDirection.y, 0) * Time.deltaTime);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(horizontal, 0, vertical);

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= runMultiplier;
        }

        Vector3 desiredMove = Vector3.zero;
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            desiredMove = (camForward * vertical + camRight * horizontal).normalized;

            if (desiredMove != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        moveDirection = desiredMove * currentSpeed;

        if (characterController.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
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
