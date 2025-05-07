using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Collider))]  // Убедитесь, что на том же объекте есть Collider (не обязательно Trigger)
public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;
    private InteractableItem interactable;

    // состояния
    private bool isPlayerInTrigger = false;
    private bool isMouseOver = false;

    void Start()
    {
        // Берём все Outline в детях
        outlines = GetComponentsInChildren<Outline>(true);

        // Отключаем
        foreach (var o in outlines) o.enabled = false;

        // Ваш скрипт взаимодействия (может быть null)
        interactable = GetComponent<InteractableItem>();
    }

    private void UpdateOutlineState()
    {
        // можно ли подсвечивать?
        bool canHighlight = (interactable == null || !interactable.HasBeenUsed);
        bool shouldBeOn = canHighlight && (isPlayerInTrigger || isMouseOver);

        foreach (var o in outlines)
            o.enabled = shouldBeOn;
    }

    // ——— триггер игрока ———
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UpdateOutlineState();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UpdateOutlineState();
        }
    }

    // ——— мышь над объектом ———
    // Эти события будут вызываться, если у этого же GameObject есть Collider (не обязательно isTrigger)
    void OnMouseEnter()
    {
        isMouseOver = true;
        UpdateOutlineState();
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        UpdateOutlineState();
    }
}
