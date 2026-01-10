using UnityEditor;
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
    public UnityEvent onDeath;
    public UnityEvent<int> onDeathCountChanged;
    public UnityEvent onTakeDamage;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform respawnPoint;
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spawnPosition = respawnPoint != null ? respawnPoint.position : transform.position;

        LoadMaxHealth();
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

        if (transform.position.y < -10f)
        {
            Die();
        }
    }
    public void TakeDamageFromMining()
    {
        TakeDamage(damagePerHit);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead) return;
        Debug.Log("Player took damage: " + amount);
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
    public void Die()
    {
        if (isDead) return;

        numberOfDeaths++;
        onDeathCountChanged?.Invoke(numberOfDeaths);
        isDead = true;
        onDeath?.Invoke();
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
        controller.enabled = false;
        }

    StartCoroutine(DeathAndRespawn());
    }   

    private System.Collections.IEnumerator DeathAndRespawn()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        StartCoroutine(RespawnRoutine());
        yield return null;
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
        Heal(maxHealth);
        invincibilityTimer = 0f;
        isInvincible = false;
        isDead = false;
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
            controller.SetIsAttacking(false);
        }
    }

    public bool SpendDeaths(int amount)
    {
        if (numberOfDeaths >= amount)
        {
            numberOfDeaths -= amount;
            onDeath?.Invoke();
            return true;
        }
        return false;
    }

    public void AddDeaths(int amount)
    {
        numberOfDeaths += amount;
        onDeathCountChanged?.Invoke(numberOfDeaths);
    }

    public void SaveMaxHealth()
    {
        PlayerPrefs.SetInt("MaxHealth", maxHealth);
        PlayerPrefs.Save();
    }

    public void LoadMaxHealth()
    {
        if (PlayerPrefs.HasKey("MaxHealth"))
        {
            maxHealth = PlayerPrefs.GetInt("MaxHealth", 10);
            currentHealth = maxHealth;
        }
        else
        {
            Debug.LogWarning("No saved max health found in PlayerPrefs. Using default.");
        }
    }

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int NumberOfDeaths => numberOfDeaths;
    public bool IsInvincible => isInvincible;
    public float HealthPercentage => (float)currentHealth / maxHealth;
}