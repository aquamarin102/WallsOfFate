using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public System.Action<GameObject, GameObject> OnObjectEnteredTrigger; // Событие для передачи данных

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Объект {other.name} вошёл в триггер {gameObject.name}");
        OnObjectEnteredTrigger?.Invoke(gameObject, other.gameObject); // Вызов события
    }
}