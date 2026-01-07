using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CoinAppearance : MonoBehaviour
{
    [SerializeField] private string spriteKey = "coin"; 

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplySprite();

        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged += OnThemeChanged;
    }

    private void OnDestroy()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(int idx)
    {
        ApplySprite();
    }

    private void ApplySprite()
    {
        if (sr == null) return;
        Sprite s = ThemeManager.Instance != null ? ThemeManager.Instance.GetSprite(spriteKey) : null;
        if (s != null)
            sr.sprite = s;
    }
}