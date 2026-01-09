using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxFallSpeed = 15f;
    
    [Header("Physics")]
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fastFallMultiplier = 2f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Mining")]
    [SerializeField] private int damagePerHit = 50;
    public int DamagePerHit => damagePerHit;   
    [SerializeField] private KeyCode attackKey = KeyCode.E;

    [Header("Head Projectile")]
    [SerializeField] private HeadProjectile headProjectilePrefab;
    [SerializeField] private Vector3 headProjectileSpawnOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private KeyCode throwKey = KeyCode.R;
    [SerializeField] private float throwCooldown = 1f;

    [Header("Double jump")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private int jumpCount = 0;
    [SerializeField] private bool enableDoubleJump = false;
    [SerializeField] private bool requireReleaseBetweenJumps = true;
    [SerializeField] private float minTimeBetweenJumps = 0.1f;
    private float lastJumpTime = -Mathf.Infinity;


    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject hitEffectPrefab;

    private Animator animator; 
    private Rigidbody2D rb;
    private float moveInput;
    private float lastThrowTime = 0f;
    private bool hasActiveProjetile = false;
    private bool isGrounded;
    public bool IsGrounded() => isGrounded;
    private bool isAttacking;
    private bool canThrowHead = false;
    private int jumpsRemaining = 0;
    private float lastJumpPressedTime;
    private float lastGroundedTime; 
    private bool jumpButtonHeldLastFrame = false;
    private bool jumpReleasedSinceLastJump = true;
    public bool CanDoubleJump() => enableDoubleJump;
    private int EffectiveMaxJumps() => enableDoubleJump ? maxJumps : 1;
    

    private List<AutoTileBlock> blocksInRange = new List<AutoTileBlock>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        animator = GetComponent<Animator>();
        jumpsRemaining = maxJumps;
    }

    private void Update()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool jumpHeld = Input.GetKey(KeyCode.Space); 

        if (jumpPressed) lastJumpPressedTime = Time.time;

        bool nowGrounded = Physics2D.OverlapCircle(groundCheck. position, groundCheckRadius, groundLayer);
        if (nowGrounded)
        {
            lastGroundedTime = Time.time;
        }
        if (nowGrounded && !isGrounded)
        {
            jumpsRemaining = EffectiveMaxJumps();
            jumpReleasedSinceLastJump = true;
        }
        isGrounded = nowGrounded;

        if (!jumpHeld) jumpReleasedSinceLastJump = true;

        TryConsumeJump();

        if (!isAttacking)
        {
            moveInput = Input.GetAxis("Horizontal");
        }
        else
        {
            moveInput = 0;
        }
        
        if (moveInput != 0 && !isAttacking)
        {
            animator.SetFloat("MoveX", Mathf.Abs(moveInput));
            if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb. gravityScale = gravityScale * fastFallMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }


        if (Input.GetKey(attackKey) && !isAttacking && isGrounded)
        {
            Attack();
        }

        animator.SetBool("isJumping", !isGrounded);

        if (!hasActiveProjetile && canThrowHead)
        {
            HandleHeadThrow();
        }
    }

    private void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        if (rb. linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
        animator.SetFloat("MoveX", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("MoveY", rb.linearVelocity.y);
    }
    
    public void SetCanDoubleJump(bool enabled)
    {
        enableDoubleJump = enabled;
        jumpsRemaining = Mathf.Min(jumpsRemaining, EffectiveMaxJumps());
    }
    private void TryConsumeJump()
    {
        if (Time.time - lastJumpPressedTime > jumpBufferTime)
            return;
        if (Time.time - lastJumpTime < minTimeBetweenJumps)
            return;
        if (Time.time - lastGroundedTime <= coyoteTime && jumpsRemaining > 0)
        {
            DoJump();
            lastJumpPressedTime = -999f; 
        }

        if (!isGrounded && jumpsRemaining > 0 && CanDoubleJump())
        {
            if (requireReleaseBetweenJumps && !jumpReleasedSinceLastJump)
            {
                return; 
            }

            DoJump();
            lastJumpPressedTime = -999f;
        }
    }

    private void DoJump()
    {
        Vector2 v = rb.linearVelocity;
        v.y =0f;
        rb.linearVelocity = v;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpsRemaining = Mathf.Max(0, jumpsRemaining - 1);
        jumpReleasedSinceLastJump = false;
        lastJumpTime = Time.time;

        animator.SetBool("isJumping", true);
    }

    private void HandleHeadThrow()
    {
        if (Input.GetKeyDown(throwKey) && Time.time - lastThrowTime >= throwCooldown)
        {
            ThrowHead();
        }
    }  

    private void ThrowHead()
    {
        if (headProjectilePrefab == null)
        {
            Debug.LogWarning("HeadProjectile prefab is not assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position + headProjectileSpawnOffset;
        GameObject projectileObj = Instantiate(headProjectilePrefab.gameObject, spawnPosition, Quaternion.identity);
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(3); // 3hp perdu par lancer
        }

        HeadProjectile headProjectile = projectileObj.GetComponent<HeadProjectile>();
        if (headProjectile != null)
        {
            Vector2 throwDirection = GetThrowDirection();
            headProjectile.SetDamage(damagePerHit);
            headProjectile.Initialize(transform, throwDirection, headProjectileSpawnOffset);
            headProjectile.Launch(throwDirection);
            hasActiveProjetile = true;
            lastThrowTime = Time.time;

            StartCoroutine(WaitForProjectileDestruction(headProjectile));
        }
        else
        {
            Debug.LogWarning("The instantiated object does not have a HeadProjectile component.");
            Destroy(projectileObj);
        }
    }

    private Vector2 GetThrowDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        return direction;
    }
    
    private IEnumerator WaitForProjectileDestruction(HeadProjectile projectile)
    {
        while (projectile != null)
        {
            yield return null;
        }
        hasActiveProjetile = false;
    }

    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void Attack()
    {
        if (blocksInRange.Count > 0)
        {
            isAttacking = true;
            animator.SetTrigger("Hit");    
        }
        
    }

    public void DealDamage()
    {
        if (blocksInRange.Count > 0)
        {
            List<AutoTileBlock> blocksToHit = new List<AutoTileBlock>(blocksInRange);
            foreach (AutoTileBlock block in blocksToHit)
            {
                block.TakeDamage(damagePerHit);
            }

            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            PlayerStateManager playerStateManager = GetComponent<PlayerStateManager>();
            if (playerHealth != null && playerStateManager != null && playerStateManager.ShouldTakeMiningDamage())
            {
                playerHealth.TakeDamageFromMining();
            }

            if (hitSound != null)
            {
                AudioManager.Instance.PlaySound(hitSound, 0.15f);
            }

            if (hitEffectPrefab != null)
            {
                foreach (AutoTileBlock block in blocksToHit)
                {
                    Instantiate(hitEffectPrefab, block.transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void AttackFinished()
    {
        isAttacking = false;
        Debug.Log("Attack finished.");
    }

    public void AddBlockInRange(AutoTileBlock block)
    {
        if (!blocksInRange.Contains(block))
        {
            blocksInRange.Add(block);
        }
    }

    public void ClearCurrentBlock(AutoTileBlock block)
    {
        if (blocksInRange.Contains(block))
        {
            blocksInRange.Remove(block);
        }
    }
    public void EnableHeadThrowing()
    {
        canThrowHead = true;
    }

    public void EnableDoubleJump()
    {
        enableDoubleJump = true;
        jumpsRemaining = Mathf.Min(jumpsRemaining, EffectiveMaxJumps());
    }

    public float GetMoveInput()
    {
        return moveInput;
    }

    public void SetIsAttacking(bool attacking)
    {
        isAttacking = attacking;
    }
}