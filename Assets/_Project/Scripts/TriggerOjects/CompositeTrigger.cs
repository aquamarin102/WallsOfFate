using UnityEngine;
using System;
using System.Collections.Generic;
using Quest;

public class CompositeTrigger : MonoBehaviour, ITriggerable
{
    [Header("Quest Settings")]
    [SerializeField] private int _SelfId; // ID текущего квеста
    [SerializeField] private List<int> _requiredTriggerIds = new List<int>(); // ID требуемых квестов
    [SerializeField] private bool _once = false;

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    private void Start()
    {
        ValidateQuestConfiguration();
    }

    public void Triggered()
    {
        if (IsDone && _once) return;
        if (CheckTriggerConditions())
        {
            IsDone = true;
            OnActivated?.Invoke();
            QuestCollection.GetQuestById(_SelfId).IsDone = true;
        }
    }

    private bool CheckTriggerConditions()
    {
        // Проверка выполнения всех требуемых квестов
        foreach (int questId in _requiredTriggerIds)
        {
            Quest.Quest quest = QuestCollection.GetQuestById(questId);
            if (quest == null || !quest.IsDone)
            {
                return false;
            }
        }

        // Проверка соответствия текущего активного квеста
        Quest.Quest currentQuest = QuestCollection.GetFirstNotDoneQuest();
        return currentQuest != null && currentQuest.Id == _SelfId;
    }

    private void ValidateQuestConfiguration()
    {
        if (_requiredTriggerIds.Count == 0)
        {
            Debug.LogWarning($"No required quests assigned to CompositeTrigger on {gameObject.name}", this);
        }

        if (QuestCollection.GetQuestById(_SelfId) == null)
        {
            Debug.LogError($"Self Quest ID {_SelfId} not found in QuestCollection!", this);
        }
    }

    // Обновленные методы для работы с ID (опционально)
    public void AddRequiredQuestId(int questId)
    {
        if (!_requiredTriggerIds.Contains(questId))
        {
            _requiredTriggerIds.Add(questId);
        }
    }

    public void SetSelfId(int newId)
    {
        _SelfId = newId;
    }
}