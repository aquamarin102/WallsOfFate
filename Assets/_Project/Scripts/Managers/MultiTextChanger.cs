using UnityEngine;
using TMPro;
using Quest;

public class MultiTextChanger : MonoBehaviour
{
    [SerializeField] private TMP_Text _textMeshPro;

    [Header("Quest Settings")]
    [SerializeField] private string _defaultText = "Все квесты выполнены! Вы можетезакончить день!";

    private void Update()
    {
        UpdateQuestText();
    }

    private void UpdateQuestText()
    {
        var quest = QuestCollection.GetFirstNotDoneQuest();

        if (quest != null)
        {
            _textMeshPro.text = quest.QuestInfo;
        }
        else
        {
            _textMeshPro.text = _defaultText;
            Debug.LogWarning($"Can't find quest info");
        }
    }
}