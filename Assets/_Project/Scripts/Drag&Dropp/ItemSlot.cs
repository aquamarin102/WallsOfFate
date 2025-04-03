using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Drag_Dropp
{
    internal class ItemSlot : MonoBehaviour, IDropHandler
    {
        public event System.Action<PointerEventData, string> OnDropEvent; // Добавлен параметр string

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
            if (eventData.pointerDrag != null)
            {
                // Получаем копию перетаскиваемого объекта
                GameObject dragDropObject = eventData.pointerDrag.GetComponent<DragDropp>().GetDragCopy();
                if (dragDropObject == null)
                {
                    Debug.LogWarning("DragCopy is null.");
                    return;
                }

                // Проверяем, есть ли уже дочерний объект с именем, оканчивающимся на "(clone)"
                foreach (Transform child in transform)
                {
                    if (child.name.EndsWith("(Clone)"))
                    {
                        // Удаляем существующий дочерний объект
                        Destroy(child.gameObject);
                        break; // Удаляем только один объект
                    }
                }

                // Устанавливаем текущий объект (слот) как родителя для копии
                dragDropObject.transform.SetParent(transform);

                // Получаем RectTransform перетаскиваемого объекта
                RectTransform draggedRectTransform = dragDropObject.GetComponent<RectTransform>();

                // Получаем дочерний объект слота с именем "DragObjPlace"
                Transform childObject = transform.Find("DragObjPlace");

                // Проверяем, что дочерний объект найден
                if (childObject == null)
                {
                    Debug.LogWarning("Child object 'DragObjPlace' not found.");
                    return;
                }

                // Если дочерний объект отключен, включаем его
                if (!childObject.gameObject.activeSelf)
                {
                    childObject.gameObject.SetActive(true);
                }

                // Получаем RectTransform дочернего объекта
                RectTransform childRectTransform = childObject.GetComponent<RectTransform>();

                // Преобразуем локальные координаты дочернего объекта в глобальные (мировые)
                Vector3 globalPosition = childRectTransform.TransformPoint(childRectTransform.rect.center);

                // Устанавливаем глобальную позицию перетаскиваемого объекта
                draggedRectTransform.position = globalPosition;

                // Устанавливаем размер перетаскиваемого объекта равным размеру дочернего объекта
                draggedRectTransform.sizeDelta = childRectTransform.sizeDelta;

                // Вызываем событие OnDropEvent и передаем имя объекта, на котором висит скрипт
                OnDropEvent?.Invoke(eventData, gameObject.name);
            }
        }
    }
}