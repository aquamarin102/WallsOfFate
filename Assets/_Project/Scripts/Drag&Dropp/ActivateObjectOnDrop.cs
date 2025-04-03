using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Drag_Dropp
{
    public class ActivateObjectOnDrop : MonoBehaviour
    {
        [SerializeField] private ItemSlot itemSlot; // Ссылка на компонент ItemSlot
        [SerializeField] private GameObject objectToActivate; // Объект, который нужно включить при срабатывании события

        private RectTransform _objectRectTransform; // RectTransform объекта objectToActivate
        private Vector2 _initialSize; // Изначальный размер объекта

        private void OnEnable()
        {
            itemSlot = GetComponent<ItemSlot>();
            // Подписываемся на событие OnDropEvent
            if (itemSlot != null)
            {
                itemSlot.OnDropEvent += HandleDropEvent;
            }

            _initialSize = GetComponent<RectTransform>().sizeDelta; // Сохраняем изначальный размер
            // Получаем RectTransform объекта objectToActivate
            if (objectToActivate != null)
            {
                _objectRectTransform = objectToActivate.GetComponent<RectTransform>(); 
            }
            //if (!objectToActivate.activeSelf)
            //{
            //    // Если объект неактивен, растягиваем его на всю незанятую ширину родительского объекта
            //    StretchObject();
            //}
        }

        private void OnDisable()
        {
            // Отписываемся от события OnDropEvent при отключении объекта
            if (itemSlot != null)
            {
                itemSlot.OnDropEvent -= HandleDropEvent;
            }
        }

        private void HandleDropEvent(PointerEventData eventData, string slotName)
        {
            if (objectToActivate == null || _objectRectTransform == null)
                return;

            // Проверяем состояние объекта
            //if (objectToActivate.activeSelf)
            //{
            //    // Если объект неактивен, растягиваем его на всю незанятую ширину родительского объекта
            //    ResetObject();
            //}

            // Включаем объект
            objectToActivate.SetActive(true);
        }

        private void StretchObject()
        {
            // Получаем RectTransform родительского объекта
            RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();
            if (parentRectTransform == null)
                return;

            // Вычисляем доступную ширину для растяжения
            float parentWidth = parentRectTransform.rect.width;
            float thisWidth = GetComponent<RectTransform>().rect.width;
            float stretchWidth = parentWidth - thisWidth;

            // Если доступная ширина меньше или равна нулю, ничего не делаем
            if (stretchWidth <= 0)
                return;

            // Увеличиваем ширину объекта, сохраняя его позицию
            GetComponent<RectTransform>().sizeDelta = new Vector2(
                _initialSize.x + stretchWidth, // Новая ширина
                _initialSize.y // Высота остается неизменной
            );
        }

        private void ResetObject()
        {
            // Возвращаем объект к изначальным размерам
            GetComponent<RectTransform>().sizeDelta = _initialSize;
        }
    }
}