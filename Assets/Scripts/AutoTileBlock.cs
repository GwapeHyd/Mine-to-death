using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum SpecialType
{
    HintFar,
    HintNear,
    CoinBlock,
    MineralBlock
}
public class AutoTileBlock : MonoBehaviour
{
    [Header("BlockType")]
    [SerializeField] protected bool isSpecialBlock = false;
    [SerializeField] protected SpecialType SpecialType;

    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;

    [Header("AutoTile Sprites")]
    [SerializeField] private AutoTileSpriteSet spriteSet;
    [SerializeField] private bool forceOwnSprite = false;
    [SerializeField] private Sprite ownSprite = null;

    [Header("Feedback")]
    [SerializeField] protected SpriteRenderer blockSpriteRenderer;
    [SerializeField] protected GameObject actionFeedback;

    [Header("Settings")]
    [SerializeField] protected float tileSize = 1f;

    [Header("Events")]
    public UnityEvent onBlockDestroyed;
    [SerializeField] protected UnityEvent onBlockHit;

    private static Dictionary<Vector2, AutoTileBlock> allBlocks = new Dictionary<Vector2, AutoTileBlock>();
    private bool registeredInGrid = false;
    public Vector2Int GridPosition { get; private set; }
    
    private void Start()
    {
        currentHealth = maxHealth;
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        if (blockSpriteRenderer == null)
            blockSpriteRenderer = GetComponent<SpriteRenderer>();

        if (!isSpecialBlock)
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
        onBlockHit?.Invoke();

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }
    }

    protected virtual void UpdateVisuals()
    {
        if (blockSpriteRenderer == null) 
        {
            return;
        }

        if(spriteSet == null)
        {
            return;
        }

        if (isSpecialBlock)
        {
            switch (SpecialType)
            {
                case SpecialType.HintFar:
                    blockSpriteRenderer.sprite = spriteSet.hintFarSprite;
                    break;
                case SpecialType.HintNear:
                    blockSpriteRenderer.sprite = spriteSet.hintNearSprite;
                    break;
                case SpecialType.CoinBlock:
                    blockSpriteRenderer.sprite = spriteSet.coinBlockSprite;
                    break;
            }
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
        if (forceOwnSprite && ownSprite != null)
        {
            blockSpriteRenderer.sprite = ownSprite;
        }
        Sprite interactionSprite = GetComponentInChildren<SpriteRenderer>()?.sprite;
        if (interactionSprite == null || interactionSprite != GetFeedbackSprite())
        {
            UpdateInteractionFeedback();
        }
    }

    private void UpdateInteractionFeedback()
    {
        if (actionFeedback == null) return;

        SpriteRenderer feedbackSpriteRenderer = actionFeedback.GetComponent<SpriteRenderer>();
        if (feedbackSpriteRenderer != null)
        {
            feedbackSpriteRenderer.sprite = GetFeedbackSprite();
        }
    }

    private Sprite GetFeedbackSprite()
    {
        if (spriteSet == null)
        {
            Debug.LogWarning("Sprite set is not assigned!");
            return null;
        }

        return spriteSet.actionFeedbackSprite;
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

        bool hasTopLeft = HasNeighbor(new Vector2(-1, 1));
        bool hasTopRight = HasNeighbor(new Vector2(1, 1));
        bool hasBottomLeft = HasNeighbor(new Vector2(-1, -1));
        bool hasBottomRight = HasNeighbor(new Vector2(1, -1));

        int neighborCount = (hasTop ? 1 : 0) + (hasBottom ? 1 : 0) + (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);

        // Isolé (aucun voisin)
        if (neighborCount == 0)
            return spriteSet.isolatedSprite != null ? spriteSet.isolatedSprite : spriteSet.fullSprite;

        // 4 voisins (entouré)
        if (hasTop && hasBottom && hasLeft && hasRight)
        {
            // 4 diags
            if (hasTopLeft && hasTopRight && hasBottomLeft && hasBottomRight)
                return spriteSet.fullSprite;
            
            // 3 diag
            if (!hasTopLeft && hasTopRight && hasBottomLeft && hasBottomRight)
                return spriteSet.innerTopLeftSprite != null ? spriteSet.innerTopLeftSprite : spriteSet.fullSprite;
            if (!hasTopRight && hasTopLeft && hasBottomLeft && hasBottomRight)
                return spriteSet.innerTopRightSprite != null ? spriteSet.innerTopRightSprite : spriteSet.fullSprite;
            if (!hasBottomLeft && hasTopLeft && hasTopRight && hasBottomRight)
                return spriteSet.innerBottomLeftSprite != null ? spriteSet.innerBottomLeftSprite : spriteSet.fullSprite;
            if (!hasBottomRight && hasTopLeft && hasTopRight && hasBottomLeft)
                return spriteSet.innerBottomRightSprite != null ? spriteSet.innerBottomRightSprite : spriteSet.fullSprite;

            // 2 diag opposés
            if (!hasTopLeft && !hasBottomRight && hasTopRight && hasBottomLeft)
                return spriteSet.innerDiagTopLeftSprite != null ? spriteSet.innerDiagTopLeftSprite : spriteSet.fullSprite;
            if (!hasTopRight && !hasBottomLeft && hasTopLeft && hasBottomRight)
                return spriteSet.innerDiagTopRightSprite != null ? spriteSet.innerDiagTopRightSprite : spriteSet.fullSprite;
            
            // 2 diag adjacents
            if (!hasTopLeft && !hasTopRight && hasBottomLeft && hasBottomRight)
                return spriteSet.innerTopSprite != null ? spriteSet.innerTopSprite : spriteSet.fullSprite;
            if (!hasTopLeft && hasTopRight && !hasBottomLeft && hasBottomRight)
                return spriteSet.innerLeftSprite != null ? spriteSet.innerLeftSprite : spriteSet.fullSprite;
            if (hasTopLeft && !hasTopRight && hasBottomLeft && !hasBottomRight)
                return spriteSet.innerRightSprite != null ? spriteSet.innerRightSprite : spriteSet.fullSprite;
            if (hasTopLeft && hasTopRight && !hasBottomLeft && !hasBottomRight)
                return spriteSet.innerBottomSprite != null ? spriteSet.innerBottomSprite : spriteSet.fullSprite;
           
            // 1 diag
            if (!hasTopLeft && !hasTopRight && !hasBottomLeft && hasBottomRight)
                return spriteSet.diagBottomLeftSprite != null ? spriteSet.diagBottomLeftSprite : spriteSet.fullSprite;
            if (!hasTopLeft && !hasTopRight && hasBottomLeft && !hasBottomRight)
                return spriteSet.diagBottomRightSprite != null ? spriteSet.diagBottomRightSprite : spriteSet.fullSprite;
            if (!hasTopLeft && hasTopRight && !hasTopLeft && !hasTopRight)
                return spriteSet.diagTopLeftSprite != null ? spriteSet.diagTopLeftSprite : spriteSet.fullSprite;
            if (hasTopLeft && !hasTopRight && !hasBottomLeft && !hasBottomRight)
                return spriteSet.diagTopRightSprite != null ? spriteSet.diagTopRightSprite : spriteSet.fullSprite;

            return spriteSet.fullSprite;
        }

        // 3 voisins sans le haut
        if (!hasTop && hasBottom && hasLeft && hasRight)
        {
            if (!hasBottomLeft)
                return spriteSet.topInnerLeftSprite != null ? spriteSet.topInnerLeftSprite : spriteSet.fullSprite;    
            if (!hasBottomRight)
                return spriteSet.topInnerRightSprite != null ? spriteSet.topInnerRightSprite : spriteSet.fullSprite;

            return spriteSet.topSprite != null ? spriteSet.topSprite : spriteSet.fullSprite;
            
        }
        if (hasTop && !hasBottom && hasLeft && hasRight)
        {
            if (!hasTopLeft)
                return spriteSet.bottomInnerLeftSprite != null ? spriteSet.bottomInnerLeftSprite : spriteSet.fullSprite;   
            if (!hasTopRight)
                return spriteSet.bottomInnerRightSprite != null ? spriteSet.bottomInnerRightSprite : spriteSet.fullSprite;

            return spriteSet.bottomSprite != null ? spriteSet.bottomSprite : spriteSet.fullSprite;
        }
        if (hasTop && hasBottom && !hasLeft && hasRight)
        {
            if (!hasTopRight)
                return spriteSet.leftInnerTopSprite != null ? spriteSet.leftInnerTopSprite : spriteSet.fullSprite;
            if (!hasBottomRight)
                return spriteSet.leftInnerBottomSprite != null ? spriteSet.leftInnerBottomSprite : spriteSet.fullSprite;
            
            return spriteSet.leftSprite != null ? spriteSet.leftSprite : spriteSet.fullSprite;
        }
        if (hasTop && hasBottom && hasLeft && !hasRight)
        {
            if (!hasTopLeft)
                return spriteSet.rightInnerTopSprite != null ? spriteSet.rightInnerTopSprite : spriteSet.fullSprite;
            if (!hasBottomLeft)
                return spriteSet.rightInnerBottomSprite != null ? spriteSet.rightInnerBottomSprite : spriteSet.fullSprite;

            return spriteSet.rightSprite != null ? spriteSet.rightSprite : spriteSet.fullSprite;
        }

        // 2 voisins opposés
        if (hasTop && hasBottom && !hasLeft && !hasRight)
            return spriteSet.verticalSprite != null ? spriteSet.verticalSprite : spriteSet.fullSprite;
        if (!hasTop && !hasBottom && hasLeft && hasRight)
            return spriteSet.horizontalSprite != null ? spriteSet.horizontalSprite :  spriteSet.fullSprite;

        // 2 voisins adjacents (coins)
        if (hasBottom && hasRight && !hasTop &&!hasLeft)
        {
            if (!hasBottomRight)
                return spriteSet.interInnerTopLeftSprite != null ? spriteSet.interInnerTopLeftSprite : spriteSet.fullSprite;
            return spriteSet.topLeftSprite != null ? spriteSet.topLeftSprite : spriteSet.fullSprite;
        }
        if (hasBottom && hasLeft && !hasTop && !hasRight)
        {
            if (!hasBottomLeft)
                return spriteSet.interInnerTopRightSprite != null ? spriteSet.interInnerTopRightSprite : spriteSet.fullSprite;
            return spriteSet.topRightSprite != null ? spriteSet.topRightSprite : spriteSet.fullSprite;
        }
        if (hasTop && hasRight && !hasBottom && !hasLeft)
        {
            if (!hasTopRight)
                return spriteSet.interInnerBottomLeftSprite != null ? spriteSet.interInnerBottomLeftSprite : spriteSet.fullSprite;
            return spriteSet.bottomLeftSprite != null ? spriteSet.bottomLeftSprite : spriteSet.fullSprite;
        }
        if (hasTop && hasLeft && !hasBottom && !hasRight)
        {
            if (!hasTopLeft)
                return spriteSet.interInnerBottomRightSprite != null ? spriteSet.interInnerBottomRightSprite : spriteSet.fullSprite;
            return spriteSet.bottomRightSprite != null ? spriteSet.bottomRightSprite : spriteSet.fullSprite;
        }

        // 1 voisin
        if (hasTop && !hasBottom && !hasLeft && !hasRight)
            return spriteSet.borderBottomSprite != null ? spriteSet.borderBottomSprite : spriteSet.fullSprite;
        if (hasBottom && !hasTop && !hasLeft && !hasRight)
            return spriteSet.borderTopSprite != null ?  spriteSet.borderTopSprite : spriteSet.fullSprite;
        if (hasLeft && !hasTop && !hasBottom && !hasRight)
            return spriteSet.borderRightSprite != null ? spriteSet.borderRightSprite : spriteSet.fullSprite;
        if (hasRight && !hasTop && !hasBottom && !hasLeft)
            return spriteSet.borderLeftSprite != null ? spriteSet.borderLeftSprite : spriteSet.fullSprite;


        return spriteSet.fullSprite;
    }

    private bool HasNeighbor(Vector2 direction)
    {
        if (registeredInGrid)
        {
            Vector2Int offset = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
            Vector2Int target = GridPosition + offset;
            if (TryGetBlockAt(target, out var neighborBlock))
            {
                float neighborHealth = (float)neighborBlock.currentHealth / neighborBlock.maxHealth;
                return neighborHealth > 0.5f;
            }
            return false;
        }

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
                float neighborHealth = (float)neighborBlock.currentHealth / neighborBlock.maxHealth;
                return neighborHealth > 0.5f;
            }
        }
    
        return false;
    }

    protected virtual void DestroyBlock()
    {
        onBlockDestroyed?.Invoke();
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        currentHealth = 0;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ClearCurrentBlock(this);
        }

        UpdateNeighbors();
        
        Destroy(gameObject);
    }

    private void UpdateNeighbors()
    {
        if (registeredInGrid)
        {
            Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                                                   new Vector2Int(-1,1), new Vector2Int(1,1), new Vector2Int(-1,-1), new Vector2Int(1,-1) };
            foreach (var d in dirs)
            {
                Vector2Int target = GridPosition + d;
                if (TryGetBlockAt(target, out var neighbor) && neighbor != null)
                    neighbor.UpdateVisuals();
            }
            return;
        }
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

        Vector2[] diagDirs = { new Vector2(-1,1), new Vector2(1,1), new Vector2(-1,-1), new Vector2(1,-1) };
        foreach (Vector2 d in diagDirs)
        {
            Vector2 checkPosition = (Vector2)transform.position + d * tileSize;
            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, 0.1f);
            foreach (var hit in hits)
            {
                if (hit == null) continue;
                if (hit.isTrigger) continue;
                if (hit.transform == transform) continue;
                AutoTileBlock neighbor = hit.GetComponent<AutoTileBlock>();
                if (neighbor == null) neighbor = hit.GetComponentInParent<AutoTileBlock>();
                if (neighbor != null && neighbor != this)
                {
                    neighbor.UpdateVisuals();
                }
            }
        }
    }

    public void ForceUpdateVisuals()
    {
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        if (registeredInGrid && allBlocks.TryGetValue(GridPosition, out var existingBlock))
        {
            if (existingBlock == this)
            {
                allBlocks.Remove(GridPosition);
            }
        }
    }

    public void RegisterGridPosition(Vector2Int gridPos)
    {
        if (!registeredInGrid && !allBlocks.TryGetValue(gridPos, out var existingBlock))
        {
            GridPosition = gridPos;
            allBlocks[gridPos] = this;
            registeredInGrid = true;
        }
    }

    private static bool TryGetBlockAt(Vector2Int pos, out AutoTileBlock block)
    {
        return allBlocks.TryGetValue(pos, out block);
    }

}