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
    [SerializeField] private string _defaultText = "Все квесты выполнены! Вы можетезакончить день!";

    private void Update()
    {
        UpdateQuestText();
        if(_textMeshProLinks.Count != 0 && _IconsLinks.Count != 0 && _IconsLinks.Count == _textMeshProLinks.Count)
        {
            for(int i = 0; i < _textMeshProLinks.Count; i++)
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

            if (processingGroups.All(q => q.Complite && !q.InProgress))
            {
                _textMeshProLinks[0].text = _defaultText;
                foreach (TMP_Text tx in _textMeshProLinks) tx.text = "";
                return;
            }

            if (processingGroups.Count > 0)
            {
                for (int i = 0; i < _textMeshProLinks.Count; i++)
                {
                    if (i < processingGroups.Count)
                    {
                        _textMeshProLinks[i].text = QuestCollection.GetCurrentTaskForGroup(processingGroups[i]).TaskInfo;
                    }
                    else _textMeshProLinks[i].text = "";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error {e.Message}");
        }
    }
}