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
    // Используем один аудиоклип для шагов (будет выбран в зависимости от сцены)
    [SerializeField] private List<AudioClip> defaultFootsteps;
    // Звуки для конкретных сцен (заполняются из Resources)
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    [Header("Footstep Pitch Settings")]
    [SerializeField] private float walkingPitch = 1.0f;      // Базовый pitch при ходьбе
    [SerializeField] private float runningPitch = 1.2f;      // Базовый pitch при беге
    [SerializeField] private float stepPitchVariation = 0.05f; // Вариация для имитации разных ног

    private CharacterController characterController;
    private AudioSource footstepSource;

    private Vector3 moveDirection;
    private float verticalVelocity;

    // Для определения движения (используем разницу позиций)
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
        bool isRunning = false;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= runMultiplier;
            isRunning = true;
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
        if (footstepSource.clip != null)
        {
            // Определяем, бежит ли игрок
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            // Выбираем базовый pitch в зависимости от режима (бег/ходьба)
            float basePitch = isRunning ? runningPitch : walkingPitch;
            // Добавляем небольшую вариацию для имитации чередования ног
            float pitchVariation = isLeftFootStep ? stepPitchVariation : -stepPitchVariation;
            footstepSource.pitch = basePitch + pitchVariation;
            // Переключаем ногу для следующего шага
            isLeftFootStep = !isLeftFootStep;
            // Проигрываем звук шага
            footstepSource.PlayOneShot(footstepSource.clip);
        }
    }

    // Метод GetRandomFootstep убран, так как выбор случайного звука более не используется.

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateFootstepSounds(scene.name);
    }

    private void UpdateFootstepSounds(string sceneName)
    {
        // Выбираем один аудиоклип для шагов: 
        // Если для сцены определён звук, используем его (первый элемент), иначе используем дефолтный (первый элемент)
        if (sceneFootstepSounds.ContainsKey(sceneName) && sceneFootstepSounds[sceneName].Count > 0)
        {
            footstepSource.clip = sceneFootstepSounds[sceneName][0];
        }
        else if (defaultFootsteps.Count > 0)
        {
            footstepSource.clip = defaultFootsteps[0];
        }
    }
}
