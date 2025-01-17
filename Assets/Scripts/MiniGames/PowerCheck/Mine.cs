using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine 
{
    protected uint Number;
    protected float Ñooldown;
    protected GameObject MineGameObject;

    public Mine(uint number, float ñooldown, GameObject mine)
    {
        Number = number;
        Ñooldown = ñooldown;;
        MineGameObject = mine;
    }

    public uint GetNumber()
    {
        return this.Number;
    }

    public float GetCooldown()
    {
        return this.Ñooldown;
    }

    public GameObject GetMine()
    {
        return this.MineGameObject;
    }

    public void SetActive(bool isActive)
    {
        if (MineGameObject != null)
        {
            MineGameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Mine GameObject is null. Cannot change active state.");
        }
    }
}
