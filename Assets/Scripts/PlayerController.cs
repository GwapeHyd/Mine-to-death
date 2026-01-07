using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float jumpForce = 8f;
    
    [Header("Physics")]
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fastFallMultiplier = 2f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Mining")]
    [SerializeField] private int damagePerHit = 50;
    [SerializeField] private KeyCode attackKey = KeyCode.E;

    private Animator animator; 
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    public bool IsGrounded() => isGrounded;
    private bool isAttacking;

    private List<AutoTileBlock> blocksInRange = new List<AutoTileBlock>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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
        
        isGrounded = Physics2D.OverlapCircle(groundCheck. position, groundCheckRadius, groundLayer);
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            rb. gravityScale = gravityScale * fastFallMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            PaletteSwitcher paletteSwitcher = FindFirstObjectByType<PaletteSwitcher>();
            
            if (paletteSwitcher != null)
            {
                paletteSwitcher.SwitchPalette();
            }
        }

        if (Input.GetKeyDown(attackKey) && !isAttacking && isGrounded)
        {
            Attack();
        }

        animator.SetBool("isJumping", !isGrounded);
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
    
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetBool("isJumping", true);

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

    public float GetMoveInput()
    {
        return moveInput;
    }
}