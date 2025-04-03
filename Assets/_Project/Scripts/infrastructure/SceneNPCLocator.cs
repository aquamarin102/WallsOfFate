using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.infrastructure
{
    internal class SceneNPCLocator : MonoInstaller
    {
        public List<GameObject> NPC; // Список NPC префабов
        public Transform Parent; // Родительский объект для NPC

        public List<Transform> StartPoints; // Список точек старта для каждого NPC

        public override void InstallBindings()
        {
            InstantiateNPSPrefab();
        }

        private void InstantiateNPSPrefab()
        {
            if (NPC.Count != StartPoints.Count)
            {
                Debug.LogError("Количество NPC и точек старта должно быть одинаковым!");
                return;
            }

            for (int i = 0; i < NPC.Count; i++)
            {
                GameObject prefab = NPC[i];
                Transform startPoint = StartPoints[i];

                // Получаем позицию и поворот из Transform точки старта
                Vector3 spawnPosition = startPoint.position;
                Quaternion spawnRotation = startPoint.rotation;

                // Инстанцируем NPC
                GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, Parent);
            }
        }
    }
}
