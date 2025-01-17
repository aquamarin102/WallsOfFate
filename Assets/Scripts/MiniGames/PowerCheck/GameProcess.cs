using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameProcess : MonoBehaviour
{
    [SerializeField] private MineSpawner mineSpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    // ============================
    // Настройки игровых правил
    // ============================
    [Header("Game Rules")]
    [SerializeField] private float delayBetweenMineSpawn;       // Пауза между спавном мин
    [SerializeField] private int numberOfMinesSpawnEveryTime;   // Количество мин котторые будут заспавнены

    public GameObject healMine;
    public GameObject damageMine;
    public GameObject speedBuffMine;
    public GameObject speedDebufMine;

    IReadOnlyList<Mine> healMines;
    IReadOnlyList<Mine> damageMines;
    IReadOnlyList<Mine> buffMines;
    IReadOnlyList<Mine> debuffMines;

    void Start()
    {
        // Получаем список мин с MineSpawner
        healMines = mineSpawner.HealMines;
        damageMines = mineSpawner.DamageMines;
        buffMines = mineSpawner.BuffMines;
        debuffMines = mineSpawner.DebuffMines;

        // Для каждой мины подписываемся на событие
        SubscribeToMineEvents(healMines);
        SubscribeToMineEvents(damageMines);
        SubscribeToMineEvents(buffMines);
        SubscribeToMineEvents(debuffMines);
        mineSpawner.SpawnMines();
    }

    private void FixedUpdate()
    {
        //StartCoroutine(mineSpawner.AddAndSpawnMines(numberOfMinesSpawnEveryTime, 3, delayBetweenMineSpawn));
        //SubscribeToMineEvents(debuffMines);

    }

    private void SubscribeToMineEvents(IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            // Получаем префаб и компонент TriggerHandler
            GameObject minePrefab = mine.GetMine();
            TriggerHandler mineTriggerHandler = minePrefab.GetComponent<TriggerHandler>();

            if (mineTriggerHandler != null)
            {
                // Подписываемся на событие OnMineTriggered
                mineTriggerHandler.OnObjectEnteredTrigger += (triggeredObject, objectWhoTriger) =>
                {
                    HandleTriggeredObject(triggeredObject, objectWhoTriger);
                };
            }
        }
    }

    private void HandleTriggeredObject(GameObject triggeredObject, GameObject objectWhoTriger)
    {
        // Определяем, к какой категории мин принадлежит объект
        Mine mine = FindMineByGameObject(triggeredObject);

        if (mine != null)
        {
            //Debug.Log($"Мина с номером {mine.GetNumber()} вызвала событие.");
            HandleMineTriggered(mine, objectWhoTriger);
        }
        else
        {
            Debug.LogWarning($"Не удалось определить, к какой мине принадлежит объект {triggeredObject.name}.");
        }
    }

    private Mine FindMineByGameObject(GameObject triggeredObject)
    {
        // Проверяем во всех списках
        Mine mine = FindMineInList(triggeredObject, healMines);
        if (mine != null)
        {
            //Debug.Log("это мина хила");
            return mine;
        }

        mine = FindMineInList(triggeredObject, damageMines);
        if (mine != null)
        {
            //Debug.Log("это мина дамага");
            return mine;
        }

        mine = FindMineInList(triggeredObject, buffMines);
        if (mine != null)
        {
            //Debug.Log("это мина ускорения");
            return mine;
        }

        mine = FindMineInList(triggeredObject, debuffMines);
        if (mine != null)
        {
            //Debug.Log("это мина замедления");
            return mine;
        }

        // Если не найдено, возвращаем null
        return null;
    }

    private Mine FindMineInList(GameObject triggeredObject, IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            if (mine.GetMine() == triggeredObject)
            {
                return mine;
            }
        }

        return null;
    }


    private void HandleMineTriggered(Mine givedMine, GameObject givedPlayer)
    {
        Player givedPlayerChar = givedPlayer.GetComponent<Player>();
        Player playerChar = player.GetComponent<Player>();
        Player enemyChar = enemy.GetComponent<Player>();
        if (givedMine is HealMine healMine)
        {
            healMine.Heal(givedPlayerChar);
        }
        else if (givedMine is DamageMine damageMine)
        {
            if (givedPlayerChar.name == "Player")
                damageMine.Damage(enemyChar, playerChar);
            else
                damageMine.Damage(playerChar, enemyChar);
        }
        else if (givedMine is BuffSpeedMine buffSpeedMine)
        {
            MineExplosion(buffSpeedMine, player, enemy);
        }

        givedMine.SetActive(false); 
        mineSpawner.SpawnMine(givedMine);
    }

    private IEnumerator MineExplosion(BuffSpeedMine mine, params GameObject[] objects)
    {
        // Ждем паузу в заданное время
        yield return new WaitForSeconds(mine.GetTimeBeforeExplosion() / 1000f);

        // Находим все объекты на определенном расстоянии от мины
        List<Player> affectedPlayers = mine.FindDistanceToMine(objects);

        // Применяем бафф к найденным объектам
        StartCoroutine(BuffSpeedListCoroutine(mine, affectedPlayers));

        Debug.Log("Mine explosion completed, buff applied to nearby players.");
    }

    private IEnumerator BuffSpeedListCoroutine(BuffSpeedMine mine, List<Player> players)
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                StartCoroutine(BuffSpeedCoroutine(mine, player));
                yield return null; // Ждем следующего кадра
            }
        }
    }

    private IEnumerator BuffSpeedCoroutine(BuffSpeedMine mine, Player player)
    {
        player.TakeSpeedboost(mine.GetSpeedBuff()); // Применяем начальный бафф
        yield return new WaitForSeconds(mine.GetBuffCooldown());
        player.TakeSpeedboost(1 / mine.GetSpeedBuff()); // Убираем бафф
    }
}
