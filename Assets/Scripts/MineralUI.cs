using UnityEngine;

public class MineralUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshProUGUI mineralCountText;

    private void Start()
    {
        if (MineralManager.Instance != null)
        {
            MineralManager.Instance.onMineralCountChanged.AddListener(UpdateMineralCount);
            UpdateMineralCount(MineralManager.Instance.TotalMinerals);
        }
    }

    private void OnDisable()
    {
        if (MineralManager.Instance != null)
        {
            MineralManager.Instance.onMineralCountChanged.RemoveListener(UpdateMineralCount);
        }
    }

    private void UpdateMineralCount(int newCount)
    {
        if (mineralCountText != null)
        {
            mineralCountText.text = newCount.ToString();
        }
    }
}