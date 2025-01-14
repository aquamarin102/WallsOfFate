using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    private uint MaxHealth;       // Максимальные Жизни
    private uint Health;          // Жизни
    private uint Damage;          // Урон
    private float Speed;          // Скорость
    private uint HealingAmount;   // Количество лечения

    public Player(uint health, uint maxhealth, uint damage, float speed, uint healingAmount)
    {
        MaxHealth = maxhealth;
        Health = health;
        Damage = damage;
        Speed = speed;
        HealingAmount = healingAmount;
    }

    public uint GetHealth()
    {
        return this.Health;
    }

    public uint GetDamage()
    {
        return this.Damage;
    }

    public float GetSpeed()
    {
        return this.Speed;
    }

    public void TakeDamage(uint damage)
    {
        this.Damage += damage;
    }

    public void TakeHeal()
    {
        this.Health += HealingAmount;
        if (this.Health > MaxHealth)
        {
            this.Health = MaxHealth;
        }
    }

    public void TakeSpeedboost(float speed)
    {
        this.Speed *= speed;
    }
}
