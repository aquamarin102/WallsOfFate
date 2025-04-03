using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    private List<ITriggerable> encounteredTriggers = new List<ITriggerable>();
    private HashSet<ITriggerable> triggeredSet = new HashSet<ITriggerable>(); // Для отслеживания уже активированных триггеров
    private ITriggerable currentTriggerable;
    private GameObject interactionIndicator;
    private bool hasInteracted = false;
    private Collider currentTriggerCollider;

    private void OnTriggerEnter(Collider collider)
    {
        ITriggerable newTriggerable = collider.gameObject.GetComponent<ITriggerable>();
        if (newTriggerable != null)
        {
            // Добавляем триггер в список, если его там еще нет
            if (!encounteredTriggers.Contains(newTriggerable))
            {
                encounteredTriggers.Add(newTriggerable);
            }

            // Устанавливаем текущий активный триггер
            if (currentTriggerCollider != collider || !hasInteracted)
            {
                currentTriggerable = newTriggerable;
                currentTriggerCollider = collider;
                hasInteracted = false;

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
        //// Обработка текущего триггера
        //if (currentTriggerable != null && currentTriggerable is Box)
        //{
        //    bool isInteract = InputManager.GetInstance().GetInteractPressed();
        //    Debug.Log("Interacting with: " + currentTriggerable);
        //    TryTrigger(currentTriggerable);
        //    hasInteracted = true;
        //    if (interactionIndicator != null)
        //    {
        //        interactionIndicator.SetActive(false);
        //    }
        //}
        if (currentTriggerable != null && (currentTriggerable is Box || !hasInteracted))
        {
            bool isInteract = InputManager.GetInstance().GetInteractPressed();

            if (isInteract)
            {
                Debug.Log("Interacting with: " + currentTriggerable);
                TriggerAllEncounteredOnce(); // Переименованный метод
                //TryTrigger(currentTriggerable); // Используем новую функцию
                hasInteracted = true;


                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }
    }

    // Пытаемся активировать триггер, если еще не активировали
    private void TryTrigger(ITriggerable trigger)
    {
        if (!triggeredSet.Contains(trigger) || trigger is Box)
        {
            trigger.Trrigered();
            triggeredSet.Add(trigger);
        }
        else
        {
            Debug.Log($"Trigger {trigger} already activated, skipping");
        }
    }

    // Метод для запуска всех сохраненных триггеров по одному разу
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

    // Метод для получения списка всех встреченных триггеров
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