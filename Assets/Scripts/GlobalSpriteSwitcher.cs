using UnityEngine;

public class GlobalSpriteSwitcher : MonoBehaviour
{
    
    [SerializeField] private SwappableSprite[] swappableSprites;

    [ContextMenu("Swap All Sprites")]
    public void SwapAllSprites()
    {
        foreach (var swappable in swappableSprites)
        {
            swappable.SwapSpriteSet();
        }
    }
}
