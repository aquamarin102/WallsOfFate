using UnityEngine;
using cakeslice;

public class OutlineTrigger : MonoBehaviour
{
    private Outline[] outlines;

    void Start()
    {
        // Получаем все Outline компоненты в дочерних объектах
        outlines = GetComponentsInChildren<Outline>();

        // Выключаем все по умолчанию
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
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
