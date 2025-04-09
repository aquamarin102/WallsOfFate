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

    public static void SetData<T>(string key, T value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All // Добавляем информацию о типе
        };
        var serializedData = JsonConvert.SerializeObject(value, settings);
        currentState[key] = serializedData;
    }

    public static bool TryGetData<T>(string key, out T value)
    {
        if (currentState.TryGetValue(key, out var serializedData))
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            value = JsonConvert.DeserializeObject<T>(serializedData, settings);
            return true;
        }
        value = default;
        return false;
    }

    public static bool HasAnyData()
    {
        return currentState.Count > 0;
    }

    public static void ClearSaveData()
    {
        try
        {
            // Очищаем данные в памяти
            currentState.Clear();

            // Удаляем файл сохранения, если он существует
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log("Save file deleted successfully");
            }

            // Создаем новый пустой словарь для последующих операций
            currentState = new Dictionary<string, string>();
        }
        catch (IOException ex)
        {
            Debug.LogError($"Error clearing save data: {ex.Message}");
        }
    }
}
