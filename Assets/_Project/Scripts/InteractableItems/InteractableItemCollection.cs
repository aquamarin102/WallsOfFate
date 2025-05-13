using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class InteractableItemCollection
{
    private static Dictionary<string, bool> _itemStates = new Dictionary<string, bool>();

    // Generate a unique key for an item based on scene and object name
    public static string GetItemKey(string sceneName, string objectName)
    {
        return $"{sceneName}_{objectName}";
    }

    // Add or update an item's state
    public static void SetItemState(string sceneName, string objectName, bool hasBeenUsed)
    {
        string key = GetItemKey(sceneName, objectName);
        _itemStates[key] = hasBeenUsed;
    }

    // Get an item's state, returns false if not found
    public static bool TryGetItemState(string sceneName, string objectName, out bool hasBeenUsed)
    {
        string key = GetItemKey(sceneName, objectName);
        return _itemStates.TryGetValue(key, out hasBeenUsed);
    }

    // Get all item states for saving
    public static Dictionary<string, bool> GetAllItemStates()
    {
        return new Dictionary<string, bool>(_itemStates);
    }

    // Load item states from saved data
    public static void LoadItemStates(Dictionary<string, bool> states)
    {
        _itemStates = states ?? new Dictionary<string, bool>();
    }

    // Clear all item states (e.g., for new game)
    public static void Clear()
    {
        _itemStates.Clear();
    }

}