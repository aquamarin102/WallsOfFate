using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectOnEnableDisable : MonoBehaviour
{
    private void OnEnable()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
