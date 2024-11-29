using UnityEngine;
using System;

public class Lever : MonoBehaviour, ITriggerable
{
    public event Action OnActivated;
    public void Trrigered()
    {
        Debug.Log("Рычаг активирован!");
        // Логика активации рычага

        OnActivated?.Invoke();
    }
}
