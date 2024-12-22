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

public class PlayerSaveLoader : ISaveLoader
{
    private Transform playerTransform;

    public PlayerSaveLoader(Transform transform)
    {
        playerTransform = transform;
    }


    public void LoadData()
    {
        if (Repository.TryGetData(out Vector3DTO savedPosition))
        {
            playerTransform.position = savedPosition.ToVector3();
            //Debug.Log("Player position loaded: " + savedPosition);
        }
        else
        {
            //Debug.LogWarning("No saved position found. Loading default position.");
        }
    }

    public void SaveData()
    {
        Vector3DTO poosition = new Vector3DTO(playerTransform.position);
        Repository.SetData(poosition);
        //Debug.Log("Player position saved: " + playerTransform.position);
    }
}
