using GameResources;
using Quest;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using UnityEngine.UI;

public sealed class SaveLoadManager : MonoBehaviour
{
    private static Transform _playerTransform;

    // Полный набор загрузчиков (используется при загрузке сохранённой игры)
    private ISaveLoader[] saveLoaders;
    // Набор загрузчиков, необходимых для инициализации без загрузки позиции игрока.
    private ISaveLoader[] requiredSaveLoaders;

    // Флаг для определения начала новой игры
    public bool StartNewGame = false;

    // Задаем стартовую точку через инспектор
    [SerializeField] private Transform spawnPoint;

    [Inject]
    private void Construct(PlayerMoveController controller)
    {
        _playerTransform = controller.transform;
    }

    private void Awake()
    {
        // Полный набор загрузчиков – используется, например, при продолжении игры.
        saveLoaders = new ISaveLoader[]
        {
            new QuestSaveLoader(),
            new ResourceSaveLoader(),
            new PlayerSaveLoader(_playerTransform),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };

        // Набор загрузчиков, которые НЕ должны перезаписывать позицию игрока.
        // Убираем PlayerSaveLoader, чтобы позиция игрока оставалась такой, какой она определена на сцене.
        requiredSaveLoaders = new ISaveLoader[]
        {
            new ResourceSaveLoader(),
            // new PlayerSaveLoader(_playerTransform), // не загружаем позицию игрока
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
            new InteractableItemSaveLoader(),
            new QuestSaveLoader(),
        };
    }

    private void Start()
    {
        // Ищем кнопку с тегом или по ссылке (упростим до FindObject)
        Button loadBtn = GameObject.Find("Button_LoadGame")
                                   ?.GetComponent<Button>();
        if (loadBtn != null)
            loadBtn.interactable = CanLoad();   // активна, только если есть сейв
    }

    /// <summary>
    /// Полная загрузка сохранённого прогресса (в том числе позиции игрока).
    /// Вызывается, например, при выборе "Продолжить".
    /// </summary>
    public void LoadGame()
    {
        Repository.LoadState();
        foreach (var saveLoader in saveLoaders)
        {
            saveLoader.LoadData();
        }
    }

    /// <summary>
    /// Загрузка обязательных данных, без загрузки позиции игрока.
    /// Если для какого-либо загрузчика нет сохранённых данных, вызывается LoadDefaultData().
    /// </summary>
    public void LoadRequiredData()
    {
        Repository.LoadState();
        foreach (var saveLoader in requiredSaveLoaders)
        {
            if (!saveLoader.LoadData() || QuestCollection.CurrentDayNumber > 2)
            {
                saveLoader.LoadDefaultData();
            }
        }
    }

    /// <summary>
    /// Сохранение обязательных данных (например, при смене сцен).
    /// </summary>
    public void SaveRequiredData()
    {
        if (!StartNewGame)
        {
            foreach (var saveLoader in requiredSaveLoaders)
            {
                if (saveLoader != null)
                    saveLoader.SaveData();
            }
            Repository.SaveState();
            StartNewGame = false;
        }
    }

    /// <summary>
    /// Полное сохранение игрового прогресса (в том числе позиции игрока).
    /// Вызывается, например, при сохранении перед выходом или при переходе в меню "Продолжить".
    /// </summary>
    public void SaveGame()
    {
        foreach (var saveLoader in saveLoaders)
        {
            saveLoader.SaveData();
        }
        Repository.SetUserProgress(true);
        Repository.SaveState();
    }

    /// <summary>
    /// Проверяет, есть ли сохранённый игровой прогресс.
    /// </summary>
    public bool CanLoad()
    {
        Repository.LoadState();
        return Repository.HasAnyData();
    }

    /// <summary>
    /// Очищает сохранённые данные и устанавливает флаг новой игры.
    /// </summary>
    public void ClearSavs()
    {
        QuestCollection.ClearQuests();
        AssembledPickups.Clear();
        Repository.ClearSaveData();
        PlayerSpawnData.ClearData();
        StartNewGame = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SaveRequiredData();
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveRequiredData();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Если начинается новая игра, позиция игрока сбрасывается в spawnPoint.
        if (StartNewGame)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                // Если нужно сбросить и вращение, можно добавить:
                // player.transform.rotation = spawnPoint.rotation;
            }
            // Затем загружаем обязательные данные (без позиции игрока)
            LoadRequiredData();
            StartNewGame = false; // сбросить флаг, чтобы в дальнейшем обычная загрузка продолжала работать
        }
        else
        {
            LoadRequiredData();
        }
    }

    /*-------------------------------------------------*/
    /*      КНОПКА «ЗАГРУЗИТЬ ИГРУ»                    */
    /*-------------------------------------------------*/
    public void OnLoadGameButton()
    {
        // safety-check: ничего не делать, если сохранений нет
        if (!CanLoad()) return;

        StartNewGame = false;                  // чтобы не перезаписать позицию
        LoadGame();                             // подгрузили данные
    }

}
