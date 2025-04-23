using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLigiEnd : MonoBehaviour
{
    [SerializeField] private List<GameObject> pickupPanels;
    [SerializeField] private string _pickupType;
    //[SerializeField] private List<Image> panelImages;

    private List<Pickup> _currentPickupsOfType = new List<Pickup>();

    private void Update()
    {
        if (string.IsNullOrEmpty(_pickupType)) return;

        IEnumerable<Pickup> pickupsEnumerable = AssembledPickups.GetPickupsByType(_pickupType);
        List<Pickup> newPickups = pickupsEnumerable.ToList();

        if (!PickupListsEqual(_currentPickupsOfType, newPickups))
        {
            _currentPickupsOfType = newPickups;
            UpdatePanelsVisibility();
        }
    }

    private void UpdatePanelsVisibility()
    {
        // Сначала деактивируем все панели
        foreach (var panel in pickupPanels)
        {
            var pannelImage = panel.gameObject.transform.Find("Image").gameObject;
            pannelImage.SetActive(false);
        }

        // Для каждого пикапа в AssembledPickups
        foreach (var pickup in _currentPickupsOfType)
        {
            // Ищем панель, у которой компонент Pickup совпадает с текущим пикапом
            var matchingPanel = pickupPanels.FirstOrDefault(panel =>
                panel.GetComponent<Pickup>() == pickup);

            if (matchingPanel != null)
            {
                var pannelImage = matchingPanel.gameObject.transform.Find("Image").gameObject;
                pannelImage.SetActive(true);
                //UpdatePanelFromPickup(matchingPanel, pickup);
            }
        }
    }

    private void UpdatePanelFromPickup(GameObject panel, Pickup pickup)
    {
        var panelPickup = panel.GetComponent<Pickup>();
        var panelImage = panel.transform.Find("Image")?.GetComponent<Image>();

        if (panelPickup == null) return;

        // Копируем данные
        panelPickup.Name = pickup.Name;
        panelPickup.Type = pickup.Type;
        panelPickup.Description = pickup.Description;
        panelPickup.HideDescription = pickup.HideDescription;
        panelPickup.Picture = pickup.Picture;
        panelPickup.Rendered = pickup.Rendered;
        panelPickup.RenderedOnScreen = pickup.RenderedOnScreen;

        // Копируем словари
        panelPickup.SimpleDict.Clear();
        foreach (var pair in pickup.SimpleDict)
        {
            panelPickup.SimpleDict.Add(pair.Key, pair.Value);
        }

        // Устанавливаем изображение
        if (!string.IsNullOrEmpty(pickup.Picture) && panelImage != null)
        {
            Sprite loadedSprite = Resources.Load<Sprite>(pickup.Picture);
            if (loadedSprite != null)
            {
                panelImage.sprite = loadedSprite;
                panelImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Не удалось загрузить изображение: {pickup.Picture}");
            }
        }

        panelPickup.Display();
    }

    private bool PickupListsEqual(List<Pickup> list1, List<Pickup> list2)
    {
        if (list1.Count != list2.Count) return false;
        return !list1.Where((t, i) => t != list2[i]).Any();
    }

    public Pickup GetCurrentPickup()
    {
        var activePanel = pickupPanels.FirstOrDefault(p => p.activeSelf);
        return activePanel?.GetComponent<Pickup>();
    }
}