using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
    private bool _catchPlatform = false;
    private MovemtForMOvingObjects _movementComponent;
    private Transform _thisTransform;

    public bool IsMove = false;

    [Inject]
    private void Construct(Transform cameraTransform)
    {
        _cameraTrnsform = cameraTransform;
    }

    private void Awake()
    {
        Box[] boxes = FindObjectsOfType<Box>();
        foreach(Box box in boxes)
        {
            if (box != null)
            {
                box.OnActivated += CollidedWithMovingObject;
            }
        }
        
        
        _coll = GetComponent<BoxCollider>();
        _rb = GetComponent<Rigidbody>();
        _thisTransform = GetComponent<Transform>();

        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if(DialogueManager.GetInstance() != null && DialogueManager.GetInstance().DialogueIsPlaying)
        {
            IsMove = false;
            return;
        }

        if (!_catchPlatform)
        {
            HandleHorizontalMovement();
        }
        else if (_catchPlatform)
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
                //Debug.Log("������ ��������� ������ � ����������� MovemtForMOvingObjects: " + closestCollidedObject.name);
                _movementComponent.ChangeNeedToMovie();
                _catchPlatform = !_catchPlatform;
                Debug.Log("Player:" + _catchPlatform);
            }
        }
        else
        {
            Debug.Log("��������� ������ � ����������� MovemtForMOvingObjects �� ������.");
        }
    }

    // ��������, ����� �������� �������� ������
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

            // ����� �������� � forward ��� ������ ��� ����� �������� ����������� ��������� �� ������
            Vector3 targetPosition = platform.position - platform.forward * interval;
            Vector3 changedTargetPosition = targetPosition;

            //Debug.Log("changedTargetPosition " + changedTargetPosition);

            // ������ ������ ������� �. �. ����� �� ��������� ������� � ��������� � �������� ������ � �������
            transform.position = changedTargetPosition /*Vector3.MoveTowards(transform.position, changedTargetPosition, step)*/;

            // ��� ��� �������� �� �������� �������� �������
            Quaternion targetRotation = platform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            //transform.rotation = platform.rotation;
        }
        else
        {
            Debug.LogWarning("Platform ��� _movementComponent �� ����������!");
        }
    }

    private void HandleHorizontalMovement()
    {
        IsMove = true;
        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();
        Vector3 movePlayerInputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 moveDirection = Vector3.zero;

        // ��������� ���� �� ���������� ������
        float cameraAngle = _cameraTrnsform.eulerAngles.y * Mathf.Deg2Rad;

        // ���������� ������������������ ��������
        float sinAngle = Mathf.Sin(cameraAngle);
        float cosAngle = Mathf.Cos(cameraAngle);
        float cotAngle = cosAngle / sinAngle; // ctg(x) = cos(x) / sin(x)
        if (sinAngle != 0)
        {
            // ������� ��� moveDirection
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
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
    }
}
