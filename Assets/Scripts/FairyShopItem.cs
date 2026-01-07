using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FairyShopItem : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private int cost;
    [SerializeField] private Image itemIcon;


    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button purchaseButton;

    private PlayerHealth playerHealth;
    private bool isPurchased = false;

    public enum PsychosisType
    {
        HeadThrow
    }

    private void Start()
    {
        UpdateUI();

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }

        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void UpdateUI()
    {
        if (nameText != null)
            nameText.text = itemName;

        if (descriptionText != null)
            descriptionText.text = itemDescription;

        if (costText != null)
            costText.text = $"{cost} Deaths";

        if (iconImage != null)
            iconImage.sprite = itemIcon.sprite;
        
        UpdatePurchaseButton();
    }

    private void UpdatePurchaseButton()
    {
        if (purchaseButton != null && playerHealth != null)
        {
            bool canAfford = playerHealth.NumberOfDeaths >= cost;
            purchaseButton.interactable = canAfford;
        }
    }

    private void OnPurchaseClicked()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth instance not found!");
            return;
        }

        if (playerHealth.SpendDeaths(cost))
        {
            ApplyItemEffect();
            Debug.Log($"Purchased {itemName} for {cost} deaths.");
            UpdateCostText();
            UpdatePurchaseButton();
            isPurchased = true;
        }
        else
        {
            Debug.Log("Not enough deaths to purchase this item.");
        }
    }

    private void ApplyItemEffect()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        playerController.EnableHeadThrowing();
    }

    private void OnEnable()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.onDeath.AddListener(OnDeathsChanged);
            UpdatePurchaseButton();
            Debug.Log("<color=red>FairyShopItem registered to PlayerHealth's onDeathCountChanged event.</color>");
        }
    }


    private void OnDisable()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.onDeath.RemoveListener(OnDeathsChanged);
        }
    }

    private void OnDeathsChanged()
    {
        Debug.Log("FairyShopItem detected death count changing...");
        UpdatePurchaseButton();
        Debug.Log("FairyShopItem updated purchase button state.");
    }

    private void UpdateCostText()
    {
        if (costText != null)
        {
            if (isPurchased)
            {
                costText.text = "Purchased";
            }
            else
                costText.text = $"{cost} Deaths";
        }
    }
}
