using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{

    private ITriggerable currentTriggerable; 

    private void OnTriggerEnter(Collider collider)
    {
        currentTriggerable = collider.gameObject.GetComponent<ITriggerable>();

        if (currentTriggerable != null)
        {
            Debug.Log("Игрок может взаимодействовать с объектом.");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<ITriggerable>() == currentTriggerable)
        {
            currentTriggerable = null;
            Debug.Log("Игрок покинул зону взаимодействия.");
        }
    }

    private void Update()
    {        
        if (currentTriggerable != null && InputManager.GetInstance().GetInteractPressed())
        {
            Debug.Log("Игрок взаимодействует с объектом.");
            currentTriggerable.Trrigered();
            currentTriggerable = null; 
        }
    }
}
