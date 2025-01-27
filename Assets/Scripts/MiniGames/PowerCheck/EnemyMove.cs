using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Movement Params")]

    [SerializeField] private float _runDefaultSpeed = 6.0f;
    [SerializeField] private float _runSpeed = 6.0f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private Transform _cameraTrnsform;

    private Rigidbody _rb;
    private MiniGamePlayer _playerChar;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
    }

    public void ChangeSpeed(float speed)
    {
        _runSpeed = _runDefaultSpeed * speed;
    }

    private void HandleHorizontalMovement()
    {
        // Обработка нажатия клавиш
        //if (Input.GetKeyDown(KeyCode.O)) isMovingUp = true;
        //if (Input.GetKeyDown(KeyCode.K)) isMovingLeft = true;
        //if (Input.GetKeyDown(KeyCode.L)) isMovingDown = true;
        //if (Input.GetKeyDown(KeyCode.Semicolon)) isMovingRight = true;

        //// Обработка отпускания клавиш
        //if (Input.GetKeyUp(KeyCode.O)) isMovingUp = false;
        //if (Input.GetKeyUp(KeyCode.K)) isMovingLeft = false;
        //if (Input.GetKeyUp(KeyCode.L)) isMovingDown = false;
        //if (Input.GetKeyUp(KeyCode.Semicolon)) isMovingRight = false;

        // Построение вектора направления
        Vector2 moveInput = Vector2.zero;

        // Обработка нажатия клавиш (удержание)
        if (Input.GetKey(KeyCode.O)) moveInput.y += 1;     // Вверх
        if (Input.GetKey(KeyCode.K)) moveInput.x -= 1;     // Влево
        if (Input.GetKey(KeyCode.L)) moveInput.y -= 1;     // Вниз
        if (Input.GetKey(KeyCode.Semicolon)) moveInput.x += 1; // Вправо

        if (moveInput == Vector2.zero) return;

        Vector3 movePlayerInputDirection = new Vector3(moveInput.x, this.GetComponent<Transform>().position.y, moveInput.y).normalized;
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
