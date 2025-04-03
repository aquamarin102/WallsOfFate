using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameProcess : MonoBehaviour
{
    [SerializeField] private MineSpawner _mineSpawner;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;

    IReadOnlyList<Mine> _healMines;
    IReadOnlyList<Mine> _damageMines;
    IReadOnlyList<Mine> _buffMines;
    IReadOnlyList<Mine> _debuffMines;

    private PlayerMove _playerMove;
    private AIController _enemyMove;

    private MiniGamePlayer playerChar;
    private MiniGamePlayer enemyChar;

    [Inject]
    private void Construct([Inject(Id = "Player")] PlayerMove player, [Inject(Id = "Enemy")] AIController enemy)
    {
        _player = player.gameObject;
        _enemy = enemy.gameObject;
    }

    public event Action<string, string> OnEndGame;

    void Start()
    {
        _mineSpawner = GameObject.FindGameObjectWithTag("MineSpawner").GetComponent<MineSpawner>();

        _playerMove = _player.GetComponent<PlayerMove>();
        _enemyMove = _enemy.GetComponent<AIController>();

        playerChar = _player.GetComponent<MiniGamePlayer>();
        enemyChar = _enemy.GetComponent<MiniGamePlayer>();

        playerChar.OnSpeedChanged += _playerMove.ChangeSpeed;
        enemyChar.OnSpeedChanged += _enemyMove.ChangeSpeed;

        // Получаем список мин с MineSpawner
        _healMines = _mineSpawner.HealMines;
        _damageMines = _mineSpawner.DamageMines;
        _buffMines = _mineSpawner.BuffMines;
        _debuffMines = _mineSpawner.DebuffMines;

        // Для каждой мины подписываемся на событие
        SubscribeToMineEvents(_healMines);
        SubscribeToMineEvents(_damageMines);
        SubscribeToMineEvents(_buffMines);
        SubscribeToMineEvents(_debuffMines);
    }

    private void FixedUpdate()
    {
        //if (_isPanelActive) return;
        SubscribeToMineEvents(_debuffMines);
        if ((playerChar.Health <= 0 && !playerChar.isDead) || (enemyChar.Health <= 0 && !enemyChar.isDead))
        {
            string winner, loser;
            if (playerChar.Health > 0)
            {
                winner = playerChar.Name;
                loser = enemyChar.Name;
            }
            else
            {
                winner = enemyChar.Name;
                loser = playerChar.Name;
            }

            playerChar.Health = 0;
            enemyChar.Health = 0;
            playerChar.isDead = true;
            enemyChar.isDead = true;

            OnEndGame?.Invoke(winner, loser);
        }
    }
        
    private void SubscribeToMineEvents(IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            // Получаем префаб и компонент TriggerHandler
            GameObject minePrefab = mine.MineGameObject;
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
        Mine mine = FindMineInList(triggeredObject, _healMines);
        if (mine != null)
        {
            return mine;
        }

        mine = FindMineInList(triggeredObject, _damageMines);
        if (mine != null)
        {
            return mine;
        }

        mine = FindMineInList(triggeredObject, _buffMines);
        if (mine != null)
        {
            return mine;
        }

        mine = FindMineInList(triggeredObject, _debuffMines);
        if (mine != null)
        {
            return mine;
        }

        // Если не найдено, возвращаем null
        return null;
    }

    private Mine FindMineInList(GameObject triggeredObject, IEnumerable<Mine> mines)
    {
        foreach (Mine mine in mines)
        {
            if (mine.MineGameObject == triggeredObject)
            {
                return mine;
            }
        }
            
        return null;
    }

    private void HandleMineTriggered(Mine givedMine, GameObject givedPlayer)
    {
        MiniGamePlayer givedPlayerChar = givedPlayer.GetComponent<MiniGamePlayer>();
        MiniGamePlayer playerChar = _player.GetComponent<MiniGamePlayer>();
        MiniGamePlayer enemyChar = _enemy.GetComponent<MiniGamePlayer>();

        if (givedMine is HealMine healMine)
        {
            healMine.Heal(givedPlayerChar);
        }
        else if (givedMine is DamageMine damageMine)
        {
            if (givedPlayerChar.Name == "Player")
                damageMine.Damage(enemyChar, playerChar);
            else
                damageMine.Damage(playerChar, enemyChar);
        }
        else if (givedMine is BuffSpeedMine buffSpeedMine)
        {
            MineExplosion(buffSpeedMine, _player, _enemy);
        }

        givedMine.SetActive(false);
    }

    private async void MineExplosion(BuffSpeedMine mine, params GameObject[] objects)
    {
        Vector3 initialMinePosition = mine.MineGameObject.transform.position;

        // Ждем паузу в 3 секунды
        await Task.Delay(mine.GetTimeBeforeExplosion());

        // Находим все объекты на определенном расстоянии от мины
        List<MiniGamePlayer> affectedPlayers = mine.FindDistanceToMine(initialMinePosition, objects);

        // Передаем найденные объекты в метод BuffSpeedList
        await mine.BuffSpeedList(affectedPlayers);
    }
}