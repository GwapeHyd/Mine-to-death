using UnityEngine;
using TMPro;

public class ShopDeathsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathsText;

    private void OnEnable()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onDeathCountChanged.AddListener(UpdateDisplay);
            UpdateDisplay(playerHealth.NumberOfDeaths);
        }
    }

    private void OnDisable()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onDeathCountChanged.RemoveListener(UpdateDisplay);
        }
    }

    private void UpdateDisplay(int totalDeaths)
    {
        if (deathsText != null)
        {
            deathsText.text = $"{totalDeaths}";
        }
    }

    public void SetUIColor(Color newColor)
    {
        if (deathsText != null)
        {
            deathsText.color = newColor;
        }
    }
}