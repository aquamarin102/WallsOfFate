// QuestCollection и связанные классы
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Quest
{
    [System.Serializable]
    public struct QuestResources
    {
        public int Gold;
        public int Food;
        public int PeopleSatisfaction;
        public int CastleStrength;
    }

    [System.Serializable]
    public class QuestSaveData
    {
        public int CurrentDay;
        public List<DayData> Days = new List<DayData>();
    }

    public static class QuestCollection
    {
        private static QuestSaveData _saveData = new QuestSaveData();

        public static int CurrentDayNumber
        {
            get => _saveData.CurrentDay;
            private set => _saveData.CurrentDay = value;
        }

        public static void Initialize(QuestSaveData saveData = null)
        {
            _saveData = saveData ?? new QuestSaveData();
            if (_saveData.Days == null) _saveData.Days = new List<DayData>();
        }

        public static void AddDay(DayData day)
        {
            _saveData.Days.Add(day);
            CurrentDayNumber = day.Day;
        }

        public static void ClearQuests() => _saveData.Days.Clear();
        public static List<DayData> GetAllDays() => new List<DayData>(_saveData.Days);

        public static void IncreaseCurrentDay()
        {
            CurrentDayNumber++;
        }

        public static DayData GetCurrentDayData()
        {
            return _saveData.Days.FirstOrDefault(d => d.Day == CurrentDayNumber);
        }

        public static List<QuestGroup> GetActiveQuestGroups()
        {
            var currentDay = GetCurrentDayData();
            return currentDay?.Quests.Where(q => q.InProgress && !q.Complite).ToList()
                   ?? new List<QuestGroup>();
        }

        public static List<QuestGroup> GetAllQuestGroups()
        {
            var currentDay = GetCurrentDayData();
            return currentDay.Quests ?? new List<QuestGroup>();
        }

        public static string SaveToJson()
        {
            return JsonConvert.SerializeObject(_saveData, Formatting.Indented);
        }

        public static void LoadFromJson(string json)
        {
            _saveData = JsonConvert.DeserializeObject<QuestSaveData>(json);
        }

        public static void ForceCompleteQuest(int questId, bool shouldComplete)
        {
            if (!shouldComplete) return;

            var allDays = GetAllDays();
            var day = _saveData.Days[CurrentDayNumber];
            var quest = day.Quests.FirstOrDefault(q => q.Id == questId);
            if (quest != null)
            {
                // Завершаем квест
                quest.Complite = true;
                quest.InProgress = false;
            }
        }
    }


    [System.Serializable]
    public class DayData
    {
        public int Day;
        public List<QuestGroup> Quests = new List<QuestGroup>();
    }

    [System.Serializable]
    public class QuestGroup
    {
        public int Id;
        public bool InProgress;
        public bool Complite;
        public string OpenNPS;
        public String OpenDialog;
        public Evidence Evidence;
        public int CurrentTaskId;
        public bool Prime;
        public List<QuestTask> Tasks = new List<QuestTask>();

        public bool CheckOpen(string npcName)
        {
            return OpenNPS == npcName && !InProgress && !Complite;
        }

        public bool IsEneded()  { return !InProgress && Complite; }

        public void StartQuest()
        {
            InProgress = true;
            CurrentTaskId = Tasks.FirstOrDefault()?.Id ?? -1;
        }
        public void CopyFrom(QuestGroup source)
        {
            if (source == null) return;

            // Копируем простые поля
            Id = source.Id;
            InProgress = source.InProgress;
            Complite = source.Complite;
            OpenNPS = source.OpenNPS;
            OpenDialog = source.OpenDialog;
            CurrentTaskId = source.CurrentTaskId;
            Prime = source.Prime;

            // Копируем Evidence (с проверкой на null)
            if (source.Evidence != null)
            {
                Evidence = new Evidence
                {
                    EvidenceType = source.Evidence.EvidenceType,
                    Dialoge = source.Evidence.Dialoge,
                    DialogePlayed = source.Evidence.DialogePlayed,
                    Picture = source.Evidence.Picture,
                    Description = source.Evidence.Description
                };
            }
            else
            {
                Evidence = null;
            }

            // Копируем список Tasks (с глубоким копированием)
            Tasks = source.Tasks.Select(task => new QuestTask
            {
                Id = task.Id,
                TaskInfo = task.TaskInfo,
                IsDone = task.IsDone,
                ForNPS = task.ForNPS,
                RequeredTasksIds = task.RequeredTasksIds?.ToArray(), // Копируем массив
                RequeredDialog = task.RequeredDialog,
                Resources = new QuestResources
                {
                    Gold = task.Resources.Gold,
                    Food = task.Resources.Food,
                    PeopleSatisfaction = task.Resources.PeopleSatisfaction,
                    CastleStrength = task.Resources.CastleStrength
                }
            }).ToList();
        }

          public void TryCompleteGroup()
    {
        if (Tasks.All(t => t.IsDone))
        {
            Complite = true;
            InProgress = false;
            // можно тут ещё отправить событие, лог, звук и т.п.
        }
    }

        public int GetCurrentTaskId() => CurrentTaskId;
        public QuestTask GetCurrentTask() => Tasks[CurrentTaskId];
    }

    [System.Serializable]
    public class Evidence
    {
        public string EvidenceType;
        public string Dialoge;
        public bool DialogePlayed;
        public string Picture;
        public string Description;
    }

    [System.Serializable]
    public class QuestTask
    {
        public int Id;
        public string TaskInfo;
        public bool IsDone;
        public string ForNPS;
        public int[] RequeredTasksIds;
        public string RequeredDialog;
        public QuestResources Resources;

        public void CompleteTask()
        {
            if (IsDone || !CanComplete()) return;

            IsDone = true;
            ApplyResources();
        }

        public bool CanComplete()
        {
            return RequeredTasksIds.All(id =>
                QuestCollection.GetAllDays()
                    .SelectMany(d => d.Quests)
                    .SelectMany(q => q.Tasks)
                    .Any(t => t.Id == id && t.IsDone)) && !this.IsDone;
        }

        private void ApplyResources()
        {
            GameResources.GameResources.ChangeGold(Resources.Gold);
            GameResources.GameResources.ChangeFood(Resources.Food);
            GameResources.GameResources.ChangePeopleSatisfaction(Resources.PeopleSatisfaction);
            GameResources.GameResources.ChangeCastleStrength(Resources.CastleStrength);
        }
    }
}