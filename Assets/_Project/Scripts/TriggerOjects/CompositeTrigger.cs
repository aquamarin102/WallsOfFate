using UnityEngine;
using System;
using System.Collections.Generic;
using Quest;
using System.Linq;

public class CompositeTrigger : MonoBehaviour, ITriggerable
{
    [Header("Quest Settings")]
    [SerializeField] private int _SelfId; // ID текущего квеста
    [SerializeField] private List<int> _requiredTriggerIds = new List<int>(); // ID требуемых квестов
    [SerializeField] private bool _once = false;

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    public void Triggered()
    {
        // Проверка на старт новых квестов
        var availableGroups = QuestCollection.GetAllDays()
            .SelectMany(d => d.Quests)
            .Where(q => q.CheckOpen(_SelfId.ToString()))
            .ToList();

        if (availableGroups.Count > 0)
        {
            var group = availableGroups.First();
            group.StartQuest();
            return;
        }

        // Обработка активных квестов
        var activeGroups = QuestCollection.GetActiveQuestGroups();
        foreach (var group in activeGroups)
        {
            var currentTask = QuestCollection.GetCurrentTaskForGroup(group);
            if (currentTask != null && CanTriggerTask(currentTask))
            {
                currentTask.CompleteTask();
                UpdateGroupState(group);
                return;
            }
        }
    }

    private bool CanTriggerTask(QuestTask task)
    {
        return task.ForNPS == _SelfId.ToString();
    }

    private void UpdateGroupState(QuestGroup group)
    {
        if (group.Tasks.All(t => t.IsDone))
        {
            group.Complite = true;
            group.InProgress = false;
        }
        else
        {
            group.CurrentTaskId = group.Tasks
                .FirstOrDefault(t => !t.IsDone && t.Id > group.CurrentTaskId)?.Id ?? -1;
        }
    }
}