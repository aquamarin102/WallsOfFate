using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class ChangeText : MonoBehaviour
    {
        private GameObject _textField;
        private GameObject _indication;
        Pickup _pickup;


        private void Start()
        {
            // Находим объект с тегом "TextField"
            _textField = GameObject.FindWithTag("TextField"); 
            _indication = GetComponentsInChildren<Transform>(true)
    .FirstOrDefault(t => t.CompareTag("InteractionIndicator"))?.gameObject;

            if (_textField == null)
            {
                Debug.LogWarning("Объект с тегом 'TextField' не найден.");
            }


            _pickup = AssembledPickups.FindByName(this.gameObject.name);
            if (_pickup == null)
            {
                _pickup = GetComponent<Pickup>();
            }
        }

        private void Update()
        {
            //Text textComponent = _textField?.GetComponent<Text>();
            TMPro.TextMeshProUGUI textMeshProComponent = _textField.GetComponent<TMPro.TextMeshProUGUI>();
            if ((_pickup.Description + "\n" +_pickup.HideDescription) != textMeshProComponent.text && _indication.activeSelf) _indication.SetActive(false);
        }

        public void ChangeTextContent()
        {
            if (_textField == null)
            {
                Debug.LogWarning("TextField не назначен.");
                return;
            }

            // Получаем компонент Text или TextMeshProUGUI у текстового поля
            Text textComponent = _textField.GetComponent<Text>();
            TMPro.TextMeshProUGUI textMeshProComponent = _textField.GetComponent<TMPro.TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Устанавливаем текст для стандартного компонента Text
                textComponent.text = $"{_pickup.Description}\n{_pickup.HideDescription}";
                _indication.SetActive(true);
            }
            else if (textMeshProComponent != null)
            {
                // Устанавливаем текст для компонента TextMeshProUGUI
                textMeshProComponent.text = $"{_pickup.Description}\n{_pickup.HideDescription}";
                _indication.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Не удалось найти компонент Text или TextMeshProUGUI в префабе.");
            }
        }
    }
}