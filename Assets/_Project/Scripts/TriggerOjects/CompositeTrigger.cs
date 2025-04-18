using UnityEngine;
using System;
using System.Collections.Generic;
using Quest;
using System.Linq;

public class CompositeTrigger : MonoBehaviour, ITriggerable
{
    [Header("Quest Settings")]
    [SerializeField] private string _selfName; // ID текущего квеста
    //[SerializeField] private List<int> _requiredTriggerIds = new List<int>(); // ID требуемых квестов
    //[SerializeField] private bool _once = false;

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    public void Triggered()
    {
        // Проверка на старт новых квестов
        var availableGroups = QuestCollection.GetAllDays()
            .SelectMany(d => d.Quests)
            .Where(q => q.CheckOpen(_selfName))
            .ToList();

        if (availableGroups.Count > 0)
        {
            var group = availableGroups.First();
            group.StartQuest();
            return;
        }

        // Обработка активных квестов
        var activeGroups = QuestCollection.GetActiveQuestGroups();
        for ( int i = 0; i < activeGroups.Count; i++)
        {
            var currentTask = QuestCollection.GetCurrentTaskForGroup(activeGroups[i]);
            if (currentTask != null && CanTriggerTask(currentTask))
            {
                currentTask.CompleteTask();
                activeGroups[i] = UpdateGroupState(activeGroups[i]);
                QuestCollection.UpdateQuestGroup(activeGroups[i]);
                return;
            }
        }
    }

    private bool CanTriggerTask(QuestTask task)
    {
        return task.ForNPS == _selfName && task.CanComplete();
    }

    private QuestGroup UpdateGroupState(QuestGroup group)
    {
        bool allTasksDone = group.Tasks.All(t => t.IsDone);

        return new QuestGroup
        {
            Id = group.Id,
            Complite = allTasksDone,
            InProgress = !allTasksDone,
            OpenNPS = group.OpenNPS,
            OpenDialog = group.OpenDialog,
            CurrentTaskId = allTasksDone
                ? -1
                : group.Tasks
                    .Where(t => !t.IsDone)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault()?.Id ?? -1,
            Tasks = group.Tasks
        };
    }
}