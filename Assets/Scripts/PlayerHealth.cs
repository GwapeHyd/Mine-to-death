using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("Damage Settings")]
    [SerializeField] private int damagePerHit = 1;
    [SerializeField] private float invincibilityDuration = 1f; 
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("Events")]
    public UnityEvent<int> onHealthChanged; 
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent onTakeDamage;

    private void Start()
    {
        currentHealth = maxHealth;

        onHealthChanged?.Invoke(currentHealth);
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamageFromMining()
    {
        TakeDamage(damagePerHit);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        onHealthChanged?.Invoke(currentHealth);
        onTakeDamage?.Invoke();

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        onHealthChanged?.Invoke(currentHealth);
    }

    private void Die()
    {
        onDeath?.Invoke();
        
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    public float HealthPercentage => (float)currentHealth / maxHealth;
}