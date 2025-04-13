using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private AudioSource footstepSource;

    [Header("Speed Normalization Settings")]
    [Tooltip("Максимальная скорость игрока для нормализации параметра Speed (например, скорость бега)")]
    [SerializeField] private float maxSpeed = 4.5f;
    [Tooltip("Время затухания для интерполяции параметра Speed")]
    [SerializeField] private float speedDampTime = 0.1f;

    [Header("Footstep Timing Settings")]
    [Tooltip("Базовый интервал между шагами при ходьбе (сек.)")]
    [SerializeField] private float baseFootstepInterval = 0.5f;
    [Tooltip("Интервал между шагами при беге (сек.)")]
    [SerializeField] private float runFootstepInterval = 0.3f;


    private float footstepTimer = 0f;

    private MonoBehaviour playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayerAnimator: Не найден компонент Animator!");
        }
        lastPosition = transform.position;


        playerController = GetComponent<PlayerMoveController>(); // Предполагается, что такой скрипт существует
        if (playerController == null)
        {
            Debug.LogWarning("PlayerAnimator: Не найден компонент управления персонажем!");
        }
        
    }

    private void FixedUpdate()
    {
        // Вычисляем скорость как длину перемещения за кадр
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        // Нормализуем скорость (значение от 0 до 1)
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        // Проверяем, держится ли игрок за ящик
        bool isPushing = (transform.parent != null && transform.parent.CompareTag("Box"));

        if (!isPushing)
        {
            // Обычное движение: стандартная анимация передвижения
            animator.speed = 1f; // нормальная скорость анимации
            animator.SetFloat("Speed", normalizedSpeed, speedDampTime, Time.deltaTime);
            animator.SetBool("IsPushing", false);
            animator.SetFloat("PushSpeed", 0f);
        }
        else
        {
            // Режим толкания ящика:
            animator.SetBool("IsPushing", true);
            animator.SetFloat("Speed", 0f); // отключаем стандартную анимацию передвижения

            if (normalizedSpeed > 0.1f)
            {
                // Если игрок двигается, запускаем анимацию толкания с постоянной скоростью
                animator.SetFloat("PushSpeed", 1f);
                animator.speed = 2f;
            }
            else
            {
                // Если игрок не двигается, анимация толкания не проигрывается
                animator.SetFloat("PushSpeed", 0f);
                animator.speed = 0f;
            }
        }

        // Интерполируем интервал между шагами
        float currentFootstepInterval = Mathf.Lerp(baseFootstepInterval, runFootstepInterval, normalizedSpeed);

        lastPosition = transform.position;
    }

    // Метод для запуска анимации подбора с пола
    public void PlayPickupFloor()
    {
        // Запускаем анимацию подбора; управление движением не блокируется.
        animator.SetTrigger("PickupFloor");
    }

    // Метод для запуска анимации подбора предмета на уровне тела
    public void PlayPickupBody()
    {
        animator.SetTrigger("PickupBody");
    }

    // Метод для запуска анимации открытия сундука
    public void PlayOpenChest()
    {
        animator.SetTrigger("OpenChest");
    }
}
