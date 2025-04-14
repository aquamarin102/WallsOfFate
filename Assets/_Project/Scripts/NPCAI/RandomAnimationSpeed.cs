using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    // Минимальное и максимальное значения для случайного множителя скорости.
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Назначаем случайное значение скорости анимации.
            animator.speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
