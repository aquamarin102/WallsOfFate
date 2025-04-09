using UnityEngine;
using Quest;
using System.Collections.Generic;

[System.Serializable]
public class DialogueData
{
    public int questId;
    public TextAsset inkJSON;
    public List<int> requiredQuestIds = new List<int>();
}

internal class DialogeTrigger : MonoBehaviour, ICheckableTrigger
{
    [Header("Dialogue Settings")]
    [SerializeField] private List<DialogueData> _dialogues = new List<DialogueData>();
    [SerializeField] private TextAsset _defaultDialogue;

    public bool IsDone { get; private set; }

    private void Start()
    {
        ValidateDialogues();
    }

    public void Triggered()
    {
        if (CanTrigger(out DialogueData dialogueData))
        {
            DialogueManager.GetInstance().EnterDialogueMode(dialogueData.inkJSON);

            if (dialogueData.questId == QuestCollection.GetFirstNotDoneQuest()?.Id)
            {
                 QuestCollection.GetFirstNotDoneQuest().IsDone = true;
            }
        }
        else if (_defaultDialogue != null)
        {
            DialogueManager.GetInstance().EnterDialogueMode(_defaultDialogue);
        }
    }

    private bool CanTrigger(out DialogueData foundDialogue)
    {
        var currentQuest = QuestCollection.GetFirstNotDoneQuest();
        foundDialogue = null;

        if (currentQuest == null) return false;

        // Ищем диалог для текущего квеста
        foreach (var dialogue in _dialogues)
        {
            if (dialogue.questId == currentQuest.Id)
            {
                // Проверяем требования для этого конкретного диалога
                foreach (int reqId in dialogue.requiredQuestIds)
                {
                    Quest.Quest quest = QuestCollection.GetQuestById(reqId);
                    if (quest == null || !quest.IsDone) return false;
                }
                foundDialogue = dialogue;
                return true;
            }
        }
        return false;
    }

    private void ValidateDialogues()
    {
        HashSet<int> uniqueIds = new HashSet<int>();

        foreach (var dialogue in _dialogues)
        {
            if (!uniqueIds.Add(dialogue.questId))
            {
                Debug.LogError($"Duplicate quest ID {dialogue.questId} in dialogues", this);
            }

            if (dialogue.inkJSON == null)
            {
                Debug.LogError($"Missing Ink JSON for quest ID {dialogue.questId}", this);
            }
        }
    }

    // Метод для редактора
    public void AddNewDialogue(int questId, TextAsset inkJSON)
    {
        _dialogues.Add(new DialogueData
        {
            questId = questId,
            inkJSON = inkJSON
        });
    }
}