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
    [SerializeField] private float speedModifier;// Скорость
    [SerializeField] private uint healingAmount; // Количество лечения
    /*[SerializeField] private*/public bool isDead = false;// Количество лечения

    private bool underDebufff;

    public string Name
    {
        get => playerName;
        set => playerName = value;
    }

    public uint MaxHealth => maxHealth;
    public uint Health
    {
        get => health;
        set => health = value;
    }
    public uint Damage => damage;

    public float SpeedModifier
    {
        get => speedModifier;
        set
        {
            //Debug.Log($"Setting speed: {value}, current speed: {speed}");
            speedModifier = value;
            OnSpeedChanged?.Invoke(speedModifier, underDebufff); // Вызываем событие при изменении скорости
        }
    }

    public float Speed
    {
        get => speed;
    }

    public uint HealingAmount => healingAmount;

    public event Action<float, bool> OnSpeedChanged;

    private void OnEnable()
    {
        ResetHealth();
    }

    // Unity-метод для начальной инициализации
    private void Start()
    {
        //this.health = this.maxHealth;
        //Debug.Log($"{Name} создан с {health} здоровья и скоростью {speed}");
    }

    public void Initialize(string playerName, uint maxHp, uint startHealth, uint dmg, float initialSpeed, uint healAmount)
    {
        Name = playerName;
        maxHealth = maxHp;
        health = startHealth;
        damage = dmg;
        speedModifier = initialSpeed;
        healingAmount = healAmount;
    }

    public void TakeDamage(uint damage)
    {
        health = health >= damage ? health - damage : 0;
        //Debug.Log($"{Name} получил урон. Здоровье: {health}");
    }

    public void TakeHeal()
    {
        health += healingAmount;
        if (health > maxHealth) health = maxHealth;
        //Debug.Log($"{Name} отхилился. Здоровье: {health}");
    }

    public void TakeSpeedboost(float speedMultiplier, bool isDebuff)
    {
        underDebufff = isDebuff;
        //Debug.Log($"{Name} получил скоростной баф {speedMultiplier}");
        SpeedModifier = (float)speedMultiplier; // Изменяем скорость, вызывая событие
        //Debug.Log($"{Name} новая скорость {Speed}");
    }

    private void ResetHealth()
    {
        health = maxHealth;
        isDead = false;
    }
}
