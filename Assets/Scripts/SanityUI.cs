using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SanityUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image fill;

    

    private void Start()
    {
        if (SanityManager.Instance == null)
        {
            Debug.LogError("SanityManager instance not found in SanityUI Start.");
            return;
        }

        SanityManager.Instance.onSanityChanged.AddListener(UpdateSanityUI);
        UpdateSanityUI(SanityManager.Instance.CurrentSanity, SanityManager.Instance.MaxSanity);
    }

    private void OnDestroy()
    {
        if (SanityManager.Instance != null)
        {
            SanityManager.Instance.onSanityChanged.RemoveListener(UpdateSanityUI);
        }
    }

    private void UpdateSanityUI(int currentSanity, int maxSanity)
    {
        if (fill == null)
        {
            Debug.LogError("Fill Image reference is missing.");
            return;
        }

        if (maxSanity <= 0)
        {
            Debug.LogWarning("Max sanity is zero, cannot update UI.");
            fill.fillAmount = 0;
            return;
        }

        fill.fillAmount = (float)currentSanity / maxSanity;
    }

    public void SetUIColor(Color newColor, Color newColor2)
    {
        if (fill != null)
        {
            fill.color = newColor;
        }
    }
}
