using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    /* ---------- новое ---------- */
    public bool IsLoading { get; private set; }
    public event Action LoadingStarted;
    public event Action LoadingFinished;
    /* --------------------------- */

    [Header("UI")]
    public GameObject loadingScreen;
    public Image loadingImage;
    public TMP_Text loadingText;

    private UISpriteAnimator spriteAnimator;

    [Header("Настройки финального вида")]
    public Sprite finalSprite;

    public float inputDelay = 0.05f;

    private string targetSceneName;
    private bool waitingForInput;

    /* ---------- SINGLETON ---------- */
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
    }

    /* ---------- ЗАПУСК ЗАГРУЗКИ ---------- */
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        targetSceneName = sceneName;
        loadingScreen.SetActive(true);
        loadingText.text = "Загрузка...";

        /* --- уведомляем остальной код --- */
        IsLoading = true;
        LoadingStarted?.Invoke();
        /* -------------------------------- */

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /* ---------- КОРУТИНА ЗАГРУЗКИ ---------- */
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
           
                yield return new WaitForSeconds(inputDelay);

                
                if (spriteAnimator != null)
                {
                    spriteAnimator.enabled = false;
                }
                // Подставляем финальный спрайт, если он задан.
                if (finalSprite != null)
                {
                    loadingImage.sprite = finalSprite;
                }

                loadingText.text = "Продолжить";
                waitingForInput = true;

                StartCoroutine(WaitForUserInput());
                StartCoroutine(FadeText());
                break;
            }
            yield return null;
        }
    }

    /* ---------- ОЖИДАЕМ КЛИК ПОЛЬЗОВАТЕЛЯ ---------- */
    private IEnumerator WaitForUserInput()
    {
        while (!Input.anyKeyDown) yield return null;

        waitingForInput = false;

        AudioManager.Instance.ReloadVolumeSettings();
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);

        spriteAnimator.enabled = true;

        loadingScreen.SetActive(false);

        /* --- загрузка закрылась ------------- */
        IsLoading = false;
        LoadingFinished?.Invoke();
        /* ------------------------------------ */
    }

    /* ---------- ПУЛЬСИРУЮЩИЙ ТЕКСТ ---------- */
    private IEnumerator FadeText()
    {
        float frequency = 2f;

        while (waitingForInput)
        {
            float a = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
            Color c = loadingText.color; c.a = a;
            loadingText.color = c;
            yield return null;
        }

        Color fin = loadingText.color; fin.a = 1f;
        loadingText.color = fin;
    }
}
