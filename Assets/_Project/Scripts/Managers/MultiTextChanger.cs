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
        SyncIcons();   // ← здесь ваша старая, уже рабочая логика
    }

    private void UpdateQuestText()
    {
        try
        {
            // Берём все квесты текущего дня
            var allGroups = QuestCollection.GetAllQuestGroups();

            // 0) Ни одного квеста ещё не стартовано
            if (allGroups.Count > 0 && allGroups.All(q => !q.InProgress && !q.Complite))
            {
                ShowSingleMessage(_defaultTextStillQuests);
                return;
            }

            // 1) Все квесты этого дня завершены
            if (allGroups.Count > 0 && allGroups.All(q => q.Complite))
            {
                ShowSingleMessage(_defaultTextAllQuests);
                return;
            }

            // 2) Смешанные случаи
            bool primeDone = allGroups.Any(q => q.Prime && q.Complite);
            bool hasSide = allGroups.Any(q => !q.Prime);
            bool sideLeft = allGroups.Any(q => !q.Prime && !q.Complite);

            // 2-а) Главный пройден, есть сайд-квесты, но все они завершены → всё сделано
            if (primeDone && hasSide && !sideLeft)
            {
                ShowSingleMessage(_defaultTextAllQuests);
                return;
            }

            // 2-б) Только главный и побочных изначально не было → подсказка «поговорить»
            if (primeDone && !hasSide)
            {
                ShowSingleMessage(_defaultTextStillQuests);
                return;
            }

            // 2-в) Иначе показываем (опциональную) подсказку + текущие задачи
            int idx = 0;

            // Если главный уже сделан — сначала сообщение
            if (primeDone)
                _textMeshProLinks[idx++].text = _defaultTextStillQuests;

            // Затем все активные (InProgress && !Complite), при этом Prime первым
            foreach (var group in allGroups
                                  .Where(q => q.InProgress && !q.Complite)
                                  .OrderByDescending(q => q.Prime))
            {
                if (idx >= _textMeshProLinks.Count) break;
                _textMeshProLinks[idx++].text = group.GetCurrentTask().TaskInfo;
            }

            // — очищаем остаток —
            for (; idx < _textMeshProLinks.Count; idx++)
                _textMeshProLinks[idx].text = "";
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
        bool isShowingMessage = string.IsNullOrEmpty(_textMeshProLinks[0].text) == false
                                && QuestCollection.GetActiveQuestGroups().Count == 0;
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
