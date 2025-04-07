using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance; // Синглтон для доступа из любой сцены

    public GameObject loadingScreen; // Панель с UI загрузочного экрана (например, Canvas с слайдером)
    public Slider progressBar;       // UI-элемент для отображения прогресса

    private void Awake()
    {
        // Если экземпляра еще нет, устанавливаем его и не уничтожаем при загрузке новой сцены
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
    /// Публичный метод для загрузки новой сцены с отображением загрузочного экрана
    /// </summary>
    /// <param name="sceneName">Имя сцены, которую нужно загрузить</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Асинхронная загрузка сцены с обновлением UI загрузки
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Активируем загрузочный экран
        loadingScreen.SetActive(true);

        // Начинаем асинхронную загрузку сцены
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Если нужно, можно отключить автоматическую активацию сцены:
        // operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // AsyncOperation.progress возвращает значение от 0 до 0.9,
            // поэтому нормализуем его до диапазона 0-1
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Если вы используете allowSceneActivation = false,
            // можно добавить проверку, когда загрузка почти завершена
            // if (operation.progress >= 0.9f)
            // {
            //     // Здесь можно, например, ожидать нажатия кнопки "Начать игру" и затем:
            //     // operation.allowSceneActivation = true;
            // }

            yield return null;
        }

        // Деактивируем загрузочный экран после завершения загрузки
        loadingScreen.SetActive(false);
    }
}
