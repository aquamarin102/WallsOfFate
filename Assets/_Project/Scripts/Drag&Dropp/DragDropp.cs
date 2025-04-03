using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts.Drag_Dropp
{
    internal class DragDropp : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Canvas canvas;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private GameObject dragCopy; // Копия объекта для перетаскивания

        private bool destroible;

        public GameObject GetDragCopy()
        {
            destroible = false;
            return dragCopy;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>(); // Инициализируем на текущем объекте
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            destroible = true;

            Transform itemsParent = FindParentWithName(transform, "Items");
            //if (itemsParent != null)
            //{
            //    // Устанавливаем родителя для копии
            //    dragCopy.transform.SetParent(itemsParent, false);
            //}

            // Создаем копию объекта
            dragCopy = Instantiate(gameObject, transform.position, transform.rotation, itemsParent);

            // Настраиваем копию
            RectTransform copyRectTransform = dragCopy.GetComponent<RectTransform>();
            CanvasGroup copyCanvasGroup = dragCopy.GetComponent<CanvasGroup>();

            // Устанавливаем настройки для копии
            copyRectTransform.sizeDelta = rectTransform.sizeDelta;
            copyCanvasGroup.alpha = 0.6f;
            copyCanvasGroup.blocksRaycasts = false;

            // Отключаем блокировку лучей у оригинала
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");
            if (dragCopy != null)
            {
                // Перемещаем копию
                RectTransform copyRectTransform = dragCopy.GetComponent<RectTransform>();
                copyRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");

            CanvasGroup copyCanvasGroup = dragCopy.GetComponent<CanvasGroup>();

            // Уничтожаем копию
            if (dragCopy != null && destroible)
            {
                Destroy(dragCopy);
            }

            // Восстанавливаем блокировку лучей у оригинала
            canvasGroup.blocksRaycasts = true;
            copyCanvasGroup.alpha = 1f;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
        }

        private Transform FindParentWithName(Transform current, string name)
        {
            // Поднимаемся вверх по иерархии
            while (current != null)
            {
                if (current.name == name)
                {
                    return current;
                }
                current = current.parent;
            }
            return null; // Если элемент не найден
        }
    }
}