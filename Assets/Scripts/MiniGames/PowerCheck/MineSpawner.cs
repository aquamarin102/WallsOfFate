using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    // ============================
    // Настройки спавна и области
    // ============================
    [Header("Spawn Settings")]
    [SerializeField] private Transform CenterPoint; // Центр спавна мин
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10, 10); // Размер области спавна
    //[SerializeField] private int numberOfMines = 5; // Количество мин, которые будут спавниться
    [SerializeField] private Transform parentTransform; // Родительский объект для мин

    // ============================
    // Префабы для мин
    // ============================
    [Header("Mine Prefabs")]
    [SerializeField] private GameObject HealMinePrefab; // Префаб мины лечения
    [SerializeField] private GameObject DamageMinePrefab; // Префаб мины урона
    [SerializeField] private GameObject BuffMinePrefab; // Префаб мины усиления

    // ============================
    // Настройки для мины лечения
    // ============================
    [Header("Heal Mine Settings")]
    [SerializeField] private float healCooldown = 1.0f; // Время перезарядки мины лечения
    [SerializeField] private int numberOfHealMines = 5; // Количество мин, которые будут спавниться


    // ============================
    // Настройки для мины урона
    // ============================
    [Header("Damage Mine Settings")]
    [SerializeField] private float damageCooldown = 1.0f; // Время перезарядки мины урона
    [SerializeField] private int numberOfDamageMines = 5; // Количество мин, которые будут спавниться


    // ============================
    // Настройки для мины усиления скорости
    // ============================
    [Header("Speed Buff Mine Settings")]
    [SerializeField] private float speedBufCooldown = 1.0f; // Время перезарядки мины усиления скорости
    [SerializeField] private float speedBuf = 1.0f; // Множитель усиления скорости
    [SerializeField] private float buffTime = 2.0f; // Время действия усиления скорости
    [SerializeField] private int numberOfBuffMines = 5; // Количество мин, которые будут спавниться

    // ============================
    // Настройки для мины ослабления скорости
    // ============================
    [Header("Speed Debuff Mine Settings")]
    [SerializeField] private float speedDebufCooldown = 1.0f; // Время перезарядки мины ослабления скорости
    [SerializeField] private float speedDebuf = 1.0f; // Множитель ослабления скорости
    [SerializeField] private float debuffTime = 5.0f; // Время действия ослабления скорости
    [SerializeField] private int numberOfDebuffMines = 5; // Количество мин, которые будут спавниться

    // ============================
    // Списки для каждого типа мин
    // ============================
    private MineList healMineList;
    private MineList damageMineList;
    private MineList buffMineList;

    void Start()
    {
        healMineList = new MineList(numberOfHealMines);
        damageMineList = new MineList(numberOfDamageMines);
        buffMineList = new MineList(numberOfBuffMines);

        healMineList.InitializeMines(HealMinePrefab, healCooldown, (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
        damageMineList.InitializeMines(DamageMinePrefab, damageCooldown, (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
        buffMineList.InitializeSpeedBuffMines(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime);

        SpawnMines();
    }

    private void SpawnMines()
    {
        SpawnMinesFromList(healMineList);

        SpawnMinesFromList(damageMineList);

        SpawnMinesFromList(buffMineList);
    }

    private void SpawnMinesFromList(MineList mineList)
    {
        foreach (Mine mine in mineList.Minelist)
        {
            SpawnMine(mine);
        }
    }

    private void SpawnMine(Mine mine)
    {
        StartCoroutine(SpawnMineWithDelay(mine));
    }

    private IEnumerator SpawnMineWithDelay(Mine mine)
    {
        yield return new WaitForSeconds(mine.GetCooldown()); 

        float xPos = Random.Range(CenterPoint.position.x - spawnAreaSize.x / 2, CenterPoint.position.x + spawnAreaSize.x / 2);
        float zPos = Random.Range(CenterPoint.position.z - spawnAreaSize.y / 2, CenterPoint.position.z + spawnAreaSize.y / 2);

        Vector3 randomPosition = new Vector3(xPos, 0f, zPos);

        mine.GetMine().transform.position = randomPosition;

        if (parentTransform != null)
        {
            mine.GetMine().transform.SetParent(parentTransform);
        }

        mine.SetActive(true); 
    }

}
