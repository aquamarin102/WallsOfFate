using Quest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public sealed class SaveLoadManager : MonoBehaviour
{
    private static Transform _playerTransform;

    private ISaveLoader[] saveLoaders;
    private ISaveLoader[] requiredSaveLoaders;

    [Inject]
    private void Construct(PlayerMoveController controller)
    {
        _playerTransform = controller.transform;
    }

    private void Awake()
    {
        saveLoaders = new ISaveLoader[]
        {
            new QuestSaveLoader(),
            new PlayerSaveLoader(_playerTransform),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };
        requiredSaveLoaders = new ISaveLoader[]
        {
            //new PlayerSaveLoader(_playerTransform),
            new QuestSaveLoader(),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };
        //LoadGame();
        //LoadRequiredData();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }
    }

    public void LoadGame()
    {
        Repository.LoadState();

        foreach (var saveLoader in this.saveLoaders)
        {
            saveLoader.LoadData();
        }
    }
    public void LoadRequiredData()
    {
        Repository.LoadState();
        foreach (var saveLoader in this.requiredSaveLoaders)
        {
            if (!saveLoader.LoadData())
            {
                saveLoader.LoadDefaultData();
            }
        }
    }

    public void SaveRequiredData() // правильное именование
    {
        foreach (var saveLoader in this.requiredSaveLoaders) // исправление опечатки
        {
            if (saveLoader != null)
                saveLoader.SaveData();
        }
        Repository.SaveState();
    }


    public void SaveGame()
    {
        foreach (var saveLoader in this.saveLoaders)
        {
            saveLoader.SaveData();
        }

        Repository.SaveState();
    }

    public bool CanLoad()
    {
        Repository.LoadState();

        return Repository.HasAnyData();
    }

    public void ClearSavs()
    {
        QuestCollection.ClearQuests();
        AssembledPickups.Clear();
        Repository.ClearSaveData();
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
        LoadRequiredData();
    }
}