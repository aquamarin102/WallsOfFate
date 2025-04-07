using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class Repository
{
    private const string GAME_STATE_FILE = "GameState.json";

    private static Dictionary<string, string> currentState = new();

    private static string FilePath => Path.Combine(Application.persistentDataPath, GAME_STATE_FILE);

    public static void LoadState()
    {
        if (File.Exists(FilePath))
        {
            var serializedState = File.ReadAllText(FilePath);
            currentState = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedState) ?? new Dictionary<string, string>();
        }
        else
        {
            currentState = new Dictionary<string, string>();
        }
    }

    public static void SaveState()
    {
        var serializedState = JsonConvert.SerializeObject(currentState, Formatting.Indented);
        File.WriteAllText(FilePath, serializedState);
    }

    public static T GetData<T>()
    {
        if (currentState.TryGetValue(typeof(T).Name, out var serializedData))
        {
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        throw new KeyNotFoundException($"Data for type {typeof(T).Name} not found.");
    }

    public static void SetData<T>(T value)
    {
        var serializedData = JsonConvert.SerializeObject(value);
        currentState[typeof(T).Name] = serializedData;
    }

    public static bool TryGetData<T>(out T value)
    {
        if (currentState.TryGetValue(typeof(T).Name, out var serializedData))
        {
            value = JsonConvert.DeserializeObject<T>(serializedData);
            return true;
        }

        value = default;
        return false;
    }
}
