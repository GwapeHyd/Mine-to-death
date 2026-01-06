using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Player Reference")]
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHealthDisplay);
            UpdateHealthDisplay(playerHealth.CurrentHealth);
        }
    }

    public void UpdateHealthDisplay(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
    }

    private void OnDestroy()
    {
        // Se désabonner pour éviter les erreurs
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthDisplay);
        }
    }
}