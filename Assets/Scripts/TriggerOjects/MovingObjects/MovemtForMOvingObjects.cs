using UnityEngine;
using System.Collections.Generic;

public class MovemtForMOvingObjects : MonoBehaviour
{
    [SerializeField] private Transform _platform;
    [SerializeField] private Transform _pointsParent;        
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxTurnAngle = 30f;

    private List<Transform> _points = new List<Transform>(); 
    private List<Quaternion> _rotations = new List<Quaternion>(); 
    private int _currentPointIndex = 0;                      
    private Vector3 _currentTarget;
    private int targetIndex = 0;
    private Quaternion _savedRotation;

    void Start()
    {
        Box box = FindObjectOfType<Box>();
        if (box != null)
        {
            box.OnActivated += Move;
        }
        else
        {
            Debug.LogWarning("Box не найден!");
        }

        foreach (Transform point in _pointsParent)
        {
            _points.Add(point);
            _rotations.Add(Quaternion.identity); 
        }

        if (_points.Count > 0)
        {
            _currentPointIndex = FindNearestPointIndex();
            _platform.position = _points[_currentPointIndex].position;
            _currentTarget = _platform.position;
        }
        _currentPointIndex = FindNearestPointIndex();
        targetIndex = _currentPointIndex + 1;
    }

    void Rotate()
    {
        Vector3 directionToTarget = (_currentTarget - _platform.position).normalized;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            float angleDifference = Quaternion.Angle(_platform.rotation, targetRotation);

            if (angleDifference < _maxTurnAngle)
            {
                _platform.rotation = Quaternion.Slerp(
                    _platform.rotation,
                    targetRotation,
                    Time.deltaTime * _rotationSpeed
                );
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

    void Move()
    {
        if (_points.Count == 0) return;

        Transform targetPoint;

        Vector2 moveInput = InputManager.GetInstance().GetMoveDirection();

        if (moveInput.y == 1) 
        {
            targetPoint = _points[targetIndex];
            _currentTarget = targetPoint.position;

            Rotate();
        }
        else if (moveInput.y == -1) 
        {
            targetPoint = _points[_currentPointIndex];
            _currentTarget = targetPoint.position;
        }
        else
        {
            _currentTarget = _platform.position;
        }


        _platform.position = Vector3.MoveTowards(_platform.position, _currentTarget, _speed * Time.deltaTime);
        if (Vector3.Equals(_platform.position, _currentTarget))
        {
            if (moveInput.y == 1)
            {
                _rotations[_currentPointIndex] = _platform.rotation;

                _savedRotation = _platform.rotation;
                _currentPointIndex = targetIndex;
                targetIndex = DefinePosition(targetIndex, +1);
            }
            else if (moveInput.y == -1) 
            {
                // Плавно возвращаемся к сохраненному углу(почему-то не доворачивает, проблема на будущее)
                //_platform.rotation = Quaternion.Slerp(
                //    _platform.rotation,
                //    _rotations[_currentPointIndex],
                //    Time.deltaTime * _rotationSpeed
                //);

                targetIndex = _currentPointIndex;
                _currentPointIndex = DefinePosition(_currentPointIndex, -1);
                _platform.rotation = _rotations[_currentPointIndex];
            }
            else
            {
                _currentTarget = _platform.position;
            }
        }
    }

    void Update()
    {
        Move();
    }
}
