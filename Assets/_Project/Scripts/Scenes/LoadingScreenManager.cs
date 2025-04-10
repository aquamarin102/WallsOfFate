using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // Синглтон

    [Header("UI Элементы загрузочного экрана")]
    public GameObject loadingScreen;
    public Image loadingImage;
    public TMP_Text loadingText;

    [Header("Настройки финального вида")]
    public Sprite finalSprite;

    // Время задержки перед тем, как игрок сможет закрыть экран загрузки (в секундах)
    public float inputDelay = 0.05f;

    private string targetSceneName;
    private bool waitingForInput = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        // Восстанавливаем нормальное течение времени
        Time.timeScale = 1f;

        targetSceneName = sceneName;

        loadingScreen.SetActive(true);
        loadingText.text = "Загрузка...";

        // Переключаем микшер на режим загрузки (замьютить звуки сцены)
        AudioManager.Instance.ActivateLoadingSnapshot();

        // Запускаем загрузочную музыку (если нужно)
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                // Отключаем компонент анимации, если он есть.
                var spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
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
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForUserInput()
    {
        // Запускаем мигание текста
        StartCoroutine(BlinkText());

        // Добавляем задержку перед активацией возможности закрытия
        yield return new WaitForSeconds(inputDelay);

        // После задержки начинаем ожидать нажатия любой клавиши
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        waitingForInput = false;

        // Восстанавливаем звуки согласно пользовательским настройкам:
        AudioManager.Instance.ReloadVolumeSettings();
        // Переключаем музыку на музыку целевой сцены
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);
        // Скрываем загрузочный экран
        loadingScreen.SetActive(false);
    }

    private IEnumerator BlinkText()
    {
        while (waitingForInput)
        {
            loadingText.enabled = !loadingText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        loadingText.enabled = true;
    }
}
