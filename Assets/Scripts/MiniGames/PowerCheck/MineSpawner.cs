using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    // ============================
    // Настройки спавна и области
    // ============================
    [Header("Spawn Settings")]
    [SerializeField] private Transform CenterPoint;                         // Центр спавна мин
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10, 10);   // Размер области спавна
    [SerializeField] private Transform parentTransform;                     // Родительский объект для мин
    [SerializeField] private List<Transform> forbiddenSpawnPoints;          // Точки, где спавн запрещен
    [SerializeField] private float allowedDistanseForForrbidenSpawnPoint;   // Дистанция до точек где спавн запрещен
    [SerializeField] private bool onTestSpawn;                              // Включение тестовой прогонки спавна мин при большом количестве итераций
    [SerializeField] private bool onTestProgressionSpawn;                   // Включение тестовой прогонки спавна мин с прогресией
    [SerializeField] private float yPositionOfSpawnMine;                    // высота  на которой надо заспавнить мину

    // ============================
    // Префабы для мин
    // ============================
    [Header("Mine Prefabs")]
    [SerializeField] private GameObject HealMinePrefab;     // Префаб мины лечения
    [SerializeField] private GameObject DamageMinePrefab;   // Префаб мины урона
    [SerializeField] private GameObject BuffMinePrefab;     // Префаб мины усиления скорости
    [SerializeField] private GameObject DebuffMinePrefab;   // Префаб мины уменьшения скорости

    // ============================
    // Настройки для мины лечения
    // ============================
    [Header("Heal Mine Settings")]
    [SerializeField] private float healCooldown = 1.0f; // Время перезарядки мины лечения
    [SerializeField] private int numberOfHealMines = 5; // Количество мин, которые будут спавниться сразу


    // ============================
    // Настройки для мины урона
    // ============================
    [Header("Damage Mine Settings")]
    [SerializeField] private float damageCooldown = 1.0f; // Время перезарядки мины урона
    [SerializeField] private int numberOfDamageMines = 5; // Количество мин, которые будут спавниться сразу


    // ============================
    // Настройки для мины усиления скорости
    // ============================
    [Header("Speed Buff Mine Settings")]
    [SerializeField] private float speedBufCooldown = 1.0f; // Время перезарядки мины усиления скорости
    [SerializeField] private float speedBuf = 1.0f;         // Множитель усиления скорости
    [SerializeField] private float buffTime = 2.0f;         // Время действия усиления скорости
    [SerializeField] private int numberOfBuffMines = 5;     // Количество мин, которые будут спавниться сразу
    [SerializeField] private int buffTimeBeforeExplosion = 5;     // Время до взрыва(активации эффектов)
    [SerializeField] private float buffRadiusOfExplosion = 5;     // Радиус взрыва

    // ============================
    // Настройки для мины ослабления скорости
    // ============================
    [Header("Speed Debuff Mine Settings")]
    [SerializeField] private float speedDebufCooldown = 1.0f; // Время перезарядки мины ослабления скорости
    [SerializeField] private float speedDebuf = 1.0f;         // Множитель ослабления скорости
    [SerializeField] private float debuffTime = 5.0f;         // Время действия ослабления скорости
    [SerializeField] private int numberOfDebuffMines = 5;     // Количество мин, которые будут спавниться сразу
    [SerializeField] private int debuffTimeBeforeExplosion = 5;     // Время до взрыва(активации эффектов)
    [SerializeField] private float debuffRadiusOfExplosion = 5;     // Радиус взрыва

    // ============================
    // Списки для каждого типа мин
    // ============================
    private MineList healMineList;
    private MineList damageMineList;
    private MineList buffMineList;
    private MineList debuffMineList;

    public IReadOnlyList<Mine> HealMines => healMineList.Minelist;
    public IReadOnlyList<Mine> DamageMines => damageMineList.Minelist;
    public IReadOnlyList<Mine> BuffMines => buffMineList.Minelist;
    public IReadOnlyList<Mine> DebuffMines => debuffMineList.Minelist;

    void Awake()
    {
        healMineList = new MineList(numberOfHealMines);
        damageMineList = new MineList(numberOfDamageMines);
        buffMineList = new MineList(numberOfBuffMines);
        debuffMineList = new MineList(numberOfDebuffMines);
       
        healMineList.InitializeMines(HealMinePrefab, healCooldown, (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
        damageMineList.InitializeMines(DamageMinePrefab, damageCooldown, (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
        buffMineList.InitializeSpeedBuffMines(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion);
        debuffMineList.InitializeSpeedBuffMines(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, debuffTimeBeforeExplosion, debuffRadiusOfExplosion);

    }

    private void Update()
    {
        if(onTestSpawn) SpawnMines();
        if (onTestProgressionSpawn) StartCoroutine(AddAndSpawnMines(2, 3));
    }

    // Добавляет трансформы переданных мин в список запрещенных точек спавна.
    private void AddForbiddenSpawnPoints(params Mine[] mines)
    {
        foreach (var mine in mines)
        {
            if (mine != null)
            {
                Transform mineTransform = mine.GetMine().transform;
                if (!forbiddenSpawnPoints.Contains(mineTransform))
                {
                    forbiddenSpawnPoints.Add(mineTransform);
                }
            }
        }
    }
    private void AddForbiddenSpawnPoints(List<Mine> mines)
    {
        foreach (var mine in mines)
        {
            if (mine != null)
            {
                Transform mineTransform = mine.GetMine().transform;
                if (!forbiddenSpawnPoints.Contains(mineTransform))
                {
                    forbiddenSpawnPoints.Add(mineTransform);
                }
            }
        }
    }

    public void SpawnMines()
    {
        SpawnMinesFromList(healMineList);

        SpawnMinesFromList(damageMineList);

        SpawnMinesFromList(buffMineList);

        SpawnMinesFromList(debuffMineList);
    }

    public void SpawnMinesFromList(MineList mineList)
    {
        foreach (Mine mine in mineList.Minelist)
        {
            SpawnMine(mine);
        }
    }

    public IEnumerator AddAndSpawnMines(int numOfMines, uint typeOfMine, float delayBetweenSpawns = 0.5f)
    {
        Mine newMine = null;
        while (numOfMines > 0)
        {
            switch (typeOfMine)
            {
                case 0:
                    newMine = this.healMineList.AddMine(HealMinePrefab, healCooldown,
                        (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
                    this.SpawnMine(newMine);
                    break;

                case 1:
                    newMine = this.damageMineList.AddMine(DamageMinePrefab, damageCooldown,
                        (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
                    this.SpawnMine(newMine);
                    break;

                case 2:
                    newMine = this.buffMineList.AddMine(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion,
                        (number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion) => new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion));
                    this.SpawnMine(newMine);
                    break;

                case 3:
                    newMine = this.debuffMineList.AddMine(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion,
                        (number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion) => new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion));
                    this.SpawnMine(newMine);
                    break;
            }

            numOfMines--;

            // Задержка перед созданием следующей мины
            if (delayBetweenSpawns > 0)
                yield return new WaitForSeconds(delayBetweenSpawns);
            else
                yield return null;
        }
    }


    public void SpawnMine(Mine mine)
    {
        StartCoroutine(SpawnMineWithDelay(mine));
    }

    private IEnumerator SpawnMineWithDelay(Mine mine)
    {
        this.AddForbiddenSpawnPoints(healMineList.Minelist);
        this.AddForbiddenSpawnPoints(damageMineList.Minelist);
        this.AddForbiddenSpawnPoints(buffMineList.Minelist);
        this.AddForbiddenSpawnPoints(debuffMineList.Minelist);

        yield return new WaitForSeconds(mine.GetCooldown());

        Vector3 randomPosition;
        bool positionValid;
        int numOfIterations = 0;

        do
        {
            float xPos = Random.Range(CenterPoint.position.x - spawnAreaSize.x / 2, CenterPoint.position.x + spawnAreaSize.x / 2);
            float zPos = Random.Range(CenterPoint.position.z - spawnAreaSize.y / 2, CenterPoint.position.z + spawnAreaSize.y / 2);

            randomPosition = new Vector3(xPos, yPositionOfSpawnMine, zPos);
            positionValid = true;

            foreach (Transform forbiddenPoint in forbiddenSpawnPoints)
            {
                numOfIterations++;
                if (Vector3.Distance(randomPosition, forbiddenPoint.position) < allowedDistanseForForrbidenSpawnPoint) 
                {
                    positionValid = false;
                    break;
                }
            }
        } while (!positionValid && numOfIterations < 1000000);

        if (numOfIterations < 100000)
        {
            mine.GetMine().transform.position = randomPosition;

            if (parentTransform != null)
            {
                mine.GetMine().transform.SetParent(parentTransform);
            }

            mine.SetActive(true);
        }

    }

}
