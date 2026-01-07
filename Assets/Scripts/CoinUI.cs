using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image coinIcon;

    [Header("Animations")]
    [SerializeField] private float scaleBounceDuration = 0.2f;
    [SerializeField] private float scaleBounceAmount = 1.2f;

    private void Start()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.AddListener(UpdateCoinDisplay);
            CoinManager.Instance.onCoinsAdded.AddListener(OnCoinsAdded);

            UpdateCoinDisplay(CoinManager.Instance.TotalCoins);
        }
    }

    private void UpdateCoinDisplay(int totalCoins)
    {
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}";
        }
    }

    private void OnCoinsAdded(int amount)
    {
        if (coinIcon != null)
        {
            StopAllCoroutines();
            StartCoroutine(BounceAnimation());
            Debug.Log($"Added {amount} coins");
        }
    }

    private IEnumerator BounceAnimation()
    {
        Vector3 originalScale = coinIcon.transform.localScale;
        Vector3 targetScale = originalScale * scaleBounceAmount;

        float timer = 0f;

        // Scale up
        while (timer < scaleBounceDuration / 2f)
        {
            coinIcon.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / scaleBounceDuration / 2f);
            timer += Time.deltaTime;
            yield return null;
        }
        coinIcon.transform.localScale = targetScale;

        timer = 0f;

        // Scale down
        while (timer < scaleBounceDuration / 2f)
        {
            coinIcon.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / scaleBounceDuration / 2f);
            timer += Time.deltaTime;
            yield return null;
        }
        coinIcon.transform.localScale = originalScale;
    }

    public void SetUIColor(Color newColor)
    {
        if (coinText != null)
        {
            coinText.color = newColor;
        }
        if (coinIcon != null)
        {
            coinIcon.color = newColor;
        }
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.onCoinCountChanged.RemoveListener(UpdateCoinDisplay);
            CoinManager.Instance.onCoinsAdded.RemoveListener(OnCoinsAdded);
        }
    }

}
