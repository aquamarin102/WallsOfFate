using UnityEngine;
using UnityEngine.SceneManagement;
using Quest;
using System.Linq;

public class DayManager : MonoBehaviour
{
    [SerializeField] private InventoryLogicEnd _inventoryLogicEnd;

    private void Update()
    {
        // Проверяем условия для старта нового дня каждый кадр
        CheckNewDayConditions();
    }

    // Публичный метод для проверки условий нового дня
    public void CheckNewDayConditions()
    {
        // Ищем главный квест текущего дня, который завершен
        var completedPrimeQuest = QuestCollection.GetAllQuestGroups()
            .FirstOrDefault(q => q.Prime && q.Complite && !q.InProgress);

        if (completedPrimeQuest != null)
        {
            StartNewDay();
        }
    }

    // Публичный метод для начала нового дня
    public void StartNewDay()
    {
        if (_inventoryLogicEnd != null)
        {
            // Обновляем панель инвентаря
            _inventoryLogicEnd.RefreshPanel();
        }
        else
        {
            Debug.LogWarning("InventoryLogicEnd не назначен в инспекторе!");
        }

        QuestCollection.IncreaseCurrentDay();

        // Загружаем сцену начала дня
        SceneManager.LoadScene("StartDay");

        // Дополнительные действия при начале нового дня
        Debug.Log("Начинается новый день!");
    }

    // Метод для установки ссылки на InventoryLogicEnd
    public void SetInventoryLogicEnd(InventoryLogicEnd logicEnd)
    {
        _inventoryLogicEnd = logicEnd;
    }
}