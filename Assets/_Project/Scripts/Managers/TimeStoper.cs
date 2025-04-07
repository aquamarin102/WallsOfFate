using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Managers
{
    public class TimeStoper : MonoBehaviour
    {
        [field: SerializeField] public List<GameObject> TargetObjects { get; private set; }
        private void Update()
        {
            if (TargetObjects != null && TargetObjects.Any(obj => obj.activeSelf != false)) TimeStop(true);
            else TimeStop(false);
        }
        public void TimeStop(bool state)
        {
            if (state)
            {
                Time.timeScale = 0f; // Останавливаем время
                Debug.Log("Игра на паузе");
            }
            else
            {
                Time.timeScale = 1f; // Восстанавливаем время
                Debug.Log("Игра продолжается");
            }
        }
    }
}
