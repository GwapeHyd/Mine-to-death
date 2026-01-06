using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager :  MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Bonus,
        Invincible,
        PoweredUp
    }

    [Header("State Settings")]
    [SerializeField] private PlayerState currentState = PlayerState.Normal;
    [SerializeField] private float bonusStateDuration = 10f;
    private float stateTimer = 0f;

    [Header("Bonus State Effects")]
    [SerializeField] private bool noMiningDamage = true; 
    [SerializeField] private float miningSpeedMultiplier = 2f; 
    [SerializeField] private int bonusDamagePerHit = 100; 

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem bonusParticles;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color bonusColor = Color.cyan;
    private Color originalColor;

    [Header("Events")]
    [SerializeField] private UnityEvent onBonusStateActivated;
    [SerializeField] private UnityEvent onBonusStateEnded;

    private PlayerController playerController;
    private Animator animator;
    private float originalAnimationSpeed = 1f;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();

        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();

        if (playerSprite != null)
            originalColor = playerSprite.color;
    }

    private void Update()
    {
        if (currentState == PlayerState.Bonus)
        {
            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0f)
            {
                EndBonusState();
            }
        }
    }

    public void ActivateBonusState()
    {
        currentState = PlayerState.Bonus;
        stateTimer = bonusStateDuration;

        Debug.Log($"Bonus State Activated for {bonusStateDuration} seconds!");

        if (bonusParticles != null)
        {
            bonusParticles.Play();
        }

        if (playerSprite != null)
        {
            StartCoroutine(PulseColor());
        }

        if (playerController != null && bonusDamagePerHit > 0)
        {
            var field = playerController.GetType().GetField("damagePerHit", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(playerController, bonusDamagePerHit);
            }
        }

        if (animator != null)
        {
            animator.speed = miningSpeedMultiplier;
        }

        onBonusStateActivated?. Invoke();
    }

    private void EndBonusState()
    {
        currentState = PlayerState.Normal;

        Debug.Log("Bonus State Ended");

        if (bonusParticles != null)
        {
            bonusParticles.Stop();
        }

        StopAllCoroutines();
        if (playerSprite != null)
        {
            playerSprite. color = originalColor;
        }

        if (playerController != null)
        {
            var field = playerController.GetType().GetField("damagePerHit", 
                System.Reflection.BindingFlags.NonPublic | 
                System. Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(playerController, 50);
            }
        }

        if (animator != null)
        {
            animator.speed = originalAnimationSpeed;
        }

        PaletteSwitcher paletteSwitcher = FindFirstObjectByType<PaletteSwitcher>();
        if (paletteSwitcher != null)
        {
            paletteSwitcher.SwitchPalette();
        }

        onBonusStateEnded?.Invoke();
    }

    private System.Collections.IEnumerator PulseColor()
    {
        while (currentState == PlayerState.Bonus)
        {
            playerSprite.color = Color.Lerp(originalColor, bonusColor, Mathf.PingPong(Time.time * 2f, 1f));
            yield return null;
        }

        playerSprite.color = originalColor;
    }

    public bool ShouldTakeMiningDamage()
    {
        if (currentState == PlayerState.Bonus && noMiningDamage)
        {
            return false; 
        }
        return true;
    }

    public PlayerState CurrentState => currentState;
    public bool IsInBonusState => currentState == PlayerState.Bonus;
    public float RemainingBonusTime => stateTimer;
}