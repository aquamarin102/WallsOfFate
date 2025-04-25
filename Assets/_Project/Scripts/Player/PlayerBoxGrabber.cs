using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxGrabber : MonoBehaviour
{
    #region Inspector
    [Header("Grab Settings")]
    [SerializeField] private float grabRange = 2.0f;
    [SerializeField] private KeyCode grabKey = KeyCode.E;

    [Header("Box Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    #endregion

    // -------------------- runtime --------------------
    public Transform attachedBox = null;

    private int originalBoxLayer;
    private Vector3 localGrabOffset = Vector3.zero;
    private float currentSpeed = 0f;

    private Rigidbody attachedRb;
    private bool wasKinematic;

    private enum GrabAxis { ForwardBack, LeftRight }
    private GrabAxis grabAxis = GrabAxis.ForwardBack;
    // -------------------------------------------------

    private void Update()
    {
        /*---------- E: взять / отпустить ----------*/
        if (Input.GetKeyDown(grabKey))
        {
            if (attachedBox == null) TryAttachBox();
            else DetachBox();
        }

        if (attachedBox == null) return;

        /*---------- 1. ввод игрока ----------*/
        float vertical = Input.GetAxis("Vertical");   // W-S / ↑-↓
        float horizontal = Input.GetAxis("Horizontal"); // A-D / ←-→

        float moveInput =
            (grabAxis == GrabAxis.LeftRight) ? horizontal : vertical;

        currentSpeed = Mathf.Abs(moveInput) > 0.1f
            ? moveInput * maxSpeed
            : 0f;

        /*---------- 2. перемещение ящика (без поворота) ----------*/
        Vector3 moveDir =
            (grabAxis == GrabAxis.LeftRight) ? attachedBox.right
                                             : attachedBox.forward;

        attachedBox.position += moveDir * currentSpeed * Time.deltaTime;

        /*---------- 3. положение / взгляд игрока ----------*/
        transform.position = attachedBox.TransformPoint(localGrabOffset);

        Vector3 lookDir = attachedBox.position - transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    /*================ захват =================*/
    private void TryAttachBox()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, grabRange);
        foreach (Collider col in cols)
        {
            if (!col.CompareTag("Box")) continue;

            attachedBox = col.transform;
            originalBoxLayer = attachedBox.gameObject.layer;
            attachedBox.gameObject.layer = LayerMask.NameToLayer("MovableBox");

            localGrabOffset = attachedBox.InverseTransformPoint(transform.position);

            // определяем, к какой стороне ближе
            grabAxis = Mathf.Abs(localGrabOffset.x) > Mathf.Abs(localGrabOffset.z)
                       ? GrabAxis.LeftRight
                       : GrabAxis.ForwardBack;

            currentSpeed = 0f;
            transform.SetParent(attachedBox);

            attachedRb = attachedBox.GetComponent<Rigidbody>();
            if (attachedRb != null)
            {
                wasKinematic = attachedRb.isKinematic;
                attachedRb.isKinematic = false;             // ← ящик теперь "живой"
            }

            Debug.Log($"Захватил {attachedBox.name} ({grabAxis})");
            return;
        }
        Debug.Log("Ящик в зоне захвата не найден.");
    }

    /*================ отпуск =================*/
    private void DetachBox()
    {
        if (attachedBox == null) return;

        if (attachedRb != null)
            attachedRb.isKinematic = wasKinematic;

        attachedBox.gameObject.layer = originalBoxLayer;
        transform.SetParent(null);

        Debug.Log($"Отцепился от {attachedBox.name}");
        attachedBox = null;
        currentSpeed = 0f;
    }

    /*================ визуализация =================*/
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
