using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public class QuestSaveLoader : ISaveLoader
    {
        public bool LoadData()
        {
            if (Repository.TryGetData("QuestSchedule", out QuestSaveData saveData))
            {
                QuestCollection.Initialize(saveData);
                Debug.Log($"Loaded quest data for day {saveData.CurrentDay}");
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
                    MissingMemberHandling = MissingMemberHandling.Ignore // Изменили на Ignore для большей устойчивости
                };

                var defaultData = JsonConvert.DeserializeObject<QuestSaveData>(textAsset.text, settings);

                if (defaultData == null)
                {
                    Debug.LogError("Failed to deserialize default quest data");
                    return;
                }

                QuestCollection.Initialize(defaultData);
                Debug.Log($"Loaded default quest data for day {defaultData.CurrentDay} with {defaultData.Days.Count} days");
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void SaveData()
        {
            var saveData = new QuestSaveData
            {
                CurrentDay = QuestCollection.CurrentDayNumber,
                Days = QuestCollection.GetAllDays()
            };

            Repository.SetData("QuestSchedule", saveData);
            Debug.Log($"Saved quest data for day {saveData.CurrentDay} with {saveData.Days.Count} days");
        }
    }
}