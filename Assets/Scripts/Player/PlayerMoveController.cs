using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Params")]

    [SerializeField] private float _runSpeed = 6.0f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private float _interval = 1f;
    [SerializeField] private Transform _cameraTrnsform;

    private BoxCollider _coll;
    private Rigidbody _rb;
    private bool _stopMoving = false;
    private MovemtForMOvingObjects _movementComponent;
    private Transform _thisTransform;

    public bool IsMove = false;


    private void Awake()
    {
        Box box = FindObjectOfType<Box>();
        if (box != null)
        {
            box.OnActivated += CollidedWithMovingObject;
        }
        else
        {
            Debug.LogWarning("Box не найден!");
        }
        _coll = GetComponent<BoxCollider>();
        _rb = GetComponent<Rigidbody>();
        _thisTransform = GetComponent<Transform>();

        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if(DialogueManager.GetInstance().DialogueIsPlaying)
        {
            IsMove = false;
            return;
        }

        if (!_stopMoving)
        {
            HandleHorizontalMovement();
        }
        else if (_stopMoving)
        {
            IsMove = false;
            MoveToPlatform(_interval);
        }
    }

    public void CollidedWithMovingObject()
    {
        GameObject closestCollidedObject = FindClosestMovingObject(); 
        if (closestCollidedObject != null)
        {
            _movementComponent = closestCollidedObject.GetComponent<MovemtForMOvingObjects>(); 
            if (_movementComponent != null)
            {
                //Debug.Log("Найден ближайший объект с компонентом MovemtForMOvingObjects: " + closestCollidedObject.name);
                _movementComponent.ChangeNeedToMovie();
                _stopMoving = !_stopMoving;
                Debug.Log("Player:" + _stopMoving);
            }
        }
        else
        {
            Debug.Log("Ближайший объект с компонентом MovemtForMOvingObjects не найден.");
        }
    }

    // Возможно, потом обновить алгоритм поиска
    private GameObject FindClosestMovingObject()
    {
        MovemtForMOvingObjects[] movingComponents = FindObjectsOfType<MovemtForMOvingObjects>(); 
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity; 
        Vector3 currentPosition = transform.position; 

        foreach (MovemtForMOvingObjects component in movingComponents)
        {
            float distance = Vector3.Distance(currentPosition, component.transform.position); 
            if (distance < closestDistance) 
            {
                closestDistance = distance; 
                closestObject = component.gameObject; 
            }
        }

        return closestObject; 
    }

    public void MoveToPlatform(float interval)
    {
        if (_movementComponent != null && _movementComponent.Platform != null)
        {
            Transform platform = _movementComponent.Platform;
            float step = _runSpeed * Time.deltaTime;

            // Берем интервал к forward так только так можно добиться адекваетной дистанции от кубика
            Vector3 targetPosition = platform.position - platform.forward * interval;
            Vector3 changedTargetPosition = targetPosition;

            //Debug.Log("changedTargetPosition " + changedTargetPosition);

            // Просто меняем позицию т. к. тогда не возникает проблем с задержкой в движении игрока к позиции
            transform.position = changedTargetPosition /*Vector3.MoveTowards(transform.position, changedTargetPosition, step)*/;

            // Тут без разниции по скорость поворота большая
            Quaternion targetRotation = platform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            //transform.rotation = platform.rotation;
        }
        else
        {
            Debug.LogWarning("Platform или _movementComponent не установлен!");
        }
    }

    private void HandleHorizontalMovement()
    {
        IsMove = true;
        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();
        Vector3 movePlayerInputDirection = new Vector3(moveInput.x, _thisTransform.position.y, moveInput.y).normalized;
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
