using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // Синглтон для доступа из любых сцен

    [Header("UI Элементы загрузочного экрана")]
    public GameObject loadingScreen;   // Контейнер (например, панель или Canvas) загрузочного экрана
    public Image loadingImage;         // Изображение, которое отображается во время загрузки
    public TMP_Text loadingText;       // Текстовый элемент (TextMeshPro) для вывода сообщений

    [Header("Настройки финального вида")]
    [Tooltip("Спрайт, который подставляется в loadingImage после загрузки новой сцены.")]
    public Sprite finalSprite;         // Финальный спрайт

    // Флаг, указывающий, что окно ждёт нажатия клавиши
    private bool waitingForUserInput = false;

    private void Awake()
    {
        // Реализуем синглтон, чтобы объект не уничтожался между сценами
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

    /// <summary>
    /// Запускает асинхронную загрузку указанной сцены.
    /// Загрузочный экран появляется сразу, и после загрузки новой сцены окно остаётся видимым,
    /// пока пользователь не нажмёт любую клавишу.
    /// </summary>
    /// <param name="sceneName">Имя сцены для загрузки</param>
    public void LoadScene(string sceneName)
    {
        // Отображаем загрузочный экран с изначальным сообщением "Загрузка..."
        loadingScreen.SetActive(true);
        loadingText.text = "Загрузка...";

        // Подписываемся на событие завершения загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Запускаем асинхронную загрузку новой сцены.
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Асинхронно загружает указанную сцену.
    /// allowSceneActivation оставляем true, чтобы новая сцена активировалась сразу после загрузки.
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // Можно добавить отслеживание прогресса, если требуется.
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Метод, вызываемый системой после загрузки новой сцены.
    /// Здесь происходит смена финальных элементов загрузочного экрана,
    /// и начинается ожидание нажатия любой клавиши для закрытия окна.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Отписываемся, чтобы избежать множественных вызовов
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Если на объекте loadingImage есть компонент UISpriteAnimator, отключаем его.
        UISpriteAnimator spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
        if (spriteAnimator != null)
        {
            spriteAnimator.enabled = false;
        }

        // Подставляем финальный спрайт, если он задан.
        if (finalSprite != null)
        {
            loadingImage.sprite = finalSprite;
        }

        // Меняем текст на финальное сообщение.
        loadingText.text = "Продолжить";
        waitingForUserInput = true;

        // Запускаем корутину для ожидания пользовательского ввода и, опционально, эффекта мигания текста.
        StartCoroutine(WaitForUserInput());
    }

    /// <summary>
    /// Корутина, которая ждет нажатия любой клавиши.
    /// Пока не нажата любая клавиша, можно добавить дополнительные эффекты (например, мигание текста).
    /// </summary>
    private IEnumerator WaitForUserInput()
    {
        // Если хотите добавить мигание текста, можно запустить отдельную корутину.
        StartCoroutine(BlinkText());

        // Ждем, пока пользователь не нажмет любую клавишу.
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        waitingForUserInput = false;
        // После нажатия клавиши скрываем загрузочный экран.
        loadingScreen.SetActive(false);
    }

    /// <summary>
    /// Пример корутины эффекта мигания текста.
    /// Текст будет переключаться каждые 0.5 секунды, пока ждем ввода.
    /// </summary>
    private IEnumerator BlinkText()
    {
        while (waitingForUserInput)
        {
            loadingText.enabled = !loadingText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        // В конце обязательно включаем текст
        loadingText.enabled = true;
    }
}
