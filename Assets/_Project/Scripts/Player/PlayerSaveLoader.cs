using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vector3DTO
{
    public float x;
    public float y;
    public float z;

    public Vector3DTO(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3DTO position;
    public Vector3DTO rotation;
    public string sceneName;

    public PlayerSaveData(Vector3 position, Quaternion rotation, string sceneName)
    {
        this.position = new Vector3DTO(position);
        this.rotation = new Vector3DTO(rotation.eulerAngles);
        this.sceneName = sceneName;
    }
}

public class PlayerSaveLoader : ISaveLoader
{
    private Transform playerTransform;

    public PlayerSaveLoader(Transform transform)
    {
        playerTransform = transform;
    }

    public void LoadData()
    {
        if (Repository.TryGetData(out PlayerSaveData savedData))
        {
            PlayerSpawnData.SpawnPosition = savedData.position.ToVector3();
            PlayerSpawnData.SpawnRotation = Quaternion.Euler(savedData.rotation.ToVector3());

            if (!string.IsNullOrEmpty(savedData.sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(savedData.sceneName);
                Time.timeScale = 1;
            }
        }
        else
        {
            //Debug.LogWarning("No saved data found. Loading default scene and position.");
        }
    }

    public void LoadDefaulData()
    {
        throw new NotImplementedException();
    }

    public void SaveData()
    {
        PlayerSaveData saveData = new PlayerSaveData(
            playerTransform.position,
            playerTransform.rotation,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );

        Repository.SetData(saveData);
        //Debug.Log("Player data saved: " + saveData.sceneName);
    }
}
