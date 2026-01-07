using Unity.VisualScripting;
using UnityEngine;

public class GlobalSpriteSwitcher : MonoBehaviour
{
    [Header("Swappable Sprites")]
    [SerializeField] private SwappableSprite[] swappableSprites;

    [Header("Animation Controllers")]
    [SerializeField] private RuntimeAnimatorController fairyAnimatorSetA;
    [SerializeField] private RuntimeAnimatorController fairyAnimatorSetB;
    [SerializeField] private RuntimeAnimatorController playerAnimatorSetA;
    [SerializeField] private RuntimeAnimatorController playerAnimatorSetB;

    [Header("TileSets")]
    [SerializeField] private AutoTileSpriteSet tileSetA;
    [SerializeField] private AutoTileSpriteSet tileSetB;

    private int currentIndex = 0;

    [ContextMenu("Swap All Sprites")]
    public void SwapAllSprites()
    {
        foreach (var swappable in swappableSprites)
        {
            swappable.SwapSpriteSet();
        }
        
        UpdateAllAnimators();
        
        currentIndex = 1 - currentIndex; 
        UpdateAllAnimators();
        UpdateAllAutoTileBlocks();
        
    }

    private void UpdateAllAnimators()
    {
        Fairy fairy = FindFirstObjectByType<Fairy>();
        Animator fairyAnim = fairy.GetComponent<Animator>();
        if (fairyAnim != null)
        {
            fairyAnim.runtimeAnimatorController = currentIndex == 0 ? fairyAnimatorSetA : fairyAnimatorSetB;
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.runtimeAnimatorController = currentIndex == 0 ? playerAnimatorSetA : playerAnimatorSetB;
        }
    }

    private void UpdateAllAutoTileBlocks()
    {
        AutoTileBlock[] allBlocks = FindObjectsByType<AutoTileBlock>(FindObjectsSortMode.None);
        AutoTileSpriteSet selectedTileSet = currentIndex == 0 ? tileSetA : tileSetB;

        foreach (var block in allBlocks)
        {
            block.SetSpriteSet(selectedTileSet);
        }
    }
}
