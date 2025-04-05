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

    private void Update()
    {
        // Вычисляем скорость как длину перемещения за кадр
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        // Нормализуем скорость (значение от 0 до 1)
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        // Обновляем параметр "Speed" в Animator с затуханием
        animator.SetFloat("Speed", normalizedSpeed, speedDampTime, Time.deltaTime);

        // Регулируем pitch звука шагов в зависимости от скорости (чем быстрее — тем выше)
        if (footstepSource != null)
        {
            footstepSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        }

        // Интерполируем текущий интервал между шагами:
        // При низкой скорости используем baseFootstepInterval, при максимальной — runFootstepInterval
        float currentFootstepInterval = Mathf.Lerp(baseFootstepInterval, runFootstepInterval, normalizedSpeed);

        // Обработка звука шагов: увеличиваем таймер и, если игрок движется, воспроизводим шаг по истечении интервала
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
            // Можно установить текущий клип для теста или обновлять список звуков
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
