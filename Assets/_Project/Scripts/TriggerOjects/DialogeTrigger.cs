// DialogeTrigger.cs
using UnityEngine;
using Quest;
using System.Collections.Generic;
using System.Linq;

internal class DialogeTrigger : MonoBehaviour, ICheckableTrigger
{
    [Header("Dialogue Settings")]
    [SerializeField] private string _defaultDialogue;
    [SerializeField] private string _npcName;

    public bool IsDone { get; private set; }

    public void Triggered()
    {
        // Проверка на старт новых квестов
        var availableGroups = QuestCollection.GetAllDays()
            .SelectMany(d => d.Quests)
            .Where(q => q.CheckOpen(_npcName))
            .ToList();

        if (availableGroups.Count > 0)
        {
            var group = availableGroups.First();
            group.StartQuest();
            DialogueManager.GetInstance().EnterDialogueMode(group.OpenDialog);
            return;
        }

        // Обработка активных квестов
        var activeGroups = QuestCollection.GetActiveQuestGroups();
        foreach (var group in activeGroups)
        {
            var currentTask = QuestCollection.GetCurrentTaskForGroup(group);
            if (currentTask != null && CanTriggerTask(currentTask, out var dialogue))
            {
                DialogueManager.GetInstance().EnterDialogueMode(dialogue);
                currentTask.CompleteTask();
                UpdateGroupState(group);
                return;
            }
        }

        // Дефолтный диалог
        DialogueManager.GetInstance().EnterDialogueMode(_defaultDialogue);
    }

    private bool CanTriggerTask(QuestTask task, out string dialogue)
    {
        dialogue = task.RequeredDialogPath;
        return dialogue != null && task.ForNPS == _npcName;
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