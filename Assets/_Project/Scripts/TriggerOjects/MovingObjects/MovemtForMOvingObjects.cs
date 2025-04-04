using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovemtForMOvingObjects : MonoBehaviour
{
    [Header("Платформа")]
    [SerializeField] private Transform _platform; // Объект, который будет двигаться (например, ящик или платформа)
    public Transform Platform => _platform;

    [Header("Настройки движения")]
    [SerializeField] private float _moveSpeed = 2.0f;       // Скорость перемещения объекта
    [SerializeField] private float _rotationSpeed = 200f;     // Скорость поворота объекта

    // Флаг, определяющий, нужно ли двигать объект
    private bool _needToMove = false;

    void Update()
    {
        if (_needToMove)
        {
            // Получаем ввод (предполагается, что InputManager реализован в вашем проекте)
            Vector2 input = InputManager.GetInstance().GetMoveDirection();
            Vector3 moveInput = new Vector3(input.x, 0, input.y).normalized;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                // Перемещаем объект
                _platform.position += moveInput * _moveSpeed * Time.deltaTime;

                // Плавно поворачиваем объект в направлении движения
                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                _platform.rotation = Quaternion.RotateTowards(_platform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Метод переключения режима движения объекта.
    // Вызывается из PlayerMoveController при нажатии на кнопку E.
    public void ChangeNeedToMovie()
    {
        _needToMove = !_needToMove;
        Debug.Log("MovemtForMOvingObjects active: " + _needToMove);
    }
}
