using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionSaveLoader : ISaveLoader
{
    private List<Pickup> _pickups;

    public CollectionSaveLoader(List<Pickup> pickups)
    {
        _pickups = pickups;
    }

    public void LoadData()
    {
        if (Repository.TryGetData(out List<PickupData> savedPickups))
        {
            //AssembledPickups.Clear();
            foreach (var pickup in savedPickups)
            {
                AssembledPickups.AddPickup(pickup.ToPickup());
            }
            Debug.Log("Collection loaded: " + savedPickups.Count + " items.");
            foreach (var pickup in savedPickups)
            {
                Debug.Log($"Pickup {pickup.Name}");
            }
        }
        //else
        //    LoadDefaulData();
    }

    public void LoadDefaulData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/Inventory/Conclusions");
        if (textAsset == null)
        {
            Debug.LogError("File not found: Resources/SavsInformation/Inventory/Conclusions");
            return;
        }

        try
        {
            // Десериализуем напрямую в список объектов
            List<PickupData> savedPickups = JsonConvert.DeserializeObject<List<PickupData>>(textAsset.text);

            if (savedPickups != null && savedPickups.Count > 0)
            {
                LoadPickups(savedPickups);
                Debug.Log($"Successfully loaded {savedPickups.Count} items");
            }
            else
            {
                Debug.LogWarning("Loaded list is empty or null");
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON error: {ex.Message}\nJSON content: {textAsset.text}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"General error: {ex.Message}");
        }
    }

    private void LoadPickups(List<PickupData> savedPickups)
    {
        // Очистка текущих пикапов (если нужно)
        //AssembledPickups.Clear();

        // Добавление загруженных пикапов
        foreach (var pickup in savedPickups)
        {
            AssembledPickups.AddPickup(pickup.ToPickup());
        }

        Debug.Log("Collection loaded: " + savedPickups.Count + " items.");
        foreach (var pickup in savedPickups)
        {
            Debug.Log($"Pickup {pickup.Name}");
        }
    }

    public void SaveData()
    {
        _pickups = AssembledPickups.GetAllPickups();
        var pickupData = _pickups.Select(PickupData.FromPickup).ToList();
        Repository.SetData(pickupData);
        Debug.Log("Collection saved: " + _pickups.Count + " items.");
    }
}