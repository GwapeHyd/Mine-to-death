using UnityEngine;
using UnityEngine.Events;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerHit = 50;
    private int currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite halfHealthSprite;
    [SerializeField] private GameObject actionFeedback;

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode. Space;
    [SerializeField] private string playerTag = "Player";

    [Header("Events (Optional)")]
    [SerializeField] private UnityEvent onBlockDestroyed;
    [SerializeField] private UnityEvent onBlockHit;

    private bool playerInRange = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        if (blockSpriteRenderer == null)
            blockSpriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisuals();
    }

    private void Update()
    {
        if (playerInRange && Input. GetKeyDown(interactionKey))
        {
            TakeDamage(damagePerHit);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerInRange = true;
            
            if (actionFeedback != null)
                actionFeedback.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            playerInRange = false;
            
            if (actionFeedback != null)
                actionFeedback.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); 

        UpdateVisuals();
        
        onBlockHit?.Invoke();

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }
    }

    private void UpdateVisuals()
    {
        if (blockSpriteRenderer == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;

        if (healthPercentage > 0.5f)
        {
            blockSpriteRenderer.sprite = fullHealthSprite;
        }
        else if (healthPercentage > 0f)
        {
            blockSpriteRenderer.sprite = halfHealthSprite;
        }
    }

    private void DestroyBlock()
    {
        onBlockDestroyed?.Invoke();

        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        Destroy(gameObject);
    }

    public void SetDamagePerHit(int newDamage)
    {
        damagePerHit = newDamage;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color. yellow;
        if (GetComponent<Collider2D>() != null)
        {
            Collider2D col = GetComponent<Collider2D>();
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}