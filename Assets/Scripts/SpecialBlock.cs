using UnityEngine;
using UnityEngine.Events;

public class SpecialBlock : AutoTileBlock
{

    [Header("Special Sound")]
    [SerializeField] private AudioClip hintSound;


    private void Awake()
    {
        isSpecialBlock = true;
    }
    
    private void Start()
    {
        currentHealth = maxHealth;
        
        if (actionFeedback != null)
            actionFeedback.SetActive(false);

        if (blockSpriteRenderer == null)
            blockSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void DestroyBlock()
    {
        base.DestroyBlock();
        if (hintSound != null)
        {
            AudioManager.Instance.PlaySound(hintSound, 0.1f);
        }
    }
}