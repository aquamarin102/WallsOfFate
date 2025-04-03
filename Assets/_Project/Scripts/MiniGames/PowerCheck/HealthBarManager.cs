using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    private Slider _healthBar;
    private MiniGamePlayer player;

    public void SetHealthBar(Slider healthBar)
    {
        _healthBar = healthBar;
    }

    void Start()
    {
        if(this.gameObject.activeSelf) _healthBar.gameObject.SetActive(true);

        player = GetComponent<MiniGamePlayer>();
        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        if (_healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned!");
            return;
        }

        UpdateHealthBar();
    }

    void OnDisable()
    {
        RemoveHealthBar();
    }

    void Update()
    {
        if (this.gameObject.activeSelf) _healthBar.gameObject.SetActive(true);
        if (player != null && _healthBar != null)
        {
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        float currentHealth = player.Health;
        float maxHealth = player.MaxHealth;
        _healthBar.value = currentHealth / maxHealth;

        Text healthBarText = _healthBar.GetComponentInChildren<Text>();
        if (healthBarText != null)
        {
            healthBarText.text = $"{Mathf.Ceil(currentHealth)} / {Mathf.Ceil(maxHealth)}";
        }
    }

    public void RemoveHealthBar()
    {
        if (_healthBar != null)
        {
            Destroy(_healthBar.gameObject);
        }
    }
}
