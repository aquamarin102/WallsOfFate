using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Zenject;

public sealed class SaveLoadManager : MonoBehaviour
{
    private static Transform _playerTransform;

    private ISaveLoader[] saveLoaders;
    private ISaveLoader[] requeredSaveLoaders;

    [Inject]
    private void Construct(PlayerMoveController controller)
    {
        _playerTransform = controller.transform;
    }

    private void Awake()
    {
        saveLoaders = new ISaveLoader[]
        {
            new PlayerSaveLoader(_playerTransform),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };
        requeredSaveLoaders = new ISaveLoader[]
        {
            //new PlayerSaveLoader(_playerTransform),
            new CollectionSaveLoader(AssembledPickups.GetAllPickups()),
        };
        LoadRequiredData();
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

        foreach (var saveLoader in this.requeredSaveLoaders)
        {
            saveLoader.LoadDefaulData();
        }
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
        Repository.ClearSaveData(); 
    }
}