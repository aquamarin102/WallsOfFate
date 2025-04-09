using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Quest
{
    public class QuestSaveLoader : ISaveLoader
    {
        //private List<Quest> _quests;

        //public QuestSaveLoader(List<Quest> quests)
        //{
        //    _quests = quests;
        //}

        public void SaveData()
        {
            List<QuestData> saveData = QuestCollection.GetAllQuests()
                .Select(q => QuestData.FromQuest(q))
                .ToList();

            // Используем уникальный ключ для квестов
            Repository.SetData("Quests", saveData);
            Debug.Log($"Saved {saveData.Count} quests");
        }

        public bool LoadData()
        {
            // Загружаем по уникальному ключу
            if (Repository.TryGetData("Quests", out List<QuestData> savedQuests))
            {
                QuestCollection.ClearQuests();
                foreach (var questData in savedQuests)
                {
                    QuestCollection.AddQuest(questData.ToQuest());
                }
                return true;
            }
            return false;
        }

        public void LoadDefaultData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/Quests/DefaultQuests");
            if (textAsset == null)
            {
                Debug.LogError("File not found: Resources/SavsInformation/Quests/DefaultQuests");
                return;
            }

            try
            {
                List<QuestData> defaultQuests = JsonConvert.DeserializeObject<List<QuestData>>(textAsset.text);
                QuestCollection.ClearQuests();

                foreach (var questData in defaultQuests)
                {
                    QuestCollection.AddQuest(questData.ToQuest());
                }
                Debug.Log($"Loaded {defaultQuests.Count} default quests");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Load error: {ex.Message}");
            }
        }
    }

    [System.Serializable]
    public class QuestData
    {
        public int Id;
        public string QuestInfo;
        public bool IsDone;

        public Quest ToQuest() => new Quest(Id, QuestInfo, IsDone);

        public static QuestData FromQuest(Quest quest) => new QuestData
        {
            Id = quest.Id,
            QuestInfo = quest.QuestInfo,
            IsDone = quest.IsDone
        };
    }

}
