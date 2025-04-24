using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLogicEnd : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private List<GameObject> pickupPanels;
    [SerializeField] private string _pickupType;

    [Header("Button Settings")]
    [SerializeField] private GameObject _buttonObject; // Ссылка на объект-кнопку

    private List<Pickup> _currentPickupsOfType = new List<Pickup>();
    private int _displayedImagesCount = 0;

    private void Update()
    {
        if (string.IsNullOrEmpty(_pickupType)) return;

        var newPickups = AssembledPickups.GetPickupsByType(_pickupType).ToList();

        if (!PickupListsEqual(_currentPickupsOfType, newPickups))
        {
            _currentPickupsOfType = newPickups;
            UpdatePanelsVisibility();
            CheckAllImagesDisplayed();
        }
    }

    private void UpdatePanelsVisibility()
    {
        _displayedImagesCount = 0;

        foreach (var panel in pickupPanels)
        {
            if (panel == null) continue;

            var imageTransform = panel.transform.Find("Image");
            var panelPickup = panel.GetComponent<Pickup>();

            bool shouldDisplay = panelPickup != null &&
                               AssembledPickups.ContainsPrefab(panelPickup);

            if (imageTransform != null)
            {
                imageTransform.gameObject.SetActive(shouldDisplay);
                if (shouldDisplay) _displayedImagesCount++;
            }
        }
    }

    private void CheckAllImagesDisplayed()
    {
        if (_buttonObject == null) return;

        // Находим первый неотрендеренный пикап
        var nonRenderedPickup = FindFirstNonRenderedPickup(_pickupType);

        // Если все пикапы отрендерены и есть что показывать
        bool allDisplayed = nonRenderedPickup == null && _displayedImagesCount >= 3;
        _buttonObject.SetActive(allDisplayed);
    }

    public Pickup FindFirstNonRenderedPickup(string pickupType)
    {
        return AssembledPickups.GetPickupsByType(pickupType + "Conc")
            .FirstOrDefault(p => !p.Rendered);
    }

    public void RefreshPanel()
    {
        _displayedImagesCount = 0;
        _buttonObject.SetActive(false);
        foreach(var panel in pickupPanels)
        {
            var imageTransform = panel.transform.Find("Image");
            var indicationTransform = panel.transform.Find("Indication");
            imageTransform.gameObject.SetActive(false);
            indicationTransform.gameObject.SetActive(false);
            Pickup pannelPickup = panel.GetComponent<Pickup>();
            AssembledPickups.RemovePickupByName(pannelPickup.Name);
        }
    }

    public void ChangeVisibility(string pickupType)
    {
        Pickup pickup = FindFirstNonRenderedPickup(pickupType);
        pickup.Rendered = true;
        RefreshPanel();
    }

    private void UpdatePanelFromPickup(GameObject panel, Pickup pickup)
    {
        var panelPickup = panel.GetComponent<Pickup>();
        var panelImage = panel.transform.Find("Image")?.GetComponent<Image>();

        if (panelPickup == null) return;

        // Копируем основные данные
        CopyPickupData(panelPickup, pickup);

        // Устанавливаем изображение
        if (!string.IsNullOrEmpty(pickup.Picture) && panelImage != null)
        {
            LoadAndSetSprite(pickup.Picture, panelImage);
        }

        panelPickup.Display();
    }

    private void CopyPickupData(Pickup target, Pickup source)
    {
        target.Name = source.Name;
        target.Type = source.Type;
        target.Description = source.Description;
        target.HideDescription = source.HideDescription;
        target.Picture = source.Picture;
        target.Rendered = source.Rendered;
        target.RenderedOnScreen = source.RenderedOnScreen;

        // Копируем словари
        target.SimpleDict.Clear();
        foreach (var pair in source.SimpleDict)
        {
            target.SimpleDict.Add(pair.Key, pair.Value);
        }
    }

    private void LoadAndSetSprite(string path, Image targetImage)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(path);
        if (loadedSprite != null)
        {
            targetImage.sprite = loadedSprite;
            targetImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Не удалось загрузить изображение: {path}");
        }
    }

    private bool PickupListsEqual(List<Pickup> list1, List<Pickup> list2)
    {
        if (list1.Count != list2.Count) return false;
        return !list1.Where((t, i) => t != list2[i]).Any();
    }

    public Pickup GetCurrentPickup()
    {
        return pickupPanels
            .Where(p => p != null && p.activeSelf)
            .Select(p => p.GetComponent<Pickup>())
            .FirstOrDefault();
    }
}