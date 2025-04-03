using UnityEngine;

public class BoxGrabber : MonoBehaviour
{
    [Header("Настройки захвата")]
    // Расстояние, в пределах которого можно схватить коробку
    public float grabDistance = 2.0f;
    // Клавиша для захвата/отпуска коробки
    public KeyCode grabKey = KeyCode.E;
    // Точка, в которой происходит захват (например, позиция рук персонажа)
    public Transform grabPoint;

    // Текущий FixedJoint, соединяющий коробку с персонажем
    private FixedJoint grabJoint;
    // Rigidbody захваченной коробки
    private Rigidbody grabbedBoxRb;

    void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (grabJoint == null)
            {
                TryGrab();
            }
            else
            {
                ReleaseGrab();
            }
        }
    }

    // Метод попытки захвата коробки
    void TryGrab()
    {
        // Находим все коллайдеры в пределах grabDistance от точки захвата
        Collider[] hits = Physics.OverlapSphere(grabPoint.position, grabDistance);
        foreach (Collider hit in hits)
        {
            // Для захвата ищем объект с тегом "Box"
            if (hit.CompareTag("Box"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Добавляем FixedJoint к объекту grabPoint и соединяем его с Rigidbody коробки
                    grabJoint = grabPoint.gameObject.AddComponent<FixedJoint>();
                    grabJoint.connectedBody = rb;
                    grabbedBoxRb = rb;
                    // При необходимости можно настроить параметры соединителя (например, breakForce)
                    return;
                }
            }
        }
    }

    // Метод отпускания коробки
    void ReleaseGrab()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
            grabbedBoxRb = null;
        }
    }

    // Отрисовка сферы захвата в редакторе для удобства настройки
    private void OnDrawGizmosSelected()
    {
        if (grabPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grabPoint.position, grabDistance);
        }
    }
}
