using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Params")]
    [SerializeField] private float _moveSpeed = 6.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _interval = 1f; // расстояние от платформы при захвате
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _controller;
    private Vector3 _moveDirection;
    private float _verticalVelocity;

    // Захват движущегося объекта
    private bool _catchPlatform = false;
    private MovemtForMOvingObjects _movementComponent;

    public bool IsMove = false;

    // Состояние нажатия клавиши E, обновляемое в Update
    private bool _grabButtonPressed = false;

    [Inject]
    private void Construct(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            Debug.LogError("PlayerController требует наличия CharacterController!");
        }
    }

    private void Update()
    {
        // Если идёт диалог, пропускаем обработку ввода
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().DialogueIsPlaying)
        {
            return;
        }

        // Обработка нажатия кнопки E в Update
        if (Input.GetKeyDown(KeyCode.E))
        {
            _grabButtonPressed = true;
        }
    }

    private void FixedUpdate()
    {
        // Если идёт диалог, движение не обрабатывается
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().DialogueIsPlaying)
        {
            IsMove = false;
            return;
        }

        // Сначала обрабатываем захват/отпуск объекта, если кнопка нажата
        if (_grabButtonPressed)
        {
            if (!_catchPlatform)
            {
                TryGrab();
            }
            else
            {
                ReleaseGrab();
            }
            _grabButtonPressed = false;
        }

        // Если объект захвачен — перемещаем вместе с ним, иначе — свободное перемещение
        if (!_catchPlatform)
        {
            HandleMovement();
        }
        else
        {
            IsMove = false;
            MoveToPlatform(_interval);
        }
    }

    private void HandleMovement()
    {
        IsMove = true;

        // Получаем "сырые" значения ввода
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Если ввода почти нет – обнуляем движение
        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            _moveDirection = Vector3.zero;
        }
        else
        {
            // Вычисляем векторы направления камеры по горизонтали
            Vector3 camForward = _cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = _cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Формируем итоговый вектор движения: вертикальный (вперёд/назад) и горизонтальный (влево/вправо)
            Vector3 desiredMove = (camForward * vertical + camRight * horizontal);
            desiredMove.Normalize();

            // Поворот персонажа в направлении движения (без задержки, можно добавить Slerp для плавности)
            if (desiredMove != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            _moveDirection = desiredMove * _moveSpeed;
        }

        // Обработка гравитации
        if (_controller.isGrounded)
        {
            _verticalVelocity = -1f; // немного опускаем, чтобы CharacterController оставался на земле
        }
        else
        {
            _verticalVelocity -= _gravity * Time.fixedDeltaTime;
        }
        _moveDirection.y = _verticalVelocity;

        // Перемещаем персонажа
        _controller.Move(_moveDirection * Time.fixedDeltaTime);
    }


    // Поиск ближайшего объекта с компонентом MovemtForMOvingObjects и захват его
    private void TryGrab()
    {
        GameObject closestObject = FindClosestMovingObject();
        if (closestObject != null)
        {
            _movementComponent = closestObject.GetComponent<MovemtForMOvingObjects>();
            if (_movementComponent != null)
            {
                _movementComponent.ChangeNeedToMovie();
                _catchPlatform = true;
                Debug.Log("Player grabbed: " + closestObject.name);
            }
        }
        else
        {
            Debug.Log("No moving object found to grab.");
        }
    }

    // Отпускаем захваченный объект
    private void ReleaseGrab()
    {
        if (_movementComponent != null)
        {
            _movementComponent.ChangeNeedToMovie();
            _movementComponent = null;
        }
        _catchPlatform = false;
        Debug.Log("Player released object.");
    }

    // Поиск ближайшего движущегося объекта
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

    // Перемещение игрока вместе с захваченным объектом
    public void MoveToPlatform(float interval)
    {
        if (_movementComponent != null && _movementComponent.Platform != null)
        {
            Transform platform = _movementComponent.Platform;
            Vector3 targetPosition = platform.position - platform.forward * interval;
            Vector3 delta = targetPosition - transform.position;
            _controller.Move(delta);
            Quaternion targetRotation = platform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
        else
        {
            Debug.LogWarning("Platform or movement component is missing!");
        }
    }
}
