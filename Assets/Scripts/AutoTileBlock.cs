using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class AutoTileBlock : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerHit = 50;
    private int currentHealth;

    [Header("Tile Sprites - Full Health")]
    [SerializeField] private Sprite fullSprite;           // Entouré de tous côtés
    [SerializeField] private Sprite topSprite;            // Vide en haut
    [SerializeField] private Sprite bottomSprite;         // Vide en bas
    [SerializeField] private Sprite leftSprite;           // Vide à gauche
    [SerializeField] private Sprite rightSprite;          // Vide à droite
    [SerializeField] private Sprite topLeftSprite;        // Coin haut-gauche
    [SerializeField] private Sprite topRightSprite;       // Coin haut-droite
    [SerializeField] private Sprite bottomLeftSprite;     // Coin bas-gauche
    [SerializeField] private Sprite bottomRightSprite;    // Coin bas-droite
    [SerializeField] private Sprite horizontalSprite;     // Vide haut et bas
    [SerializeField] private Sprite verticalSprite;       // Vide gauche et droite
    [SerializeField] private Sprite borderLeftSprite;     // Vide haut gauche bas
    [SerializeField] private Sprite borderRightSprite;    // Vide haut droite bas
    [SerializeField] private Sprite borderTopSprite;      // Vide gauche haut droite
    [SerializeField] private Sprite borderBottomSprite;   // Vide gauche bas droite
    [SerializeField] private Sprite isolatedSprite;       // Bloc isolé

    [Header("Tile Sprites - Damaged")]
    [SerializeField] private Sprite damagedSprite;        // Sprite endommagé (50% HP)

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer blockSpriteRenderer;
    [SerializeField] private GameObject actionFeedback;

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.Space;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float tileSize = 1f; // Taille d'un bloc

    [Header("Events")]
    [SerializeField] private UnityEvent onBlockDestroyed;
    [SerializeField] private UnityEvent onBlockHit;

    private bool playerInRange = false;
    private static AutoTileBlock[,] blockGrid; // Grille globale pour optimisation (optionnel)

    private void Start()
    {
        currentHealth = maxHealth;
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        if (blockSpriteRenderer == null)
            blockSpriteRenderer = GetComponent<SpriteRenderer>();

        Invoke(nameof(InitialUpdate), 0.2f);
    }

    private void InitialUpdate()
    {
        UpdateVisuals();
        UpdateNeighbors();
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

        Debug.Log($"Updating visuals for {gameObject.name} ({healthPercentage * 100}%)");

        // État endommagé (50% ou moins)
        if (healthPercentage <= 0.5f && healthPercentage > 0f)
        {
            blockSpriteRenderer.sprite = damagedSprite;
            Debug.Log($"Set to damaged sprite for {gameObject.name}");
            return;
        }

        if (healthPercentage > 0.5f)
        {
            bool hasTop = HasNeighbor(Vector2.up);
            bool hasBottom = HasNeighbor(Vector2.down);
            bool hasLeft = HasNeighbor(Vector2.left);
            bool hasRight = HasNeighbor(Vector2.right);

            Debug.Log($"Neighbors for {gameObject.name} - Top: {hasTop}, Bottom: {hasBottom}, Left: {hasLeft}, Right: {hasRight}");

            Sprite selectedSprite = GetAutoTileSprite();

            Debug.Log($"Selected sprite for {gameObject.name}: {selectedSprite.name}");

            blockSpriteRenderer.sprite = selectedSprite;

            Debug.Log($"{name}: Applied sprite = {(blockSpriteRenderer.sprite != null ? blockSpriteRenderer.sprite.name : "NULL")}");
        }
    }

    private Sprite GetAutoTileSprite()
    {
        // Vérifie les 4 directions
        bool hasTop = HasNeighbor(Vector2.up);
        bool hasBottom = HasNeighbor(Vector2.down);
        bool hasLeft = HasNeighbor(Vector2.left);
        bool hasRight = HasNeighbor(Vector2.right);

        // Détermine le sprite selon les voisins
        int neighborCount = (hasTop ? 1 : 0) + (hasBottom ? 1 : 0) + (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);

        // Isolé (aucun voisin)
        if (neighborCount == 0)
            return isolatedSprite != null ? isolatedSprite : fullSprite;

        // 4 voisins (entouré)
        if (hasTop && hasBottom && hasLeft && hasRight)
            return fullSprite;

        // 3 voisins
        if (!hasTop && hasBottom && hasLeft && hasRight)
            return topSprite != null ? topSprite : fullSprite;
        if (hasTop && !hasBottom && hasLeft && hasRight)
            return bottomSprite != null ? bottomSprite : fullSprite;
        if (hasTop && hasBottom && !hasLeft && hasRight)
            return leftSprite != null ? leftSprite : fullSprite;
        if (hasTop && hasBottom && hasLeft && !hasRight)
            return rightSprite != null ? rightSprite : fullSprite;

        // 2 voisins opposés
        if (hasTop && hasBottom && !hasLeft && !hasRight)
            return verticalSprite != null ? verticalSprite : fullSprite;
        if (!hasTop && !hasBottom && hasLeft && hasRight)
            return horizontalSprite != null ? horizontalSprite :  fullSprite;

        // 2 voisins adjacents (coins)
        if (hasBottom && hasRight && !hasTop && !hasLeft)
            return bottomRightSprite != null ? bottomRightSprite : fullSprite;
        if (hasBottom && hasLeft && !hasTop && !hasRight)
            return bottomLeftSprite != null ? bottomLeftSprite : fullSprite;
        if (hasTop && hasRight && !hasBottom && !hasLeft)
            return topRightSprite != null ? topRightSprite : fullSprite;
        if (hasTop && hasLeft && !hasBottom && !hasRight)
            return topLeftSprite != null ? topLeftSprite :  fullSprite;

        // 1 voisin
        if (hasTop && !hasBottom && !hasLeft && !hasRight)
            return borderTopSprite != null ? borderTopSprite : fullSprite;
        if (hasBottom && !hasTop && !hasLeft && !hasRight)
            return borderBottomSprite != null ?  borderBottomSprite : fullSprite;
        if (hasLeft && !hasTop && !hasBottom && !hasRight)
            return borderLeftSprite != null ? borderLeftSprite : fullSprite;
        if (hasRight && !hasTop && !hasBottom && !hasLeft)
            return borderRightSprite != null ? borderRightSprite : fullSprite;

        return fullSprite;
    }

    private bool HasNeighbor(Vector2 direction)
    {
        Vector2 checkPosition = (Vector2)transform.position + direction * tileSize;
    
        // Utilise OverlapCircle pour plus de tolérance
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, 0.1f);
    
        foreach (Collider2D hit in hits)
        {
            // Ignore les triggers (TriggerZone)
            if (hit.isTrigger) continue;
        
            // Ignore soi-même
            if (hit.transform == transform) continue;
        
            // 🔧 FIX : Utilise GetComponentInParent au lieu de GetComponent
            AutoTileBlock neighborBlock = hit.GetComponentInParent<AutoTileBlock>();
        
            // Si pas trouvé sur le parent, essaye sur l'objet lui-même
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

    private void DestroyBlock()
    {
        onBlockDestroyed?.Invoke();
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        currentHealth = 0;

        // ⚠️ IMPORTANT :  Met à jour les voisins AVANT de détruire
        UpdateNeighbors();
        
        Destroy(gameObject);
    }

    // Met à jour les sprites des blocs voisins
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

    // Méthode publique pour forcer la mise à jour (appelée par les voisins)
    public void ForceUpdateVisuals()
    {
        UpdateVisuals();
    }

    [ContextMenu("Debug Neighbors")]
public void DebugNeighbors()
{
    Debug.Log($"=== {gameObject.name} at {transform.position} ===");
    Debug.Log($"Has TOP (0,1): {HasNeighbor(Vector2.up)}");
    Debug.Log($"Has BOTTOM (0,-1): {HasNeighbor(Vector2.down)}");
    Debug.Log($"Has LEFT (-1,0): {HasNeighbor(Vector2.left)}");
    Debug.Log($"Has RIGHT (1,0): {HasNeighbor(Vector2.right)}");
    
    // Vérifie manuellement
    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    string[] names = { "TOP", "BOTTOM", "LEFT", "RIGHT" };
    
    for (int i = 0; i < directions.Length; i++)
    {
        Vector2 checkPos = (Vector2)transform.position + directions[i] * tileSize;
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, 0.2f);
        
        Debug. Log($"  {names[i]} ({checkPos}): Found {hits.Length} colliders");
        foreach (var hit in hits)
        {
            Debug.Log($"    - {hit.gameObject.name} (Trigger: {hit.isTrigger})");
        }
    }
}
}