using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MiniGameInstaller : MonoInstaller
{
    [Header("Player Settings")]
    public Transform StartPoint;
    public GameObject PlayerPrefab;
    public Transform Parent;
    public Transform CameraTransform;

    [Header("HP Bar Settings")]
    public Transform CanvasTransform;
    public Slider PlayerHPBarPrefab;
    public Slider EnemyHPBarPrefab;

    [Header("Enemie Settings")]
    public Transform SpawnPoint;
    public GameObject EnemyPrefab;

    [Header("Game Settings")]
    public MineSpawner MineSpawnerObject;

    public override void InstallBindings()
    {
        if (PlayerHPBarPrefab == null || EnemyHPBarPrefab == null || CanvasTransform == null)
        {
            Debug.LogError("HP Bar settings are not set!", this);
            return;
        }

        BindCameraTransform();
        BindPlayer();
        BindEnemies();
        BindMineSpawner();
    }

    private void BindMineSpawner()
    {
        Container.Bind<MineSpawner>().FromInstance(MineSpawnerObject).AsSingle().NonLazy();
    }

    private void BindPlayer()
    {
        if (PlayerPrefab == null || StartPoint == null)
        {
            Debug.LogError("Player prefab or StartPoint is missing!", this);
            return;
        }

        PlayerMove player = Container.InstantiatePrefabForComponent<PlayerMove>(
            PlayerPrefab, StartPoint.position, PlayerPrefab.transform.rotation, Parent);

        Container.Bind<PlayerMove>().WithId("Player").FromInstance(player).AsSingle();


        // Создаем и передаем HealthBar
        Slider playerHealthBar = Instantiate(PlayerHPBarPrefab, CanvasTransform);

        HealthBarManager healthBarManager = player.GetComponent<HealthBarManager>();
        if (healthBarManager != null)
        {
            healthBarManager.SetHealthBar(playerHealthBar);
        }
    }

    private void BindEnemies()
    {
        Transform spawnPoint = SpawnPoint;
        GameObject enemyPrefab = EnemyPrefab;
        AIController enemy = Container.InstantiatePrefabForComponent<AIController>(enemyPrefab, spawnPoint.position, spawnPoint.rotation, Parent);
        Container.Bind<AIController>().WithId("Enemy").FromInstance(enemy).AsSingle();

        HealthBarManager healthBarManager = enemy.GetComponent<HealthBarManager>();
        if (healthBarManager != null)
        {
            Slider enemyHealthBar = Instantiate(EnemyHPBarPrefab, CanvasTransform);

            healthBarManager.SetHealthBar(enemyHealthBar);
        }
    }
    private void BindCameraTransform()
    {
        Container
            .Bind<Transform>()
            //.WithId("PwerCheckCamera")
            .FromInstance(CameraTransform)
            .WhenInjectedInto<PlayerMove>();
    }

}
