using UnityEngine;
using TMPro;
using Quest;
using System.Collections.Generic;
using System.Linq;
using System;

public class MultiTextChanger : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _textMeshProLinks;
    [SerializeField] private List<GameObject> _iconsLinks;

    [Header("Quest Settings")]
    [SerializeField] private string _defaultTextAllQuests = "Все квесты выполнены! Вы можете закончить день!";
    [SerializeField] private string _defaultTextStillQuests = "Основной квест выполнен! Вы можете пообщаться с другими жителями!";

    private void Update()
    {
        UpdateQuestText();
        SyncIcons();   
    }

    private void UpdateQuestText()
    {
        try
        {
            // Берём все квесты текущего дня
            var allGroups = QuestCollection.GetAllQuestGroups();


            int idx = 0;
            // Затем все активные (InProgress && !Complite), при этом Prime первым
            foreach (var group in allGroups
                                  .Where(q => q.InProgress && !q.Complite)
                                  .OrderByDescending(q => q.Prime))
            {
                if (idx >= _textMeshProLinks.Count) break;
                _textMeshProLinks[idx++].text = group.GetCurrentTask().TaskInfo;
            }

            // — очищаем остаток —
            int idxn = idx;
            for (; idxn < _textMeshProLinks.Count; idxn++)
                _textMeshProLinks[idxn].text = "";

            if (idx > 0) return;

            // 0) Ни одного квеста ещё не стартовано
            if (allGroups.Count > 0 && allGroups.Any(q => !q.InProgress && !q.Complite))
            {
                ShowSingleMessage(_defaultTextStillQuests);
                return;
            }

            // 1) Все квесты этого дня завершены
            if (allGroups.Count > 0 && allGroups.All(q => !q.InProgress && q.Complite))
            {
                ShowSingleMessage(_defaultTextAllQuests);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error {e.Message}");
        }
    }

    private void ShowSingleMessage(string msg)
    {
        // Первый слот = сообщение, остальные = пусто
        for (int i = 0; i < _textMeshProLinks.Count; i++)
            _textMeshProLinks[i].text = (i == 0 ? msg : "");
    }

    private void SyncIcons()
    {
        // Сколько сейчас активных квестов?
        int activeQuests = QuestCollection.GetActiveQuestGroups().Count;
        // Если вместо «только задач» у вас иногда выводится общее сообщение
        // (_defaultTextStillQuests или _defaultTextAllQuests), можно сделать:
        bool isShowingMessage = QuestCollection.GetActiveQuestGroups().Count == 0;
        // Тогда значение слотов, для которых показываем иконки, таково:
        int iconsToShow = activeQuests;
        if (isShowingMessage) iconsToShow = 1;

        // Перебираем все готовые иконки
        for (int i = 0; i < _iconsLinks.Count; i++)
        {
            _iconsLinks[i].SetActive(i < iconsToShow);
        }
    }
}
