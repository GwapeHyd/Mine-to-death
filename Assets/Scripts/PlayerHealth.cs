using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float respawnDelay = 1f;
    private int currentHealth;
    private int numberOfDeaths = 0; 
    private Vector3 spawnPosition;
    private bool isDead;
    [Header("Damage Settings")]
    [SerializeField] private int damagePerHit = 1;
    [SerializeField] private float invincibilityDuration = 1f; 
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;
    [Header("Events")]
    public UnityEvent<int> onHealthChanged; 
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent onTakeDamage;

    private Rigidbody2D rb;
    private Transform respawnPoint;
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = respawnPoint != null ? respawnPoint.position : transform.position;

        onHealthChanged?. Invoke(currentHealth);
    }
    private void Update()
    {
        if (isInvincible && !isDead)
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
        if (isInvincible || isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player took {amount} damage. Current health: {currentHealth}/{maxHealth}");
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
        Debug.Log($"Player healed {amount}. Current health: {currentHealth}/{maxHealth}");
    }
    private void Die()
    {
        if (isDead) return;

        Debug.Log("Player died!");
        numberOfDeaths++;
        isDead = true;
        onDeath?.Invoke();
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
        controller.enabled = false;
        }

    StartCoroutine(RespawnRoutine());
    }   

    private System.Collections.IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        transform.position = spawnPosition;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        currentHealth = maxHealth;
        invincibilityTimer = 0f;
        isInvincible = false;
        isDead = false;
        onHealthChanged?.Invoke(currentHealth);
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
        }
    }
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int NumberOfDeaths => numberOfDeaths;
    public bool IsInvincible => isInvincible;
    public float HealthPercentage => (float)currentHealth / maxHealth;
}