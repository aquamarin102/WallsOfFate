using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        float xRotation = transform.localEulerAngles.x; if (xRotation > 180f) xRotation -= 360f; bool isInRotationRange = xRotation >= 85f && xRotation <= 95f;

        if (isInRotationRange)
        {
            // ‘иксируем ротацию, если угол в заданном диапазоне
            transform.rotation = Quaternion.Euler(90f, 180f, 0f);
        }
        else if (Camera.main != null)
            transform.rotation =
                Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
