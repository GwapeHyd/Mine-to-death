using UnityEngine;
using TMPro;

public class ShopMineralDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mineralText;

    private void OnEnable()
    {
        if (MineralManager.Instance != null)
        {
            MineralManager.Instance.onMineralCountChanged.AddListener(UpdateDisplay);
            UpdateDisplay(MineralManager.Instance.TotalMinerals);
        }
    }

    private void OnDisable()
    {
        if (MineralManager.Instance != null)
        {
            MineralManager.Instance.onMineralCountChanged.RemoveListener(UpdateDisplay);
        }
    }

    private void UpdateDisplay(int totalMinerals)
    {
        if (mineralText != null)
        {
            mineralText.text = $"Your Minerals: {totalMinerals}";
        }
    }

    public void SetUIColor(Color newColor)
    {
        if (mineralText != null)
        {
            mineralText.color = newColor;
        }
    }
}