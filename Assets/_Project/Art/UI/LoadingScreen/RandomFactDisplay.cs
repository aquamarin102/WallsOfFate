using UnityEngine;
using TMPro;

public class RandomFactDisplay : MonoBehaviour
{
    [Header("Настройки фактов")]
    [Tooltip("Компонент TextMeshPro для отображения факта.")]
    public TMP_Text factText;

    [Tooltip("Массив фактов, из которого выбирается один случайный.")]
    [TextArea(2, 5)]
    public string[] facts;

    private void OnEnable()
    {
        RefreshFact();
    }

    /// <summary>
    /// Выбирает случайный факт и присваивает его текстовому компоненту.
    /// </summary>
    public void RefreshFact()
    {
        if (factText != null && facts != null && facts.Length > 0)
        {
            int index = Random.Range(0, facts.Length);
            factText.text = facts[index];
        }
    }
}
