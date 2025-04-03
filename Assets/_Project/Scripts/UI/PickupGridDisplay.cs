using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PickupGridDisplay : MonoBehaviour
{
    [SerializeField] private GameObject pickupUIPrefab; // Префаб для отображения пикапа
    [SerializeField] private RectTransform contentPanel; // Панель, на которой будут отображаться пикапы
    [SerializeField] private Vector2 gridSpacing = new Vector2(10, 10); // Отступы между элементами сетки

    private List<GameObject> pickupUIElements = new List<GameObject>();
    private int lastPickupCount = 0; // Переменная для хранения последнего известного количества пикапов

    private void OnEnable()
    {
        RefreshGrid();
        lastPickupCount = AssembledPickups.GetRenderedPickups().Count; // Инициализируем начальное количество
    }

    private void Update()
    {
        // Получаем текущее количество пикапов
        int currentPickupCount = AssembledPickups.GetRenderedPickups().Count;

        // Если количество изменилось
        if (currentPickupCount != lastPickupCount)
        {
            RefreshGrid();  // Обновляем сетку
            lastPickupCount = currentPickupCount; // Обновляем сохраненное значение
        }
    }

    public void RefreshGrid()
    {
        // Очищаем предыдущие элементы
        foreach (var element in pickupUIElements)
        {
            Destroy(element);
        }
        pickupUIElements.Clear();

        // Принудительно обновляем макет
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);

        // Получаем все пикапы
        List<Pickup> pickups = AssembledPickups.GetRenderedPickups();

        // Получаем размеры родительской панели и префаба
        RectTransform prefabRect = pickupUIPrefab.GetComponent<RectTransform>();
        float prefabWidth = prefabRect.rect.width;
        float prefabHeight = prefabRect.rect.height;

        float panelWidth = contentPanel.rect.width;
        float panelHeight = contentPanel.rect.height;

        // Проверяем, что размеры не равны нулю
        if (prefabWidth <= 0 || prefabHeight <= 0 || panelWidth <= 0 || panelHeight <= 0)
        {
            Debug.LogError("Размеры contentPanel или pickupUIPrefab равны нулю. Проверьте настройки RectTransform.");
            return;
        }

        // Рассчитываем количество столбцов и строк
        int gridColumns = Mathf.FloorToInt((panelWidth + gridSpacing.x) / (prefabWidth + gridSpacing.x));
        int gridRows = Mathf.CeilToInt((float)pickups.Count / gridColumns);
        if (gridColumns == 0 || gridRows == 0)
        {
            prefabRect = FindParentWithName(contentPanel, "View").GetComponent<RectTransform>();
            panelWidth = prefabRect.rect.width;
            panelHeight = prefabRect.rect.height;

            gridColumns = Mathf.FloorToInt((panelWidth + gridSpacing.x) / (prefabWidth + gridSpacing.x));
            gridRows = Mathf.CeilToInt((float)pickups.Count / gridColumns);
        }

        // Смещение для учета pivot contentPanel (0.5, 0.5)
        float offsetX = -panelWidth / 2 + prefabWidth / 2; // Смещение по X для начала с левого края
        float offsetY = panelHeight / 2 - prefabHeight / 2; // Смещение по Y для начала с верхнего края

        // Смещение для учета pivot префаба (0.5, 0.5)
        float prefabOffsetX = prefabWidth / 2; // Смещение по X для левого верхнего угла префаба
        float prefabOffsetY = -prefabHeight / 2; // Смещение по Y для левого верхнего угла префаба

        // Создаем сетку
        for (int i = 0; i < pickups.Count; i++)
        {
            // Создаем новый элемент UI
            GameObject pickupUI = Instantiate(pickupUIPrefab, contentPanel);
            pickupUIElements.Add(pickupUI);

            // Настраиваем позицию элемента в сетке
            RectTransform rectTransform = pickupUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(prefabWidth, prefabHeight);

            int row = i / gridColumns;
            int col = i % gridColumns;

            // Рассчитываем позицию с учетом смещений
            float x = offsetX + prefabOffsetX + col * (prefabWidth + gridSpacing.x);
            float y = offsetY + prefabOffsetY - row * (prefabHeight + gridSpacing.y);

            rectTransform.anchoredPosition = new Vector2(x, y);

            // Настраиваем отображение пикапа (имя и иконку)
            SetPickupUI(pickupUI, pickups[i]);
        }
    }

    private void SetPickupUI(GameObject pickupUI, Pickup pickup)
    {
        // Получаем компонент Image у панели (для установки фона)
        Image backgroundImage = pickupUI.GetComponent<Image>();
        pickupUI.name = pickup.Name;
        // Получаем компонент TMP_Text внутри панели
        //TMP_Text nameText = pickupUI.GetComponentInChildren<TMP_Text>();

        if (backgroundImage != null /*&& nameText != null*/) // Убедимся, что компоненты существуют
        {
            // Загружаем спрайт из папки Resources
            Sprite loadedSprite = Resources.Load<Sprite>(pickup.Picture);

            if (loadedSprite != null)
            {
                backgroundImage.sprite = loadedSprite; // Устанавливаем фон панели
            }
            else
            {
                Debug.LogWarning($"Sprite not found at path: {pickup.Picture}");
                //backgroundImage.sprite = null; // Очищаем фон, если спрайт не найден
            }

            // Устанавливаем имя пикапа в текстовый компонент
            //nameText.text = pickup.Name;
        }
        else
        {
            Debug.LogWarning("Не удалось найти компонент Image или TMP_Text в префабе.");
        }
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