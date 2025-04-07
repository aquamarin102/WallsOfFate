using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camer : MonoBehaviour
{
    [SerializeField] private Camera TheCamera;
    [SerializeField] private Camera Cam;

    void Start()
    {
        if (TheCamera == null)
        {
            TheCamera = GetComponent<Camera>();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (TheCamera != null)
                TheCamera.enabled = !TheCamera.enabled;

            if (Cam != null)
                Cam.enabled = !Cam.enabled;
        }
    }
}
