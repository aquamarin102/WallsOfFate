using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using Quest;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    public bool IsLoading { get; private set; }
    public event Action LoadingStarted;
    public event Action LoadingFinished;

    public GameObject panelGameOver;   // экран проигрыша
    public GameObject panelVictory;


    private InventoryLogicEnd _inventoryLogicEnd;


    [Header("UI-панели")]
    public GameObject loadingScreen;      // ваша существующая панель загрузки
    public TMP_Text loadingText;
    public Image loadingImage;            // для анимации или статичного спрайта

    public GameObject panelEndOfDay;      // новая: экран «подтвердить конец дня»
    public GameObject panelStartOfDay;    // новая: экран «начало дня»
    public float startDayDuration = 2f;   // сколько сек показывать начало дня
    public float inputDelay = 0.05f;      // пауза перед тем, как выводим кнопку Continue

    public Sprite finalSprite;            // то, чем заменить анимацию перед Continue

    private string targetSceneName;
    private bool waitingForInput;
    private UISpriteAnimator spriteAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        _inventoryLogicEnd = FindObjectOfType<InventoryLogicEnd>();
        if (_inventoryLogicEnd == null)
            Debug.LogWarning("LoadingScreenManager: не удалось найти InventoryLogicEnd на сцене!");

        spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();
    }

    private void Update()
    {
        // если уже идёт загрузка или финальный экран открыт — ничего не делаем
        //if (IsLoading || panelGameOver.activeSelf || panelVictory.activeSelf) return;

        if (IsAnyResourceZero())
        {
            ShowGameOver();
        }
    }

    private bool IsAnyResourceZero()
    {
        return GameResources.GameResources.Gold <= 0 ||
                GameResources.GameResources.Food <= 0 ||
                GameResources.GameResources.PeopleSatisfaction <= 0 ||
                GameResources.GameResources.CastleStrength <= 0;
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f;               // ставим игру на паузу
        panelGameOver.SetActive(true);
    }

    // === Кнопка «Конец дня» в вашем UI должна вызывать этот метод ===
    public void ShowEndOfDayPanel()
    {
        // Показываем экран конца дня
        panelEndOfDay.SetActive(true);

        // Находим все дочерние элементы
        Transform[] children = panelEndOfDay.GetComponentsInChildren<Transform>(true);

        // Обновляем значения ресурсов
        UpdateResourceText(children, "Gold", GameResources.GameResources.Gold.ToString());
        UpdateResourceText(children, "Food", GameResources.GameResources.Food.ToString());
        UpdateResourceText(children, "Satisfaction", GameResources.GameResources.PeopleSatisfaction.ToString());
        UpdateResourceText(children, "Staraight", GameResources.GameResources.CastleStrength.ToString());
    }

    private void UpdateResourceText(Transform[] children, string parentObjectName, string value)
    {
        // Ищем первый родительский объект с указанным именем
        Transform parentObject = System.Array.Find(children, child => child.name == parentObjectName);

        if (parentObject != null)
        {
            // Ищем дочерний объект, имя которого начинается с "Amount%"
            Transform amountChild = null;
            foreach (Transform child in parentObject)
            {
                if (child.name.StartsWith("Amount"))
                {
                    amountChild = child;
                    break;
                }
            }

            if (amountChild != null)
            {
                // Получаем компонент TextMeshProUGUI
                TextMeshProUGUI textComponent = amountChild.GetComponent<TextMeshProUGUI>();

                if (textComponent != null)
                {
                    textComponent.text = value;
                }
                else
                {
                    Debug.LogWarning($"Объект Amount% в {parentObjectName} не содержит компонент TextMeshProUGUI");
                }
            }
            else
            {
                Debug.LogWarning($"Не найден дочерний объект Amount% в {parentObjectName}");
            }
        }
        else
        {
            Debug.LogWarning($"Не найден родительский объект с именем {parentObjectName}");
        }
    }

    // === Кнопка «Подтвердить конец дня» на panelEndOfDay ===
    public void OnConfirmEndOfDay()
    {
        PlayerSpawnData.ClearData();

        if (_inventoryLogicEnd != null)
            _inventoryLogicEnd.RefreshPanel();

        QuestCollection.IncreaseCurrentDay();   // день +1

        /* --- ПРОВЕРЯЕМ ПОБЕДУ --- */
        if (QuestCollection.CurrentDayNumber > 3)
        {
            StartCoroutine(ShowVictoryAfterLoad());
            return;                             // прерываем обычный флоу загрузки
        }

        panelEndOfDay.SetActive(false);
        BeginLoadWithStartOfDay("StartDay");
    }

    // === Кнопка «Отмена» на panelEndOfDay ===
    public void OnCancelEndOfDay()
    {
        panelEndOfDay.SetActive(false);
    }

    // Общий запуск загрузки (с показом start-day после)
    public void BeginLoadWithStartOfDay(string sceneName)
    {
        targetSceneName = sceneName;
        ShowLoadingUI();

        // уведомляем
        IsLoading = true;
        LoadingStarted?.Invoke();

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName, showStartDay: true));
    }

    // === Ваш существующий метод LoadScene, но без end-day ===
    public void LoadScene(string sceneName)
    {
        // если вам нужен просто экран загрузки без start-day
        targetSceneName = sceneName;
        ShowLoadingUI();

        IsLoading = true;
        LoadingStarted?.Invoke();

        AudioManager.Instance.ActivateLoadingSnapshot();
        AudioManager.Instance.PlayLoadingMusic();

        StartCoroutine(LoadSceneAsync(sceneName, showStartDay: false));
    }

    private IEnumerator ShowVictoryAfterLoad()
    {
        // показываем обычный лоадинг, чтобы был единый флоу
        BeginLoadWithStartOfDay("StartDay");

        // ждём пока он отработает
        while (IsLoading) yield return null;

        Time.timeScale = 0f;
        panelVictory.SetActive(true);
    }

    private void ShowLoadingUI()
    {
        Time.timeScale = 1f;
        loadingScreen.SetActive(true);
        panelEndOfDay.SetActive(false);
        // НЕ скрываем panelStartOfDay здесь!
        loadingText.text = "Загрузка...";
        if (spriteAnimator != null) spriteAnimator.enabled = true;
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool showStartDay)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(inputDelay);

                if (spriteAnimator != null) spriteAnimator.enabled = false;
                if (finalSprite != null) loadingImage.sprite = finalSprite;
                loadingText.text = "Продолжить";
                waitingForInput = true;

                // Здесь ждём до полного скрытия panelStartOfDay
                yield return StartCoroutine(WaitForUserInput(showStartDay));
                yield break;
            }
            yield return null;
        }
    }


    private IEnumerator WaitForUserInput(bool showStartDay)
    {
        // Ждём нажатия
        while (!Input.anyKeyDown) yield return null;
        waitingForInput = false;

        // Подготовка звука и анимации
        AudioManager.Instance.ReloadVolumeSettings();
        AudioManager.Instance.ChangeMusicForScene(targetSceneName);
        if (spriteAnimator != null) spriteAnimator.enabled = true;

        // Показ экрана начала дня как часть загрузки
        if (showStartDay && panelStartOfDay != null)
        {
            panelStartOfDay.SetActive(true);
            // Здесь loadingScreen остаётся активным, чтобы считалось «время загрузки»
            yield return new WaitForSeconds(startDayDuration);
            panelStartOfDay.SetActive(false);
        }

        // Только после этого закрываем сам loadingScreen
        loadingScreen.SetActive(false);

        // И лишь теперь — считаем загрузку завершённой
        IsLoading = false;
        LoadingFinished?.Invoke();
    }



    private IEnumerator FadeText()
    {
        float freq = 2f;
        while (waitingForInput)
        {
            float a = (Mathf.Sin(Time.time * freq) + 1f) / 2f;
            var c = loadingText.color; c.a = a;
            loadingText.color = c;
            yield return null;
        }
        var f = loadingText.color; f.a = 1f;
        loadingText.color = f;
    }

    private IEnumerator ShowStartDay()
    {
        panelStartOfDay.SetActive(true);
        yield return new WaitForSeconds(startDayDuration);
        panelStartOfDay.SetActive(false);
    }
}
