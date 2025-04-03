using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private AudioSource footstepSource;

    [SerializeField] private List<AudioClip> defaultFootsteps; // Базовые шаги
    private Dictionary<string, List<AudioClip>> sceneFootstepSounds = new Dictionary<string, List<AudioClip>>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        footstepSource = GetComponent<AudioSource>();
        lastPosition = transform.position;

        // Добавляем звуки для разных сцен
        
        sceneFootstepSounds.Add("MainRoom", new List<AudioClip>() {Resources.Load<AudioClip>("Footsteps/Wood")});
        sceneFootstepSounds.Add("Forge", new List<AudioClip>() {Resources.Load<AudioClip>("Footsteps/Grass")});
        sceneFootstepSounds.Add("Storage", new List<AudioClip>() { Resources.Load<AudioClip>("Footsteps/Stone") });
        // Подписка на смену сцены
        SceneManager.sceneLoaded += OnSceneLoaded;

        UpdateFootstepSounds(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FixedUpdate()
    {
        if (transform.position != lastPosition)
        {
            animator.SetBool("IsWalk", true);

            if (!footstepSource.isPlaying)
            {
                PlayFootstep();
            }
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        if (footstepSource != null && footstepSource.clip != null)
        {
            footstepSource.clip = GetRandomFootstep();
            footstepSource.Play();
        }
    }

    private AudioClip GetRandomFootstep()
    {
        List<AudioClip> currentFootsteps = sceneFootstepSounds.ContainsKey(SceneManager.GetActiveScene().name)
            ? sceneFootstepSounds[SceneManager.GetActiveScene().name]
            : defaultFootsteps;

        return currentFootsteps[Random.Range(0, currentFootsteps.Count)];
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateFootstepSounds(scene.name);
    }

    private void UpdateFootstepSounds(string sceneName)
    {
        if (sceneFootstepSounds.ContainsKey(sceneName))
        {
            footstepSource.clip = sceneFootstepSounds[sceneName][0]; // Устанавливаем первый звук для теста
        }
        else if (defaultFootsteps.Count > 0)
        {
            footstepSource.clip = defaultFootsteps[0];
        }
    }
}
