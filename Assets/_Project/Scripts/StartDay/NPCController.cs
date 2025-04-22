using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCController : MonoBehaviour
{
    public event Action<NPCController> Arrived;   // дошёл до трона
    public event Action<NPCController> Left;      // покинул зал

    [Tooltip("Скорость поворота в градусах/сек")]
    [SerializeField] private float _rotationSpeed = 360f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int SpeedHash = Animator.StringToHash("speed");

    private Transform _exitSpot;
    private bool _reachedSpot;

    public void Init(Transform dialogSpot, Transform exitSpot)
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _exitSpot = exitSpot;

        _reachedSpot = false;
        _agent.isStopped = false;
        _agent.SetDestination(dialogSpot.position);
        // сразу подаем скорость в аниматор (через Update мигом подхватится)
    }

    private void Update()
    {
        // Feed the BlendTree: скорость движения персонажа
        float currentSpeed = _agent.velocity.magnitude;
        _animator.SetFloat(SpeedHash, currentSpeed);

        // Проверяем достижение диалоговой точки
        if (!_reachedSpot &&
            !_agent.pathPending &&
            _agent.remainingDistance <= _agent.stoppingDistance + 0.05f &&
            _agent.velocity.sqrMagnitude < 0.0001f)
        {
            _reachedSpot = true;
            _agent.isStopped = true;
            // velocity упадёт к нулю, BlendTree перейдёт в Idle

            Arrived?.Invoke(this);
        }
    }

    /// <summary>
    /// Запускает последовательность поворота + выход
    /// </summary>
    public void Leave()
    {
        StartCoroutine(LeaveSequence());
    }

    private IEnumerator LeaveSequence()
    {
        // 1) Останавливаем окончательно и сбрасываем скорость
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _animator.SetFloat(SpeedHash, 0f);

        // 2) Плавно разворачиваемся к exitSpot
        yield return StartCoroutine(RotateToPoint(_exitSpot.position));

        // 3) Даем команду идти к выходу
        _agent.isStopped = false;
        _agent.SetDestination(_exitSpot.position);

        // 4) Ждем, пока не дойдём до exitSpot
        yield return StartCoroutine(WaitExit());

        // 5) Останавливаемся и ставим Idle
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _animator.SetFloat(SpeedHash, 0f);

        Left?.Invoke(this);
    }

    private IEnumerator RotateToPoint(Vector3 targetPosition)
    {
        Vector3 flatDir = targetPosition - transform.position;
        flatDir.y = 0;
        if (flatDir.sqrMagnitude < 0.01f) yield break;

        Quaternion targetRot = Quaternion.LookRotation(flatDir);
        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                _rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator WaitExit()
    {
        while (_agent.pathPending ||
               _agent.remainingDistance > _agent.stoppingDistance + 0.05f)
        {
            yield return null;
        }
    }
}
