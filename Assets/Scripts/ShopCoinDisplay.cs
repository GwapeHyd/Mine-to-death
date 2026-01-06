using UnityEngine;
using TMPro;

public class ShopCoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.AddListener(UpdateDisplay);
            UpdateDisplay(CoinManager.Instance.TotalCoins);
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.RemoveListener(UpdateDisplay);
        }
    }

    private void UpdateDisplay(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text = $"Your Coins: {totalCoins}";
        }
    }
}