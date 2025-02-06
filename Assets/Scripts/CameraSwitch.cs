using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera miniGameCamera;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
    }

    public void SwitchCamera()
    {
        if (mainCamera != null)
            mainCamera.enabled = !mainCamera.enabled;

        if (miniGameCamera != null)
            miniGameCamera.enabled = !miniGameCamera.enabled;

        if (mainCamera.enabled)
        {
            mainCamera = Camera.main;
        }
        else
        {
            miniGameCamera = Camera.main;
        }
    }
}
