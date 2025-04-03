using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Box : MonoBehaviour, ITriggerable
{
    public event Action OnActivated;
    public void Trrigered()
    {
        //Debug.Log("Коробку можно двигать!");
        OnActivated?.Invoke();
    }
}
