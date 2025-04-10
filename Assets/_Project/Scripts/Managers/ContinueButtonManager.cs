using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Scripts.Managers
{
    public class ContinueButtonManager: MonoBehaviour
    {
        [SerializeField] private GameObject _button;
        private SaveLoadManager _saveLoadManager;

        private void Awake()
        {
            _saveLoadManager = GetComponent<SaveLoadManager>();
            // Если сохранённый прогресс есть, активируем кнопку "Продолжить"
            if (_saveLoadManager != null && _saveLoadManager.CanLoad())
            {
                _button.SetActive(true);
            }
            else
            {
                _button.SetActive(false);
            }
        }

    }
}
