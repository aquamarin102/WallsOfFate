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
        if (mainCamera == null || miniGameCamera == null) return;

        bool isMainActive = mainCamera.enabled;

        mainCamera.enabled = !isMainActive;
        miniGameCamera.enabled = isMainActive;
        StartCoroutine(ForceRenderUpdate());
    }

    private IEnumerator ForceRenderUpdate()
    {
        yield return null; // ∆дем один кадр
        mainCamera.Render();
        miniGameCamera.Render();
    }

}
