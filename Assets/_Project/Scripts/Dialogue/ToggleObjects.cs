using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Dialogue
{
    public class ToggleObjects : MonoBehaviour
    {
        [Header("Objects to Toggle")]
        [SerializeField] private GameObject firstObject;  // Первый объект
        [SerializeField] private GameObject secondObject; // Второй объект

        // Основной метод для управления активностью
        public void SetActiveState(bool activateFirst)
        {
            if (activateFirst)
            {
                ActivateObject(firstObject);
                DeactivateObject(secondObject);
            }
            else
            {
                ActivateObject(secondObject);
                DeactivateObject(firstObject);
            }
        }

        private void ActivateObject(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Object is not assigned!");
            }
        }

        private void DeactivateObject(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Object is not assigned!");
            }
        }

        // Методы для прямого вызова
        public void ActivateFirst() => SetActiveState(true);
        public void ActivateSecond() => SetActiveState(false);
    }
}
