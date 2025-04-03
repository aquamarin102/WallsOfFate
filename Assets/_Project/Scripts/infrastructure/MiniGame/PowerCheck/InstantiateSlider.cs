using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Infrastructure.MiniGame.General
{
    internal class InstantiateSlider : MonoInstaller
    {
        public List<Transform> SpawnSliderPoints;  // Точки спавна для HPBar
        public List<GameObject> Prefabs;           // Префабы с HealthBarManager
        public Transform Parent;                   // Родитель для всех персонажей
        public Slider SliderPrefab;                // Префаб HPBar
        public Transform CanvasTransform;          // Canvas, в котором создаются HPBar

        public override void InstallBindings()
        {
            if (SliderPrefab == null || Prefabs.Count == 0 || SpawnSliderPoints.Count == 0 || CanvasTransform == null)
            {
                Debug.LogError("Некорректно настроены ссылки в InstantiateSlider!", this);
                return;
            }

            // Биндим слайдер как префаб, который будет инжектироваться
            Container
                .Bind<Slider>()
                .FromInstance(SliderPrefab)
                .AsSingle();

            for (int i = 0; i < Mathf.Min(Prefabs.Count, SpawnSliderPoints.Count); i++)
            {
                Transform spawnPoint = SpawnSliderPoints[i];
                GameObject prefab = Prefabs[i];

                // Создаем персонажа
                GameObject instance = Container.InstantiatePrefab(prefab, spawnPoint.position, spawnPoint.rotation, Parent);

                // Получаем компонент HealthBarManager
                HealthBarManager healthBarManager = instance.GetComponent<HealthBarManager>();

                if (healthBarManager == null)
                {
                    Debug.LogError($"Префаб {prefab.name} не содержит компонент HealthBarManager!", this);
                    continue;
                }

                // Создаем HPBar и добавляем его на Canvas
                Slider healthBarInstance = Container.InstantiatePrefabForComponent<Slider>(SliderPrefab, CanvasTransform);

                // Устанавливаем HPBar в позицию над персонажем
                RectTransform healthBarRect = healthBarInstance.GetComponent<RectTransform>();
                healthBarRect.anchoredPosition = Vector2.zero;  // Если Canvas в World Space, можно оставить 0

                // Инъекция зависимостей
                Container.Inject(healthBarManager);
            }
        }
    }
}
