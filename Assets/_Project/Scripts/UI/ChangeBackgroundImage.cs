using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    internal class ChangeBackgroundImage : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Путь к спрайту относительно папки Resources (без расширения)")]
        [SerializeField] private string _spritePath;

        private GameObject _imagePannel;
        private Pickup _pickup;

        private void Start()
        {
            // Находим объект с тегом "ImageField"
            _imagePannel = GameObject.FindWithTag("ImageField");

            if (_imagePannel == null)
            {
                Debug.LogWarning("Объект с тегом 'ImageField' не найден.");
                return;
            }

            // Ищем соответствующий Pickup
            _pickup = AssembledPickups.FindByName(gameObject.name) ?? GetComponent<Pickup>();
        }

        public void ChangeBackground()
        {
            if (_imagePannel == null || _pickup == null)
            {
                Debug.LogWarning("Необходимые компоненты не назначены.");
                return;
            }

            Image backgroundPannelImage = _imagePannel.GetComponent<Image>();
            if (backgroundPannelImage == null || !AssembledPickups.ContainsPrefab(_pickup))
            {
                Debug.LogWarning("У панели отсутствует компонент Image.");
                return;
            }

            // Если указан путь в инспекторе
            if (!string.IsNullOrEmpty(_spritePath))
            {
                LoadSpriteFromResources(backgroundPannelImage);
            }
            else
            {
                // Стандартная логика из оригинального скрипта
                UseLocalImageComponent(backgroundPannelImage);
            }
        }

        private void LoadSpriteFromResources(Image targetImage)
        {
            Sprite loadedSprite = Resources.Load<Sprite>(_spritePath);

            if (loadedSprite != null)
            {
                targetImage.sprite = loadedSprite;
                Debug.Log($"Спрайт успешно загружен: {_spritePath}");
            }
            else
            {
                Debug.LogError($"Не удалось загрузить спрайт по пути: Resources/{_spritePath}");
                // Возвращаемся к стандартной логике, если загрузка не удалась
                UseLocalImageComponent(targetImage);
            }
        }

        private void UseLocalImageComponent(Image targetImage)
        {
            Image localImage = transform.Find("Image")?.GetComponent<Image>();

            if (localImage != null && localImage.sprite != null)
            {
                targetImage.sprite = localImage.sprite;
            }
            else
            {
                Debug.LogWarning($"Не найден компонент Image или спрайт на объекте {gameObject.name}");
                targetImage.sprite = null;
            }
        }
    }
}