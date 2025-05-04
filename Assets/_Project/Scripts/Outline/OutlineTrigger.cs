using UnityEngine;
using cakeslice;

public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;
    private InteractableItem interactable;

    void Start()
    {
        // Получаем все Outline компоненты в дочерних объектах
        outlines = GetComponentsInChildren<Outline>();

        // Выключаем все по умолчанию
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }

        // Ищем скрипт взаимодействия (может отсутствовать!)
        interactable = GetComponent<InteractableItem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверка: если есть InteractableItem, то подсвечиваем только если не использован
            if (interactable == null || !interactable.HasBeenUsed)
            {
                foreach (var outline in outlines)
                {
                    outline.enabled = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var outline in outlines)
            {
                outline.enabled = false;
            }
        }
    }
}
