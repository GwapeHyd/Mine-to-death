using UnityEngine;
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
        onBlockHit?. Invoke();

        if (currentHealth <= 0)
        {
            DestroyBlock();
        }
    }

    private void UpdateVisuals()
    {
        if (blockSpriteRenderer == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;

        // État endommagé (50% ou moins)
        if (healthPercentage <= 0.5f && healthPercentage > 0f)
        {
            blockSpriteRenderer.sprite = damagedSprite;
        }
        // État intact - utilise auto-tiling
        else if (healthPercentage > 0.5f)
        {
            blockSpriteRenderer.sprite = GetAutoTileSprite();
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
            return topLeftSprite != null ? topLeftSprite : fullSprite;
        if (hasBottom && hasLeft && !hasTop && !hasRight)
            return topRightSprite != null ? topRightSprite : fullSprite;
        if (hasTop && hasRight && !hasBottom && !hasLeft)
            return bottomLeftSprite != null ? bottomLeftSprite : fullSprite;
        if (hasTop && hasLeft && ! hasBottom && !hasRight)
            return bottomRightSprite != null ? bottomRightSprite :  fullSprite;

        // 1 voisin
        if (hasTop && !hasBottom && !hasLeft && ! hasRight)
            return bottomSprite != null ? bottomSprite : fullSprite;
        if (hasBottom && !hasTop && ! hasLeft && !hasRight)
            return topSprite != null ?  topSprite : fullSprite;
        if (hasLeft && !hasTop && !hasBottom && !hasRight)
            return rightSprite != null ? rightSprite : fullSprite;
        if (hasRight && !hasTop && !hasBottom && !hasLeft)
            return leftSprite != null ? leftSprite : fullSprite;

        return fullSprite;
    }

    private bool HasNeighbor(Vector2 direction)
    {
        Vector2 checkPosition = (Vector2)transform.position + direction * tileSize;
        
        // Utilise OverlapPoint pour vérifier s'il y a un bloc intact
        Collider2D hit = Physics2D.OverlapPoint(checkPosition);
        
        if (hit != null)
        {
            AutoTileBlock neighborBlock = hit.GetComponent<AutoTileBlock>();
            
            // Retourne true si le voisin existe ET est intact (>50% HP)
            if (neighborBlock != null)
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
            Collider2D hit = Physics2D.OverlapPoint(checkPosition);
            
            if (hit != null)
            {
                AutoTileBlock neighbor = hit.GetComponent<AutoTileBlock>();
                if (neighbor != null)
                {
                    neighbor.UpdateVisuals(); // Force la mise à jour du sprite
                }
            }
        }
    }

    // Méthode publique pour forcer la mise à jour (appelée par les voisins)
    public void ForceUpdateVisuals()
    {
        UpdateVisuals();
    }
}