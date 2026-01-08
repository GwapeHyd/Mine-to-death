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
    [SerializeField] private RuntimeAnimatorController fairyShopAnimatorSetA;
    [SerializeField] private RuntimeAnimatorController fairyShopAnimatorSetB;

    [Header("TileSets")]
    [SerializeField] private AutoTileSpriteSet tileSetA;
    [SerializeField] private AutoTileSpriteSet tileSetB;
    [SerializeField] private AutoTileSpriteSet specialTileSetA;
    [SerializeField] private AutoTileSpriteSet specialTileSetB;

    [Header("Colors for UI")]
    [SerializeField] private Color[] uiColorSetA;
    [SerializeField] private Color[] uiColorSetB;
    

    private int currentIndex = 0;

    [ContextMenu("Swap All Sprites")]
    public void SwapAllSprites()
    {
        foreach (var swappable in swappableSprites)
        {
            swappable.SwapSpriteSet();
        }
        
        currentIndex = 1 - currentIndex; 
        UpdateAllAnimators();
        UpdateAllAutoTileBlocks();
        UpdateAllSpecialBlocks();
        UpdateAllUIColorSets();

        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.SetIndex(currentIndex);
        }
        
    }

    private void UpdateAllAnimators()
    {
        Fairy fairy = FindFirstObjectByType<Fairy>();
        if (fairy != null)
        {
            Animator fairyAnim = fairy.GetComponent<Animator>();
            if (fairyAnim != null)
            {
                fairyAnim.runtimeAnimatorController = currentIndex == 0 ? fairyAnimatorSetA : fairyAnimatorSetB;
            }
        }
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            Animator playerAnim = player.GetComponent<Animator>();
            if (playerAnim != null)
            {
                playerAnim.runtimeAnimatorController = currentIndex == 0 ? playerAnimatorSetA : playerAnimatorSetB;
            }
        }

        FairyShop fairyShop = FindFirstObjectByType<FairyShop>();
        if (fairyShop != null)
        {
            Animator shopAnim = fairyShop.GetComponent<Animator>();
            if (shopAnim != null)
            {
                shopAnim.runtimeAnimatorController = currentIndex == 0 ? fairyShopAnimatorSetA : fairyShopAnimatorSetB;
            }
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

    private void UpdateAllSpecialBlocks()
    {
        SpecialBlock[] allSpecialBlocks = FindObjectsByType<SpecialBlock>(FindObjectsSortMode.None);
        AutoTileSpriteSet selectedTileSet = currentIndex == 0 ? specialTileSetA : specialTileSetB;

        foreach (var block in allSpecialBlocks)
        {
            block.SetSpriteSet(selectedTileSet);
        }
    }

    private void UpdateAllUIColorSets()
    {
        Color selectedColor = currentIndex == 0 ? uiColorSetA[0] : uiColorSetB[0];
        Color selectedColor2 = currentIndex == 0 ? uiColorSetA[1] : uiColorSetB[1];

        HealthUI healthUI = FindFirstObjectByType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.SetUIColor(selectedColor);
        }
        CoinUI coinUI = FindFirstObjectByType<CoinUI>();
        if (coinUI != null)
        {
            coinUI.SetUIColor(selectedColor);
        }
        Fairy fairy = FindFirstObjectByType<Fairy>();
        if (fairy != null)
        {
            fairy.SetUIColor(selectedColor);
        }
        Shop shop = FindFirstObjectByType<Shop>();
        if (shop != null)
        {
            shop.SetUIColor(selectedColor, selectedColor2);
        }
        ShopMineralDisplay shopMineralDisplay = FindFirstObjectByType<ShopMineralDisplay>();
        if (shopMineralDisplay != null)
        {
            shopMineralDisplay.SetUIColor(selectedColor);
        }
        ShopDeathsDisplay shopDeathsDisplay = FindFirstObjectByType<ShopDeathsDisplay>();
        if (shopDeathsDisplay != null)
        {
            shopDeathsDisplay.SetUIColor(selectedColor);
        }
        SanityUI sanityUI = FindFirstObjectByType<SanityUI>();
        if (sanityUI != null)
        {
            Debug.Log("Updating Sanity UI colors.");
            sanityUI.SetUIColor(selectedColor, selectedColor2);
            Debug.Log("Sanity UI colors updated.");
        }
        FairyShop fairyShop = FindFirstObjectByType<FairyShop>();
        Debug.Log("Updating Fairy Shop UI colors.");
        if (fairyShop != null)
        {
            fairyShop.SetUIColor(selectedColor, selectedColor2);
            Debug.Log("Fairy Shop UI colors updated.");
        }
        else
        {
            Debug.LogWarning("FairyShop instance not found while updating UI colors.");
        }
    }

}
