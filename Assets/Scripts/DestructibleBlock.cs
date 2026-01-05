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
    [SerializeField] private KeyCode interactionKey = KeyCode.LeftShift;
    [SerializeField] private string playerTag = "Player";

    [Header("Events")]
    [SerializeField] private UnityEvent onBlockDestroyed;
    [SerializeField] private UnityEvent onBlockHit;

    private bool playerInRange = false;

    private void Start()
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
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            TakeDamage(damagePerHit);
        }
    }

    public void OnPlayerEnterRange()
    {
        playerInRange = true;
        if (actionFeedback != null)
            actionFeedback.SetActive(true);
    }

    public void OnPlayerExitRange()
    {
        playerInRange = false;
        if (actionFeedback != null)
            actionFeedback. SetActive(false);
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
}