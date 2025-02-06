using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Slider healthBarPrefab;  // Префаб полоски здоровья
    public Transform healthBarParent;  // Родительский объект для всех полосок здоровья на Canvas

    private Slider healthBar;
    private MiniGamePlayer player;  // Ссылка на компонент Player

    void Start()
    {
        // Получаем компонент Player у объекта
        player = GetComponent<MiniGamePlayer>();
        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        // Создаем полоску здоровья
        healthBar = Instantiate(healthBarPrefab, healthBarParent);
        healthBar.gameObject.SetActive(true);

        // Инициализируем полоску здоровья
        UpdateHealthBar();
    }

    void Update()
    {
        if (player != null)
        {
            // Обновляем полоску здоровья на каждом кадре
            UpdateHealthBar();
            if (player.Health == 0)
            {
                RemoveHealthBar(); // Убираем полоску здоровья, если здоровье = 0
                this.gameObject.SetActive(false); // Деактивируем текущий объект (на котором висит скрипт)
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null && player != null)
        {
            // Получаем текущее здоровье и максимальное здоровье игрока
            float currentHealth = player.Health;
            float maxHealth = player.MaxHealth;

            // Обновляем значение слайдера
            healthBar.value = currentHealth / maxHealth;

            // Обновляем текст (если есть) для отображения чисел здоровья
            Text healthBarText = healthBar.GetComponentInChildren<Text>();
            if (healthBarText != null)
            {
                healthBarText.text = $"{Mathf.Ceil(currentHealth)} / {Mathf.Ceil(maxHealth)}";
            }
        }
    }

    public void RemoveHealthBar()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
    }
}
