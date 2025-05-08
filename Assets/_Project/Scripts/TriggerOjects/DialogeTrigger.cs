// DialogeTrigger.cs
using UnityEngine;
using Quest;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

internal class DialogeTrigger : MonoBehaviour, ICheckableTrigger
{
    [Header("Dialogue Settings")]
    [SerializeField] private string _defaultDialogue;
    [SerializeField] private string _npcName;
    public GameObject PowerCheckPrefab;

    public bool IsDone { get; private set; }

    public void Triggered()
    {
        DialogueManager.GetInstance().PowerCheckPrefab = PowerCheckPrefab;
        //if(QuestCollection.GetActiveQuestGroups().Count > 0 && QuestCollection.GetActiveQuestGroups()[0].CurrentTaskId == 5) return;
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
        var groupToUpdate = activeGroups.FirstOrDefault(g =>
            g.GetCurrentTask() != null &&
            CanTriggerTask(g.GetCurrentTask(), out var dialogue));

        if (groupToUpdate != null)
        {
            groupToUpdate.GetCurrentTask().CompleteTask();
            groupToUpdate = UpdateGroupState(groupToUpdate);

            var originalGroup = QuestCollection.GetAllQuestGroups()
                .FirstOrDefault(g => g.Id == groupToUpdate.Id);

            originalGroup?.CopyFrom(groupToUpdate);
        }

        // Дефолтный диалог
        DialogueManager.GetInstance().EnterDialogueMode(_defaultDialogue);
    }

    private bool CanTriggerTask(QuestTask task, out string dialogue)
    {
        dialogue = task.RequeredDialog;
        return dialogue != null && task.ForNPS == _npcName && task.CanComplete();
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