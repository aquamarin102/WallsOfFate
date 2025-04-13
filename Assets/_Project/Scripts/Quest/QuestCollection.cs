// QuestCollection и связанные классы
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public static class QuestCollection
    {
        private static List<DayData> Days = new List<DayData>();

        public static void AddDay(DayData day) => Days.Add(day);
        public static void ClearQuests() => Days.Clear();
        public static List<DayData> GetAllDays() => new List<DayData>(Days);

        public static List<QuestGroup> GetActiveQuestGroups()
        {
            return Days
                .SelectMany(d => d.Quests)
                .Where(q => q.InProgress && !q.Complite)
                .ToList();
        }

        public static QuestTask GetCurrentTaskForGroup(QuestGroup group)
        {
            QuestTask currentTask = group.Tasks.First(t => t.Id == group.CurrentTaskId && !t.IsDone);
            return currentTask;
        }

        public static void UpdateQuestGroup(QuestGroup updatedGroup)
        {
            // Итерируем по всем дням
            foreach (var day in Days)
            {
                // Ищем группу с нужным ID в текущем дне
                var index = day.Quests.FindIndex(g => g.Id == updatedGroup.Id);
                if (index != -1)
                {
                    // Заменяем старую версию группы на обновленную
                    day.Quests[index] = updatedGroup;
                    return; // Предполагаем уникальность ID групп
                }
            }

            Debug.LogWarning($"Quest group with ID {updatedGroup.Id} not found!");
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
        public int CurrentTaskId;
        public List<QuestTask> Tasks = new List<QuestTask>();

        public bool CheckOpen(string npcName)
        {
            return OpenNPS == npcName && !InProgress && !Complite;
        }

        public void StartQuest()
        {
            InProgress = true;
            CurrentTaskId = Tasks.FirstOrDefault()?.Id ?? -1;
        }
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
                    .Any(t => t.Id == id && t.IsDone));
        }

        private void ApplyResources()
        {
            GameResources.GameResources.ChangeGold(Resources.Gold);
            GameResources.GameResources.ChangeFood(Resources.Food);
            GameResources.GameResources.ChangePeopleSatisfaction(Resources.PeopleSatisfaction);
            GameResources.GameResources.ChangeCastleStrength(Resources.CastleStrength);
        }
    }

    [JsonArray]
    public class QuestSaveData : List<DayData>
    {
    }
}