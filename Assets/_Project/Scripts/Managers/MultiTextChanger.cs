using UnityEngine;
using TMPro;
using Quest;
using System.Collections.Generic;
using System.Linq;
using System;

public class MultiTextChanger : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _textMeshProLinks;
    [SerializeField] private List<GameObject> _IconsLinks;

    [Header("Quest Settings")]
    [SerializeField] private string _defaultText = "Все квесты выполнены! Вы можете закончить день!";

    private void Update()
    {
        UpdateQuestText();
        if (_textMeshProLinks.Count != 0 && _IconsLinks.Count != 0 && _IconsLinks.Count == _textMeshProLinks.Count)
        {
            for (int i = 0; i < _textMeshProLinks.Count; i++)
            {
                if (_textMeshProLinks[i].text == "") _IconsLinks[i].SetActive(false);
                else _IconsLinks[i].SetActive(true);
            }
        }
    }

    private void UpdateQuestText()
    {
        try
        {
            List<QuestGroup> processingGroups = QuestCollection.GetActiveQuestGroups();

            // Проверяем, все ли квесты завершены
            if (processingGroups.All(q => q.Complite && !q.InProgress))
            {
                _textMeshProLinks[0].text = _defaultText;
                for (int i = 1; i < _textMeshProLinks.Count; i++)
                {
                    _textMeshProLinks[i].text = "";
                }
                return;
            }

            if (processingGroups.Count > 0)
            {
                // Сортируем группы: сначала Prime, затем остальные
                var sortedGroups = processingGroups.OrderByDescending(q => q.Prime).ToList();

                // Ограничиваем количество групп количеством текстовых полей
                int groupsToShow = Mathf.Min(_textMeshProLinks.Count, sortedGroups.Count);

                for (int i = 0; i < _textMeshProLinks.Count; i++)
                {
                    if (i < groupsToShow)
                    {
                        // Показываем информацию о текущем задании для группы
                        _textMeshProLinks[i].text = sortedGroups[i].GetCurrentTask().TaskInfo;
                    }
                    else
                    {
                        // Очищаем остальные поля
                        _textMeshProLinks[i].text = "";
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error {e.Message}");
        }
    }
}