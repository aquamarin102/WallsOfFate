using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public class QuestSaveLoader : ISaveLoader
    {
        public bool LoadData()
        {
            if (Repository.TryGetData("QuestSchedule", out List<DayData> saveData))
            {
                QuestCollection.ClearQuests();
                foreach (var day in saveData)
                {
                    QuestCollection.AddDay(day);
                }
                Debug.Log($"Loaded {saveData.Count} days");
                return true;
            }
            return false;
        }

        public void LoadDefaultData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/Quests/DefaultQuests");
            if (textAsset == null)
            {
                Debug.LogError("Default quests file not found!");
                return;
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                var defaultData = JsonConvert.DeserializeObject<List<DayData>>(textAsset.text, settings);
                QuestCollection.ClearQuests();
                foreach (var day in defaultData)
                {
                    QuestCollection.AddDay(day);
                }
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}");
            }
        }

        public void SaveData()
        {
            Repository.SetData("QuestSchedule", QuestCollection.GetAllDays());
            Debug.Log($"Saved {QuestCollection.GetAllDays().Count} days");
        }
    }
}