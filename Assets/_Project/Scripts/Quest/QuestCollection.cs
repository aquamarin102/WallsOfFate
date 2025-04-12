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
            return group.Tasks.FirstOrDefault(t => t.Id == group.CurrentTaskId && !t.IsDone);
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
        public bool InProgress;
        public bool Complite;
        public int CurrentTaskId = 0;
        public string OpenNPS;
        public String OpenDialog;
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
        public string RequeredDialogPath;
        public QuestResources Resources;

        public void CompleteTask()
        {
            if (IsDone || !CanComplete()) return;

            IsDone = true;
            ApplyResources();
        }

        private bool CanComplete()
        {
            return RequeredTasksIds.All(id =>
                QuestCollection.GetAllDays()
                    .SelectMany(d => d.Quests)
                    .SelectMany(q => q.Tasks)
                    .Any(t => t.Id == id && t.IsDone));
        }

        private void ApplyResources()
        {
            GameResources.Resources.ChangeGold(Resources.Gold);
            GameResources.Resources.ChangeFood(Resources.Food);
            GameResources.Resources.ChangePeopleSatisfaction(Resources.PeopleSatisfaction);
            GameResources.Resources.ChangeCastleStrength(Resources.CastleStrength);
        }
    }

    [JsonArray]
    public class QuestSaveData : List<DayData>
    {
    }
}