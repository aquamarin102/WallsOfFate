using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItemSaveLoader : ISaveLoader
{
    public bool LoadData() { if (Repository.TryGetData("InteractableItemCollection", out Dictionary<string, bool> itemStates)) { InteractableItemCollection.LoadItemStates(itemStates); Debug.Log($"Loaded {itemStates.Count} interactable item states."); return true; } return false; }

    public void LoadDefaultData()
    {
        InteractableItemCollection.LoadItemStates(new Dictionary<string, bool>());
        Debug.Log("Loaded default (empty) interactable item states.");
    }

    public void SaveData()
    {
        var itemStates = InteractableItemCollection.GetAllItemStates();
        Repository.SetData("InteractableItemCollection", itemStates);
        Debug.Log($"Saved {itemStates.Count} interactable item states.");
    }

}