using System;
using UnityEngine;

public class MiniGamePlayer : MonoBehaviour
{
    // ============================
    // Настройки игровых параметров игрока
    // ============================
    [SerializeField] private string playerName;  // Имя игрока
    [SerializeField] private uint maxHealth;     // Максимальное здоровье
    [SerializeField] private uint health;        // Текущее здоровье
    [SerializeField] private uint damage;        // Урон
    [SerializeField] private float speed;        // Скорость
    [SerializeField] private uint healingAmount; // Количество лечения

    public string Name
    {
        get => playerName;
        set => playerName = value;
    }

    public uint MaxHealth => maxHealth;
    public uint Health => health;
    public uint Damage => damage;

    public float Speed
    {
        get => speed;
        set
        {
            //Debug.Log($"Setting speed: {value}, current speed: {speed}");
            if (Math.Abs(speed - value) > 0.01f)
            {
                speed = value;
                OnSpeedChanged?.Invoke(speed); // Вызываем событие при изменении скорости
            }
        }
    }

    public uint HealingAmount => healingAmount;

    public event Action<float> OnSpeedChanged;

    // Unity-метод для начальной инициализации
    private void Start()
    {
        //Debug.Log($"{Name} создан с {health} здоровья и скоростью {speed}");
    }

    public void Initialize(string playerName, uint maxHp, uint startHealth, uint dmg, float initialSpeed, uint healAmount)
    {
        Name = playerName;
        maxHealth = maxHp;
        health = startHealth;
        damage = dmg;
        speed = initialSpeed;
        healingAmount = healAmount;
    }

    public void TakeDamage(uint damage)
    {
        health = health >= damage ? health - damage : 0;
        Debug.Log($"{Name} получил урон. Здоровье: {health}");
    }

    public void TakeHeal()
    {
        health += healingAmount;
        if (health > maxHealth) health = maxHealth;
        //Debug.Log($"{Name} отхилился. Здоровье: {health}");
    }

    public void TakeSpeedboost(float speedMultiplier)
    {
        //Debug.Log($"{Name} получил скоростной баф {speedMultiplier}");
        Speed = (float)speedMultiplier; // Изменяем скорость, вызывая событие
        //Debug.Log($"{Name} новая скорость {Speed}");
    }
}
