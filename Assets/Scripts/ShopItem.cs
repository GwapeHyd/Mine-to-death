using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private int itemCost;
    [SerializeField] private Image itemIcon;

    [Header("Item Effects")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private int effectValue;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button purchaseButton;

    public enum ItemType
    {
        HealthUpgrade,
        DamageUpgrade,
        SpeedUpgrade
    }

    private void Start()
    {
        UpdateUI();

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }

    private void UpdateUI()
    {
        if (nameText != null)
            nameText.text = itemName;

        if (descriptionText != null)
            descriptionText.text = itemDescription;

        if (costText != null)
            costText.text = $"{itemCost} Coins";

        if (iconImage != null)
            iconImage.sprite = itemIcon.sprite;
        
        UpdatePurchaseButton();
    }

    private void UpdatePurchaseButton()
    {
        if (purchaseButton != null && CoinManager.Instance != null)
        {
            bool canAfford = CoinManager.Instance.TotalCoins >= itemCost;
            purchaseButton.interactable = canAfford;
        }
    }

    private void OnPurchaseClicked()
    {
        if (CoinManager.Instance == null)
        {
            Debug.LogWarning("CoinManager instance not found!");
            return;
        }

        if (CoinManager.Instance.SpendCoins(itemCost))
        {
            ApplyItemEffect();
            Debug.Log($"Purchased {itemName} for {itemCost} coins.");
            UpdatePurchaseButton();
        }
        else
        {
            Debug.Log("Not enough coins to purchase this item.");
        }
    }

    private void ApplyItemEffect()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        switch (itemType)
        {
            case ItemType.HealthUpgrade:
                if (playerHealth != null)
                {
                    var field = playerHealth.GetType().GetField("maxHealth", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        int currentMax = (int)field.GetValue(playerHealth);
                        field.SetValue(playerHealth, currentMax + effectValue);
                        playerHealth.Heal(effectValue);
                        Debug.Log($"Increased max health by {effectValue}.");
                    }
                }
                break;

            case ItemType.DamageUpgrade:
                if (playerController != null)
                {
                    var field = playerController.GetType().GetField("damagePerHit", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        int currentDamage = (int)field.GetValue(playerController);
                        field.SetValue(playerController, currentDamage + effectValue);
                    }

                    
                    Debug.Log($"Increased damage by {effectValue}.");
                }
                break;

            case ItemType.SpeedUpgrade:
                if (playerController != null)
                {
                    var field = playerController.GetType().GetField("moveSpeed", 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        int currentDamage = (int)field.GetValue(playerController);
                        field.SetValue(playerController, currentDamage + effectValue);
                    }
                }
                break;

            default:
                Debug.LogWarning("Unknown item type.");
                break;
        }
    }

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.AddListener(OnCoinsChanged);
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.RemoveListener(OnCoinsChanged);
        }
    }

    private void OnCoinsChanged(int totalCoins)
    {
        UpdatePurchaseButton();
    }
}
