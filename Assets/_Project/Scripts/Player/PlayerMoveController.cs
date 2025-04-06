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
    [SerializeField] private List<AudioClip> defaultFootsteps;  // Базовые звуки шагов
    // Звуки для конкретных сцен (заполняются из Resources)
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    private CharacterController characterController;
    private Animator animator;
    private AudioSource footstepSource;

    private Vector3 moveDirection;
    private float verticalVelocity;

    // Для определения движения (используем разницу позиций)
    private Vector3 lastPosition;

    [Inject]
    private void Construct(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        footstepSource = GetComponent<AudioSource>();
        lastPosition = transform.position;

        // Инициализация звуков шагов для разных сцен
        sceneFootstepSounds.Add("MainRoom", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Wood") });
        sceneFootstepSounds.Add("Forge", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Grass") });
        sceneFootstepSounds.Add("Storage", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Stone") });

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateFootstepSounds(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Обработка движения и анимации выполняется в Update для максимальной отзывчивости

        HandleMovement();
        UpdateAnimator();

        lastPosition = transform.position;
    }

    private void HandleMovement()
    {
        // Получаем сырой ввод для мгновенной реакции
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(horizontal, 0, vertical);

        // Определяем текущую скорость (если зажат Shift – бег)
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= runMultiplier;
        }

        Vector3 desiredMove = Vector3.zero;
        if (input.sqrMagnitude > 0.01f)
        {
            // Получаем векторы направления камеры (без вертикали)
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Итоговое направление движения – комбинация ввода по осям
            desiredMove = (camForward * vertical + camRight * horizontal).normalized;

            // Поворот игрока в направлении движения (с плавностью)
            if (desiredMove != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        moveDirection = desiredMove * currentSpeed;

        // Гравитация: если игрок на земле – слегка прижимаем, иначе уменьшаем скорость падения
        if (characterController.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        moveDirection.y = verticalVelocity;

        // Перемещаем игрока через CharacterController
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        // Если позиция изменилась, считаем, что игрок ходит
        if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {

            // Если звук шагов не воспроизводится – проигрываем его
            if (!footstepSource.isPlaying)
            {
                PlayFootstep();
            }
        }

    }

    private void PlayFootstep()
    {
        AudioClip clip = GetRandomFootstep();
        if (clip != null)
        {
            footstepSource.clip = clip;
            footstepSource.Play();
        }
    }

    private AudioClip GetRandomFootstep()
    {
        List<AudioClip> currentFootsteps = sceneFootstepSounds.ContainsKey(SceneManager.GetActiveScene().name)
            ? sceneFootstepSounds[SceneManager.GetActiveScene().name]
            : defaultFootsteps;

        if (currentFootsteps.Count > 0)
        {
            return currentFootsteps[Random.Range(0, currentFootsteps.Count)];
        }
        return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateFootstepSounds(scene.name);
    }

    private void UpdateFootstepSounds(string sceneName)
    {
        // В данном примере мы просто обновляем выбранный звук для шагов,
        // он будет выбираться при воспроизведении.
        // Если нужно установить конкретный клип сейчас, можно это сделать здесь.
    }
}
