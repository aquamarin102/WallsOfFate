using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class ChangeBackgroundImage : MonoBehaviour
    {
        private GameObject _imagePannel;

        private void Start()
        {
            // Находим объект с тегом "ImageField"
            _imagePannel = GameObject.FindWithTag("ImageField");

            if (_imagePannel == null)
            {
                Debug.LogWarning("Объект с тегом 'ImageField' не найден.");
            }
        }

        public void ChangeBackground()
        {
            if (_imagePannel == null)
            {
                Debug.LogWarning("ImagePannel не назначен.");
                return;
            }

            // Получаем компонент Image у панели
            Image backgroundPannelImage = _imagePannel.GetComponent<Image>();
            Sprite backgroundPickupSprite = this.gameObject.GetComponent<Image>().sprite;

            if (backgroundPannelImage != null)
            {

                if (backgroundPickupSprite != null)
                {
                    backgroundPannelImage.sprite = backgroundPickupSprite; // Устанавливаем фон панели
                }
                else
                {
                    Debug.LogWarning($"There no Image on {this.gameObject.name}");
                    backgroundPannelImage.sprite = null; // Очищаем фон, если спрайт не найден
                }
            }
            else
            {
                Debug.LogWarning("Не удалось найти компонент Image в префабе.");
            }
        }
    }
}