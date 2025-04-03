using UnityEngine;
using System;

public class Lever : MonoBehaviour, ICheckableTrigger
{
    public event Action OnActivated;
    public bool IsDone { get; private set; }
    [SerializeField] private bool _once = false;

    public void Trrigered()
    {
        if (IsDone && _once) return;
        IsDone = true;
        //Debug.Log("Рычаг активирован!");
        // Логика активации рычага

        OnActivated?.Invoke();
    }
}
