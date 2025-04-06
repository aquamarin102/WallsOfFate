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

    [Header("Footstep Settings")]
    [SerializeField] private List<AudioClip> defaultFootsteps; // Базовые звуки шагов

    [Header("Footstep Timing Settings")]
    [Tooltip("Базовый интервал между шагами при ходьбе (сек.)")]
    [SerializeField] private float baseFootstepInterval = 0.5f;
    [Tooltip("Интервал между шагами при беге (сек.)")]
    [SerializeField] private float runFootstepInterval = 0.3f;

    [Header("Pitch Settings")]
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxPitch = 1.3f;

    // Параметры для анимации толкания ящика:
    // В Animator персонажа должны быть параметры: 
    // - IsPushing (bool)
    // - PushSpeed (float)
    private float footstepTimer = 0f;
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayerAnimator: Не найден компонент Animator!");
        }
        lastPosition = transform.position;

        footstepSource = GetComponent<AudioSource>();
        if (footstepSource == null)
        {
            Debug.LogError("PlayerAnimator: Не найден компонент AudioSource!");
        }

        // Инициализация звуков для разных сцен
        sceneFootstepSounds.Add("MainRoom", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Wood") });
        sceneFootstepSounds.Add("Forge", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Grass") });
        sceneFootstepSounds.Add("Storage", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Stone") });

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateFootstepSounds(SceneManager.GetActiveScene().name);
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

        // Обновляем pitch звука шагов в зависимости от скорости (чем быстрее — тем выше)
        if (footstepSource != null)
        {
            footstepSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        }

        // Интерполируем интервал между шагами
        float currentFootstepInterval = Mathf.Lerp(baseFootstepInterval, runFootstepInterval, normalizedSpeed);

        // Обработка звука шагов
        footstepTimer += Time.deltaTime;
        if (normalizedSpeed > 0.1f && footstepTimer >= currentFootstepInterval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        if (footstepSource != null && defaultFootsteps != null && defaultFootsteps.Count > 0)
        {
            AudioClip clip = GetRandomFootstep();
            if (clip != null)
            {
                footstepSource.PlayOneShot(clip);
            }
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
        if (sceneFootstepSounds.ContainsKey(sceneName))
        {
            footstepSource.clip = sceneFootstepSounds[sceneName][0];
        }
        else if (defaultFootsteps.Count > 0)
        {
            footstepSource.clip = defaultFootsteps[0];
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
