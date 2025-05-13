using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArcherIdleAsShoot : MonoBehaviour
{
    [Header("Кто цель")]
    public Transform target;

    [Header("Тэг, которым помечен игрок")]
    public string playerTag = "Player";

    [Header("Диапазон скоростей анимации")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.2f;

    private Animator animator;
    private bool isBlocked;
    private float currentSpeed;

    // Здесь имя вашего состояния Idle, в которое загружен клип Shoot
    private readonly int idleStateHash = Animator.StringToHash("Idle");

    void Start()
    {
        animator = GetComponent<Animator>();
        // Стартуем сразу «выстрелом» (Idle-клипом) на рандомной скорости
        ResumeLoop();
    }

    void Update()
    {
        if (animator == null || target == null)
            return;

        // Луч от чуть выше ног к мишени
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target.position - origin).normalized;
        float dist = Vector3.Distance(origin, target.position);

        bool nowBlocked = false;
        if (Physics.Raycast(origin, dir, out var hit, dist))
            nowBlocked = hit.collider.CompareTag(playerTag);

        // Вход в блокировку: первый раз отматываем на 0-й кадр и замираем
        if (nowBlocked && !isBlocked)
        {
            isBlocked = true;
            animator.Play(idleStateHash, 0, 0f);   // моментальный переход на начало клипа
            animator.Update(0f);                  // применить первый кадр сразу
            animator.speed = 0f;                  // «заморозить» анимацию
            return;
        }
        // Выход из блокировки: возобновляем цикл на новой скорости
        else if (!nowBlocked && isBlocked)
        {
            isBlocked = false;
            ResumeLoop();
        }
    }

    // Запуск/возобновление зацикленного Shoot-клипа с новой скоростью
    private void ResumeLoop()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        animator.speed = currentSpeed;
        animator.Play(idleStateHash, 0, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, target.position);
    }
}
