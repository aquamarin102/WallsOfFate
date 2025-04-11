using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // Синглтон

    [Header("UI Элементы загрузочного экрана")]
    public GameObject loadingScreen;
    public TMP_Text loadingText;

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
                // Добавляем задержку перед активацией возможности закрытия
                yield return new WaitForSeconds(inputDelay);
                loadingText.text = "Продолжить";
                waitingForInput = true;

                // Запуск корутин: ожидание ввода пользователя и плавное изменение альфа-канала текста
                StartCoroutine(WaitForUserInput());
                StartCoroutine(FadeText());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForUserInput()
    {
        
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

    // Новый метод для плавного изменения альфа-канала текста
    private IEnumerator FadeText()
    {
        // Задаем частоту изменения прозрачности (чем больше значение, тем быстрее меняется альфа)
        float frequency = 2f;

        while (waitingForInput)
        {
            // Вычисляем новое значение альфа с помощью синусоиды,
            // которое будет плавно изменяться от 0 до 1
            float alpha = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
            Color color = loadingText.color;
            color.a = alpha;
            loadingText.color = color;
            yield return null;
        }
        // После выхода из цикла возвращаем текст в полностью непрозрачное состояние
        Color finalColor = loadingText.color;
        finalColor.a = 1f;
        loadingText.color = finalColor;
    }
}
