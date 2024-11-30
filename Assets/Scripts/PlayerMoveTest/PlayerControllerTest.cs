using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerTest : MonoBehaviour
{
    [Header("Movement Params")]

    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 20f;

    private BoxCollider coll;
    private Rigidbody rb;

 
    private void Awake()

    {
        coll = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if(DialogueManager.GetInstance().DialogueIsPlaying)
        {
            return;
        }

        HandleHorizontalMovement();
    }

    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        Vector3 velocity = moveDirection * runSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

}
