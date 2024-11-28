using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerTest : MonoBehaviour
{
    [Header("Movement Params")]
    public float runSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravityScale = 20f;

    private BoxCollider coll;
    private Rigidbody rb;

 
    private bool isGrounded = false;

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

        ApplyGravity();

        UpdateIsGrounded();

        HandleHorizontalMovement();

        HandleJumping();
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.velocity += gravityScale * Time.fixedDeltaTime * Vector3.down;
        }
    }

    private void UpdateIsGrounded()
    {
        Bounds colliderBounds = coll.bounds;
        Vector3 groundCheckPos = colliderBounds.center + Vector3.down * (colliderBounds.extents.y + 0.1f);
        float groundCheckRadius = 0.4f;

        Collider[] colliders = Physics.OverlapSphere(groundCheckPos, groundCheckRadius);
        isGrounded = false;

        foreach (Collider c in colliders)
        {
            if (c != coll)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();
        Vector3 moveDirection = new Vector3(moveInput.y, 0, moveInput.x).normalized;
        Vector3 velocity = moveDirection * runSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    private void HandleJumping()
    {
        bool jumpPressed = InputManager.GetInstance().GetJumpPressed();
        if (isGrounded && jumpPressed)
        {
            isGrounded = false;
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (coll != null)
        {
            Bounds colliderBounds = coll.bounds;
            Vector3 groundCheckPos = colliderBounds.center + Vector3.down * (colliderBounds.extents.y + 0.1f);
            float groundCheckRadius = 0.4f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPos, groundCheckRadius);
        }
    }
}
