using Assets.Scripts.MiniGames.PowerCheck.GridCoordinates;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class MineSpawner : MonoBehaviour
{
    // ============================
    // Настройки спавна и области
    // ============================
    [Header("Spawn Settings")]
    [SerializeField] private Transform CenterPoint;                         // Центр спавна мин
    public Vector2 spawnAreaSize = new Vector2(10, 10);                      // Размер области спавна

    [SerializeField] private Transform parentTransform;                     // Родительский объект для мин
    [SerializeField] private List<Transform> _forbiddenSpawnPoints;          // Точки, где спавн запрещен
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
    [SerializeField] private float speedBufCooldown = 1.0f;       // Время перезарядки мины усиления скорости
    [SerializeField] private float speedBuf = 1.0f;               // Множитель усиления скорости
    [SerializeField] private float buffTime = 2.0f;               // Время действия усиления скорости
    [SerializeField] private int numberOfBuffMines = 5;           // Количество мин, которые будут спавниться сразу
    [SerializeField] private int buffTimeBeforeExplosion = 5;     // Время до взрыва(активации эффектов)
    [SerializeField] private float buffRadiusOfExplosion = 5;     // Радиус взрыва
    [SerializeField] private uint buffDamage = 5;                 // Урон

    // ============================
    // Настройки для мины ослабления скорости
    // ============================
    [Header("Speed Debuff Mine Settings")]
    [SerializeField] private float speedDebufCooldown = 1.0f;       // Время перезарядки мины ослабления скорости
    [SerializeField] private float speedDebuf = 1.0f;               // Множитель ослабления скорости
    [SerializeField] private float debuffTime = 5.0f;               // Время действия ослабления скорости
    [SerializeField] private int numberOfDebuffMines = 5;           // Количество мин, которые будут спавниться сразу
    [SerializeField] private float debufDelayOfSpawn = 5;           // Пауза между спавном мин
    [SerializeField] private int debuffTimeBeforeExplosion = 5;     // Время до взрыва(активации эффектов)
    [SerializeField] private float debuffRadiusOfExplosion = 5;     // Радиус взрыва
    [SerializeField] private uint debuffDamage = 5;                 // Урон

    // ============================
    // Списки для каждого типа мин
    // ============================
    private MineList healMineList;
    private MineList damageMineList;
    private MineList buffMineList;
    private MineList debuffMineList;

    private bool _isSpawn = false;
    private bool _isAdding = false;

    private GridCordinates _gridCordinates;

    public IReadOnlyList<Mine> HealMines => healMineList.Minelist;
    public IReadOnlyList<Mine> DamageMines => damageMineList.Minelist;
    public IReadOnlyList<Mine> BuffMines => buffMineList.Minelist;
    public IReadOnlyList<Mine> DebuffMines => debuffMineList.Minelist;

    [Inject]
    private void Construct([Inject(Id = "Player")] PlayerMove player, [Inject(Id = "Enemy")] AIController enemy)
    {
        _forbiddenSpawnPoints.Add(player.gameObject.transform);
        _forbiddenSpawnPoints.Add(enemy.gameObject.transform);
    }

    void Awake()
    {
        healMineList = new MineList(numberOfHealMines);
        damageMineList = new MineList(numberOfDamageMines);
        buffMineList = new MineList(numberOfBuffMines);
        debuffMineList = new MineList(numberOfDebuffMines);

        healMineList.InitializeMines(HealMinePrefab, healCooldown, (number, cooldown, mineGameObject) => new HealMine(number, cooldown, mineGameObject));
        damageMineList.InitializeMines(DamageMinePrefab, damageCooldown, (number, cooldown, mineGameObject) => new DamageMine(number, cooldown, mineGameObject));
        buffMineList.InitializeSpeedBuffMines(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion, buffRadiusOfExplosion, buffDamage, false);
        debuffMineList.InitializeSpeedBuffMines(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime, debuffTimeBeforeExplosion, debuffRadiusOfExplosion, debuffDamage, true);

    }

    private void FixedUpdate()
    {
        if (onTestSpawn)
        {
            //Debug.Log("isSpawn = " + _isSpawn);
            SpawnMines();
        }
        if (onTestProgressionSpawn && !_isAdding)
        {
            StartCoroutine(AddMinesToListWhithDalay(numberOfDebuffMines, 3, debufDelayOfSpawn));
        }
        //if (onTestProgressionSpawn) AddMinesToList(numberOfDebuffMines, 3, debufDelayOfSpawn);
        //SpawnMinesFromList(debuffMineList);
    }

    private void AddForbiddenSpawnPoints(List<Mine> mines)
    {
        foreach (var mine in mines)
        {
            if (mine != null)
            {
                Transform mineTransform = mine.MineGameObject.transform;
                if (!_forbiddenSpawnPoints.Contains(mineTransform))
                {
                    _forbiddenSpawnPoints.Add(mineTransform);
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
        // Перебираем список с конца
        for (int i = mineList.Minelist.Count - 1; i >= 0; i--)
        {
            Mine mine = mineList.Minelist[i];

            // Проверяем, если мина не активирована (не задисейблена), то спавним
            if (!mine.MineGameObject.activeSelf && !_isSpawn)
            {
                _isSpawn = true;
                //Debug.Log("Заспавнены мины из одного списка!!!");
                SpawnMine(mine, mine.Cooldown);
            }
        }
    }

    public IEnumerator AddMinesToListWhithDalay(int numOfMines, uint typeOfMine, float delayBetweenSpawns = 0.5f)
    {
        _isAdding = true;

        while (numOfMines > 0)
        {
            AddMineByType(typeOfMine);

            numOfMines--;

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        _isAdding = false;
    }

    private void AddMineByType(uint typeOfMine)
    {
        switch (typeOfMine)
        {
            case 0:
                this.healMineList.AddMine(HealMinePrefab, healCooldown, (number, cooldown, mineGameObject) =>
                    new HealMine(number, cooldown, mineGameObject));
                break;
            case 1:
                this.damageMineList.AddMine(DamageMinePrefab, damageCooldown, (number, cooldown, mineGameObject) =>
                    new DamageMine(number, cooldown, mineGameObject));
                break;
            case 2:
                this.buffMineList.AddMine(BuffMinePrefab, speedBufCooldown, speedBuf, buffTime, buffTimeBeforeExplosion,
                    buffRadiusOfExplosion, buffDamage, (number, cooldown, mineGameObject, speedbuff, buffcooldown,
                        buffTimeBeforeExplosion, buffRadiusOfExplosion, buffDamage) =>
                        new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown,
                            buffTimeBeforeExplosion, buffRadiusOfExplosion, buffDamage, false));
                break;
            case 3:
                this.debuffMineList.AddMine(DebuffMinePrefab, speedDebufCooldown, speedDebuf, debuffTime,
                    buffTimeBeforeExplosion, buffRadiusOfExplosion, debuffDamage, (number, cooldown, mineGameObject,
                        speedbuff, buffcooldown, buffTimeBeforeExplosion, buffRadiusOfExplosion, debuffDamage) =>
                        new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown,
                            buffTimeBeforeExplosion, buffRadiusOfExplosion, debuffDamage, true));
                break;
        }
    }


    public void SpawnMine(Mine mine, float cooldawn)
    {
        StartCoroutine(SpawnMineWithDelay(mine, cooldawn));
    }

    private IEnumerator SpawnMineWithDelay(Mine mine, float cooldawn)
    {
        //Debug.Log("Спавним мину!!!");
        //mine.SetActive(false);
        this.AddForbiddenSpawnPoints(healMineList.Minelist);
        this.AddForbiddenSpawnPoints(damageMineList.Minelist);
        this.AddForbiddenSpawnPoints(buffMineList.Minelist);
        this.AddForbiddenSpawnPoints(debuffMineList.Minelist);

        float delay = mine.IsFirstSpawn ? 0f : cooldawn;
        yield return new WaitForSeconds(delay);

        mine.IsFirstSpawn = false;

        Vector3 randomPosition;
        bool positionValid;
        int numOfIterations = 0;

        do
        {
            Random.InitState(System.DateTime.Now.Millisecond); // Инициализация случайного генератора с текущим временем

            int randomRow = Mathf.RoundToInt(Random.Range(CenterPoint.position.x - spawnAreaSize.x / 2, CenterPoint.position.x + spawnAreaSize.x / 2));
            int randomColumn = Mathf.RoundToInt(Random.Range(CenterPoint.position.z - spawnAreaSize.y / 2, CenterPoint.position.z + spawnAreaSize.y / 2));

            //Vector2 cellposition = _gridCordinates.CordMatrix[randomRow][randomColumn].GlobalCenter; 

            randomPosition = new Vector3(randomRow, CenterPoint.position.y, randomColumn);
            positionValid = true;

            foreach (Transform forbiddenPoint in _forbiddenSpawnPoints)
            {
                numOfIterations++;
                if (Vector3.Distance(randomPosition, forbiddenPoint.position) < allowedDistanseForForrbidenSpawnPoint)
                {
                    positionValid = false;
                    break;
                }
            }
        } while (!positionValid && numOfIterations < 1000000);


        mine.MineGameObject.transform.position = randomPosition;

        if (parentTransform != null)
        {
            mine.MineGameObject.transform.SetParent(parentTransform);
        }

        mine.SetActive(true);

        _isSpawn = false;
    }
}
