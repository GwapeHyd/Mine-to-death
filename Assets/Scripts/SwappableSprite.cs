using UnityEngine;
using System.Collections.Generic;

public class SwappableSprite : MonoBehaviour
{
    [System.Serializable]
    public class SpriteMapping
    {
        public string spriteName;
        public Sprite spriteSetA;
        public Sprite spriteSetB;
    }

    [Header("Sprite Mappings")]
    [SerializeField] private SpriteMapping[] spriteMappings;

    [Header("Sprite reference")]
    [SerializeField] private string spriteName;

    private int currentIndex = 0;
    private Dictionary<string, Sprite> currentSpriteSet;

    private void Start()
    {
        
    }

    public void SwapSpriteSet()
    {
        currentIndex = 1 - currentIndex; // Toggle between 0 and 1
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        foreach (var mapping in spriteMappings)
        {
            if (mapping.spriteName == spriteName)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = currentIndex == 0 ? mapping.spriteSetA : mapping.spriteSetB;
                }
                break;
            }
        }
    }
}
