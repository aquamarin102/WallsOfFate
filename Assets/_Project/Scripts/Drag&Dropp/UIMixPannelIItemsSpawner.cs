using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Drag_Dropp
{
    public class UISpawner : MonoBehaviour
    {
        public GameObject prefab; // Префаб, который будет спавниться
        public Transform parentTransform; // Родительский объект, на котором будут размещаться префабы
        public Transform spawnPoint; // Точка спавна (глобальная позиция)
        public int initialCount = 3; // Начальное количество префабов

        private List<GameObject> spawnedPrefabs = new List<GameObject>();

        private void Start()
        {
            SpawnPrefabs(initialCount);
        }

        private void SpawnPrefabs(int count)
        {
            // Очищаем старые префабы
            foreach (var prefab in spawnedPrefabs)
            {
                Destroy(prefab);
            }
            spawnedPrefabs.Clear();

            // Получаем ширину родительского объекта
            RectTransform parentRect = parentTransform.GetComponent<RectTransform>();
            float parentWidth = parentRect.rect.width;

            // Рассчитываем ширину каждого префаба
            float prefabWidth = parentWidth / count;

            // Получаем глобальную позицию точки спавна
            Vector3 spawnPositionGlobal = spawnPoint.position;

            // Рассчитываем начальную позицию для первого префаба
            float startX = -parentWidth / 2 + prefabWidth / 2; // Начало с учетом центрирования

            for (int i = 0; i < count; i++)
            {
                // Создаем новый префаб
                GameObject newPrefab = Instantiate(prefab, parentTransform);

                // Устанавливаем размер
                RectTransform prefabRect = newPrefab.GetComponent<RectTransform>();
                prefabRect.sizeDelta = new Vector2(prefabWidth, prefabRect.sizeDelta.y);

                // Рассчитываем позицию по X для равномерного распределения
                float posX = startX + i * prefabWidth;

                // Устанавливаем глобальную позицию для нового префаба
                Vector3 newPrefabPositionGlobal = spawnPositionGlobal + new Vector3(posX, 0, 0);

                // Преобразуем глобальную позицию в локальную относительно родительского объекта
                Vector3 newPrefabPositionLocal = parentTransform.InverseTransformPoint(newPrefabPositionGlobal);

                // Устанавливаем локальную позицию префаба
                prefabRect.anchoredPosition = newPrefabPositionLocal;

                // Добавляем префаб в список
                spawnedPrefabs.Add(newPrefab);

                // Подписываемся на событие OnDrop
                ItemSlot itemSlot = newPrefab.GetComponent<ItemSlot>();
                if (itemSlot != null)
                {
                    //itemSlot.OnDropEvent += OnDropHandler;
                }
            }
        }

        private void OnDropHandler(PointerEventData eventData)
        {
            // После срабатывания OnDrop, спавним новый префаб
            SpawnPrefabs(spawnedPrefabs.Count + 1);
        }
    }
}