using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private float moveInputY;

    private void Start()
    {
        // Найти рычаг и подписаться на его событие
        Lever lever = FindObjectOfType<Lever>();
        if (lever != null)
        {
            lever.OnActivated += MoveUp;
        }
        else
        {
            Debug.LogWarning("Lever не найден!");
        }
    }

    private void MoveUp()
    {
        // Логика движения вверх
        moveInputY = 20f;
        Vector3 move = new Vector3(0, moveInputY * moveSpeed * Time.deltaTime, 0);
        transform.Translate(move);
    }
}
