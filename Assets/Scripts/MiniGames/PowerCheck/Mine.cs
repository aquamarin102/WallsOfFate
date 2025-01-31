using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine
{
    private uint number;           
    private float cooldown;         
    private GameObject mineGameObject;  
    private bool isFirst = true;    

    public Mine(uint number, float cooldown, GameObject mine)
    {
        this.number = number;
        this.cooldown = cooldown;
        this.mineGameObject = mine;
    }

    // Публичные свойства для доступа к полям
    public uint Number
    {
        get { return this.number; }
    }

    public float Cooldown
    {
        get { return this.cooldown; }
    }

    public GameObject MineGameObject
    {
        get { return this.mineGameObject; }
    }

    public bool IsFirstSpawn
    {
        get { return this.isFirst; }
        set { this.isFirst = value; }
    }

    public void SetActive(bool isActive)
    {
        if (mineGameObject != null)
        {
            mineGameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Mine GameObject is null. Cannot change active state.");
        }
    }
}
