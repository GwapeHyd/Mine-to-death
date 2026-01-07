using UnityEngine;
using UnityEngine.Events;

public class AutoTileBlock : MonoBehaviour
{
    [Header("BlockType")]
    [SerializeField] private bool isSpecialBlock = false;
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("AutoTile Sprites")]
    [SerializeField] private AutoTileSpriteSet spriteSet;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private GameObject actionFeedback;
    [SerializeField] private AudioClip hintSound;


    [Header("BonusBlock")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private float tileSize = 1f; 

    [Header("Events")]
    public UnityEvent onBlockDestroyed;
    [SerializeField] private UnityEvent onBlockHit;

    
    private void Start()
    {
        currentHealth = maxHealth;
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        if (blockSpriteRenderer == null)
            blockSpriteRenderer = GetComponent<SpriteRenderer>();

        if(!isSpecialBlock)
        {
            Invoke(nameof(InitialUpdate), 0.2f);
        }
    }

    public void SetSpriteSet(AutoTileSpriteSet newSpriteSet)
    {
        spriteSet = newSpriteSet;
        UpdateVisuals();
        UpdateNeighbors();
    }

    private void InitialUpdate()
    {
        UpdateVisuals();
        UpdateNeighbors();
    }

    public void OnPlayerEnterRange()
    {
        if (actionFeedback != null)
            actionFeedback.SetActive(true);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.AddBlockInRange(this);
        }
    }

    public void OnPlayerExitRange()
    {
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ClearCurrentBlock(this);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateVisuals();
        onBlockHit?. Invoke();

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }
    }

    private void UpdateVisuals()
    {
        if (blockSpriteRenderer == null) 
        {
            Debug.LogWarning("Block SpriteRenderer is not assigned!");
            return;
        }

        float healthPercentage = (float)currentHealth / maxHealth;

        // État endommagé (50% ou moins)
        if (healthPercentage <= 0.5f && healthPercentage > 0f)
        {
            blockSpriteRenderer.sprite = spriteSet.damagedSprite;
            Debug.Log($"Set to damaged sprite for {gameObject.name}");
            return;
        }

        if (healthPercentage > 0.5f)
        {
            Sprite selectedSprite = GetAutoTileSprite();
            blockSpriteRenderer.sprite = selectedSprite;
        }
    }

    private Sprite GetAutoTileSprite()
    {
        if (spriteSet == null)
        {
            Debug.LogWarning("Sprite set is not assigned!");
            return null;
        }

        bool hasTop = HasNeighbor(Vector2.up);
        bool hasBottom = HasNeighbor(Vector2.down);
        bool hasLeft = HasNeighbor(Vector2.left);
        bool hasRight = HasNeighbor(Vector2.right);

        int neighborCount = (hasTop ? 1 : 0) + (hasBottom ? 1 : 0) + (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);

        // Isolé (aucun voisin)
        if (neighborCount == 0)
            return spriteSet.isolatedSprite != null ? spriteSet.isolatedSprite : spriteSet.fullSprite;

        // 4 voisins (entouré)
        if (hasTop && hasBottom && hasLeft && hasRight)
            return spriteSet.fullSprite;
        // 3 voisins
        if (!hasTop && hasBottom && hasLeft && hasRight)
            return spriteSet.topSprite != null ? spriteSet.topSprite : spriteSet.fullSprite;
        if (hasTop && !hasBottom && hasLeft && hasRight)
            return spriteSet.bottomSprite != null ? spriteSet.bottomSprite : spriteSet.fullSprite;
        if (hasTop && hasBottom && !hasLeft && hasRight)
            return spriteSet.leftSprite != null ? spriteSet.leftSprite : spriteSet.fullSprite;
        if (hasTop && hasBottom && hasLeft && !hasRight)
            return spriteSet.rightSprite != null ? spriteSet.rightSprite : spriteSet.fullSprite;

        // 2 voisins opposés
        if (hasTop && hasBottom && !hasLeft && !hasRight)
            return spriteSet.verticalSprite != null ? spriteSet.verticalSprite : spriteSet.fullSprite;
        if (!hasTop && !hasBottom && hasLeft && hasRight)
            return spriteSet.horizontalSprite != null ? spriteSet.horizontalSprite :  spriteSet.fullSprite;

        // 2 voisins adjacents (coins)
        if (hasBottom && hasRight && !hasTop && !hasLeft)
            return spriteSet.bottomRightSprite != null ? spriteSet.bottomRightSprite : spriteSet.fullSprite;
        if (hasBottom && hasLeft && !hasTop && !hasRight)
            return spriteSet.bottomLeftSprite != null ? spriteSet.bottomLeftSprite : spriteSet.fullSprite;
        if (hasTop && hasRight && !hasBottom && !hasLeft)
            return spriteSet.topRightSprite != null ? spriteSet.topRightSprite : spriteSet.fullSprite;
        if (hasTop && hasLeft && !hasBottom && !hasRight)
            return spriteSet.topLeftSprite != null ? spriteSet.topLeftSprite :  spriteSet.fullSprite;

        // 1 voisin
        if (hasTop && !hasBottom && !hasLeft && !hasRight)
            return spriteSet.borderTopSprite != null ? spriteSet.borderTopSprite : spriteSet.fullSprite;
        if (hasBottom && !hasTop && !hasLeft && !hasRight)
            return spriteSet.borderBottomSprite != null ?  spriteSet.borderBottomSprite : spriteSet.fullSprite;
        if (hasLeft && !hasTop && !hasBottom && !hasRight)
            return spriteSet.borderLeftSprite != null ? spriteSet.borderLeftSprite : spriteSet.fullSprite;
        if (hasRight && !hasTop && !hasBottom && !hasLeft)
            return spriteSet.borderRightSprite != null ? spriteSet.borderRightSprite : spriteSet.fullSprite;

        return spriteSet.fullSprite;
    }

    private bool HasNeighbor(Vector2 direction)
    {
        Vector2 checkPosition = (Vector2)transform.position + direction * tileSize;
    
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, 0.1f);
    
        foreach (Collider2D hit in hits)
        {
            if (hit.isTrigger) continue;
        
            if (hit.transform == transform) continue;
        
            AutoTileBlock neighborBlock = hit.GetComponentInParent<AutoTileBlock>();
        
            if (neighborBlock == null)
                neighborBlock = hit.GetComponent<AutoTileBlock>();
        
            if (neighborBlock != null && neighborBlock != this)
            {
                if (neighborBlock.isSpecialBlock) continue;

                float neighborHealth = (float)neighborBlock.currentHealth / neighborBlock.maxHealth;
                return neighborHealth > 0.5f;
            }
        }
    
        return false;
    }

    private void DestroyBlock()
    {
        onBlockDestroyed?.Invoke();
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        currentHealth = 0;

        if (isSpecialBlock && collectiblePrefab != null)
        {
            Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ClearCurrentBlock(this);
        }

        if (hintSound != null)
        {
            AudioManager.Instance.PlaySound(hintSound, 0.1f);
        }

        UpdateNeighbors();
        
        Destroy(gameObject);
    }

    private void UpdateNeighbors()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        
        foreach (Vector2 dir in directions)
        {
            Vector2 checkPosition = (Vector2)transform.position + dir * tileSize;
            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, 0.1f);
            
            foreach (var hit in hits)
            {
                if (hit != null)
                {
                    if (hit == null) continue;

                    if (hit.isTrigger) continue;

                    if (hit.transform == transform) continue;  

                    AutoTileBlock neighbor = hit.GetComponent<AutoTileBlock>();

                    if (neighbor == null)
                    {
                        neighbor = hit.GetComponent<AutoTileBlock>();
                    }

                    if (neighbor != null && neighbor != this)
                    {
                        neighbor.UpdateVisuals();
                    }
                }
            }
        }
    }

    public void ForceUpdateVisuals()
    {
        UpdateVisuals();
    }

}