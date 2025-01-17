using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour 
{


    // ============================
    // Настройки игровых параметров игрока
    // ============================
    [Header("Player Settings")]
    [SerializeField] public string Name;           // Имя
    [SerializeField] private uint MaxHealth;       // Максимальное здоровье
    [SerializeField] private uint Health;          // Здоровье с которым он заспавниться
    [SerializeField] private uint Damage;          // Урон
    [SerializeField] private float Speed;          // Скорость
    [SerializeField] private uint HealingAmount;   // Количество лечения


    //public Player(uint maxhealth,  uint health,  uint damage, float speed, uint healingAmount, string name)
    //{
    //    Name = name;
    //    MaxHealth = maxhealth;
    //    Health = health;
    //    Damage = damage;
    //    Speed = speed;
    //    HealingAmount = healingAmount;
    //}

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
        Debug.Log(this.Name + " получил домага");
        this.Damage += damage;
    }

    public void TakeHeal()
    {
        Debug.Log(this.Name + " отхилился");
        this.Health += HealingAmount;
        if (this.Health > MaxHealth)
        {
            this.Health = MaxHealth;
        }
    }

    public void TakeSpeedboost(float speed)
    {
        Debug.Log(this.Name + " получил скоростной баф в " + speed);
        this.Speed *= speed;
    }
}
