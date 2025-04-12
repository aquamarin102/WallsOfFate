using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    // Список всех встреченных интерактивных объектов (реализующих ITriggerable)
    private List<ITriggerable> encounteredTriggers = new List<ITriggerable>();
    // Набор для отслеживания уже активированных триггеров
    private HashSet<ITriggerable> triggeredSet = new HashSet<ITriggerable>();
    // Текущий активный интерактивный объект
    private ITriggerable currentTriggerable;
    // Объект-индикатор взаимодействия (например, подсветка)
    private GameObject interactionIndicator;
    // Флаг, чтобы избежать повторного взаимодействия до выхода из зоны
    private bool hasInteracted = false;
    // Коллайдер текущего интерактивного объекта
    private Collider currentTriggerCollider;

    // Ссылка на компонент анимаций игрока, который содержит методы PlayPickupFloor, PlayPickupBody и PlayOpenChest
    private PlayerAnimator playerAnimator;

    private void Awake()
    {
       
        playerAnimator = GetComponent<PlayerAnimator>();
        if (playerAnimator == null)
        {
            Debug.LogError("InteractManager: Не найден компонент PlayerAnimator!");
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        ITriggerable newTriggerable = collider.gameObject.GetComponent<ITriggerable>();
        if (newTriggerable != null)
        {
            // Добавляем интерактивный объект в список, если его там еще нет
            if (!encounteredTriggers.Contains(newTriggerable))
            {
                encounteredTriggers.Add(newTriggerable);
            }

            // Устанавливаем текущий активный интерактивный объект
            if (currentTriggerCollider != collider || !hasInteracted)
            {
                currentTriggerable = newTriggerable;
                currentTriggerCollider = collider;
                hasInteracted = false;

                // Ищем дочерний объект с тегом "InteractionIndicator" и активируем его
                var indicators = collider.gameObject.GetComponentsInChildren<Transform>(true);
                foreach (var indicator in indicators)
                {
                    if (indicator.CompareTag("InteractionIndicator"))
                    {
                        interactionIndicator = indicator.gameObject;
                        interactionIndicator.SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // При выходе из зоны интерактивного объекта сбрасываем данные
        if (collider == currentTriggerCollider)
        {
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }

            encounteredTriggers.Clear();
            triggeredSet.Clear();
            currentTriggerable = null;
            currentTriggerCollider = null;
            interactionIndicator = null;
            hasInteracted = false;
        }
    }

    private void Update()
    {
        // Если есть активный интерактивный объект и ещё не было взаимодействия
        if (currentTriggerable != null && !hasInteracted)
        {
            // Получаем нажатие кнопки взаимодействия через ваш InputManager
            bool isInteract = InputManager.GetInstance().GetInteractPressed();
            if (isInteract)
            {
                Debug.Log("Interacting with: " + currentTriggerable);

                // Приводим currentTriggerable к MonoBehaviour для доступа к GameObject (предполагается, что ITriggerable – компонент)
                GameObject triggerObj = (currentTriggerable as MonoBehaviour).gameObject;
                // В зависимости от тега запускаем соответствующую анимацию
                if (triggerObj.CompareTag("PickupFloor"))
                {
                    playerAnimator.PlayPickupFloor();
                    TriggerAllEncounteredOnce();
                }
                else if (triggerObj.CompareTag("PickupBody"))
                {
                    playerAnimator.PlayPickupBody();
                    TriggerAllEncounteredOnce();
                }
                else if (triggerObj.CompareTag("Chest"))
                {
                    playerAnimator.PlayOpenChest();
                    TriggerAllEncounteredOnce();
                }
                else
                {
                    // Если тип не распознан, активируем все встреченные триггеры стандартно
                    TriggerAllEncounteredOnce();
                }
                hasInteracted = true;

                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }
    }

    // Пытаемся активировать конкретный триггер, если он еще не был активирован (либо если это Box)
    private void TryTrigger(ITriggerable trigger)
    {
        if (!triggeredSet.Contains(trigger) || trigger is Box)
        {
            trigger.Triggered();
            triggeredSet.Add(trigger);
        }
        else
        {
            Debug.Log($"Trigger {trigger} already activated, skipping");
        }
    }

    // Метод для однократного запуска всех встреченных триггеров
    public void TriggerAllEncounteredOnce()
    {
        foreach (var trigger in encounteredTriggers)
        {
            if (trigger != null)
            {
                TryTrigger(trigger);
            }
        }
    }

    // Метод для получения списка всех встреченных интерактивных объектов
    public List<ITriggerable> GetEncounteredTriggers()
    {
        return new List<ITriggerable>(encounteredTriggers);
    }

    // Метод для проверки, был ли триггер уже активирован
    public bool HasTriggerBeenActivated(ITriggerable trigger)
    {
        return triggeredSet.Contains(trigger);
    }
}
