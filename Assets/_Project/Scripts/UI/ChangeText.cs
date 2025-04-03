using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class ChangeText : MonoBehaviour
    {
        private GameObject _textField;

        private void Start()
        {
            // Находим объект с тегом "TextField"
            _textField = GameObject.FindWithTag("TextField");

            if (_textField == null)
            {
                Debug.LogWarning("Объект с тегом 'TextField' не найден.");
            }
        }

        public void ChangeTextContent()
        {
            // Получаем данные пикапа по имени объекта
            Pickup pickup = AssembledPickups.FindByName(this.gameObject.name);

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
                textComponent.text = $"{pickup.Description}\n{pickup.HideDescription}";
            }
            else if (textMeshProComponent != null)
            {
                // Устанавливаем текст для компонента TextMeshProUGUI
                textMeshProComponent.text = $"{pickup.Description} {pickup.HideDescription}";
            }
            else
            {
                Debug.LogWarning("Не удалось найти компонент Text или TextMeshProUGUI в префабе.");
            }
        }
    }
}