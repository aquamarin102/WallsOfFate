using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NewMovie : MonoBehaviour
{
    [SerializeField] private Transform _platform;
    [SerializeField] private Transform _pointsParent;        // Объект, содержащий точки
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _rotationSpeed;

    private List<Transform> _points = new List<Transform>(); // Список всех точек
    private int _currentPointIndex = 0;                      // Индекс текущей точки
    private Vector3 _currentTarget;

    void Start()
    {
        foreach (Transform point in _pointsParent)
        {
            _points.Add(point);
        }

        if (_points.Count > 0)
        {
            _currentPointIndex = FindNearestPointIndex();
            _platform.position = _points[_currentPointIndex].position; 
            _currentTarget = _platform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (_pointsParent != null)
        {
            Gizmos.color = Color.green;

            // Рисуем линии между точками для визуализации пути
            Transform previousPoint = null;
            foreach (Transform point in _pointsParent)
            {
                if (previousPoint != null)
                {
                    Gizmos.DrawLine(previousPoint.position, point.position);
                }
                previousPoint = point;
            }
        }
    }

    private int FindNearestPointIndex()
    {
        int nearestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < _points.Count; i++)
        {
            float distance = Vector3.Distance(_points[_currentPointIndex].position, _points[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private int DefinePosition(int index, int delta)
    {
        int newIndex = index + delta;

        if (newIndex < 0)
        {
            newIndex = 0;
        }
        else if (newIndex >= _points.Count)
        {
            newIndex = _points.Count - 1;
        }

        return newIndex;
    }

    void Update()
    {
        // Проверяем наличие точек
        if (_points.Count == 0) return;
        _currentPointIndex = FindNearestPointIndex();

        // Обновляем индекс ближайшей точки
        //_currentPointIndex = FindNearestPointIndex();

        Transform targetPoint;

        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();

        int targetIndex = _currentPointIndex;
        // Проверяем направление ввода игрока
        if (moveInput.y == 1) // Если игрок нажал "вперед"
        {
            targetIndex = DefinePosition(_currentPointIndex, +1);
            targetPoint = _points[targetIndex];
            _currentTarget = targetPoint.position;
        }
        else if (moveInput.y == -1) // Если игрок нажал "назад"
        {
            targetIndex = DefinePosition(_currentPointIndex, -1);

            targetPoint = _points[targetIndex];
            _currentTarget = targetPoint.position;
        }
        else
        {
            _currentTarget = _platform.position;
        }

        //Движение платформы к текущей цели
        if (_currentTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
        _platform.position = Vector3.MoveTowards(_platform.position, _currentTarget, _speed * Time.deltaTime);
        if (Vector3.Equals(_platform.position, _currentTarget))
        {
            _currentPointIndex = targetIndex;
        }

    }
}
