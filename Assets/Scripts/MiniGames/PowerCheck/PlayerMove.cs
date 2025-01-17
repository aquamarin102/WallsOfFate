using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Params")]

    [SerializeField] private float _runSpeed = 6.0f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private Transform _cameraTrnsform;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
    }

    
    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();
        if (moveInput == null) return;
        Vector3 movePlayerInputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 moveDirection = Vector3.zero;

        // Получение угла из ориентации камеры
        float cameraAngle = _cameraTrnsform.eulerAngles.y * Mathf.Deg2Rad;

        // Вычисление тригонометрических значений
        float sinAngle = Mathf.Sin(cameraAngle);
        float cosAngle = Mathf.Cos(cameraAngle);
        float cotAngle = cosAngle / sinAngle; // ctg(x) = cos(x) / sin(x)
        if (sinAngle != 0)
        {
            // Формулы для moveDirection
            moveDirection.x = (movePlayerInputDirection.z + movePlayerInputDirection.x * cotAngle) / (sinAngle + cotAngle * cosAngle);
            moveDirection.z = (moveDirection.x * cosAngle - movePlayerInputDirection.x) / sinAngle;
            //Debug.Log(moveDirection);
        }
        else
        {
            moveDirection = movePlayerInputDirection;
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }

        Vector3 velocity = moveDirection * _runSpeed;
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
    }
}
