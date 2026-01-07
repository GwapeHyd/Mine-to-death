using System.Collections;
using UnityEngine;

public class HeadProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private float rotationSpeed = 360f; 
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float returnSpeed = 10f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float maxLifetime = 5f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip launchSound;
    [SerializeField] private AudioClip bounceSound;

    private Rigidbody2D rb;
    private Collider2D projectileCollider;
    private Transform playerTransform;
    private Vector3 spawnOffSet;

    private int currentBounces = 0;
    private bool isLaunched = false;
    private bool isReturning = false;
    private Vector2 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        if (projectileCollider == null)
        {
            projectileCollider = gameObject.AddComponent<CircleCollider2D>();
        }  

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        projectileCollider.isTrigger = false;

        playerTransform = transform.parent;
    }

    private void Update()
    {
        if (isLaunched && !isReturning)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            lastVelocity = rb.linearVelocity;
        }

        if (isLaunched)
        {
            maxLifetime -= Time.deltaTime;
            if (maxLifetime <= 0f)
            {
                StartCoroutine(ReturnToPlayer());
            }
        }
    }

    public void Initialize(Transform player,Vector2 direction, Vector3 offset)
    {
        playerTransform = player;
        spawnOffSet = offset;
        Launch(direction);
    }

    public void Launch(Vector2 direction)
    {
        if (isLaunched) return;

        isLaunched = true;
        currentBounces = 0;
        isReturning = false;

        transform.SetParent(null);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        rb.linearVelocity = direction.normalized * launchForce;

        if (launchSound != null)
        {
            AudioManager.Instance.PlaySound(launchSound, 0.2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched || isReturning) return;

        AutoTileBlock block = collision.gameObject.GetComponent<AutoTileBlock>();

        if (block != null)
        {
            block.TakeDamage(damage);
            currentBounces++;

            if (bounceSound != null)
            {
                AudioManager.Instance.PlaySound(bounceSound, 0.2f);
            }

            if (currentBounces >= maxBounces)
            {
                StartCoroutine(ReturnToPlayer());
            }
            else
            {
                Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                rb.linearVelocity = reflectDir * launchForce;
            }
        }
        else
        {
            Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir * launchForce;
        }
    }

    private IEnumerator ReturnToPlayer()
    {
        isReturning = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.Distance(transform.position, playerTransform.position) > 0.1f)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * returnSpeed * Time.deltaTime;
            yield return null;
        }


        ResetProjectile();
        Destroy(gameObject);
    }

    private void ResetProjectile()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        projectileCollider.enabled = true;

        isLaunched = false;
        isReturning = false;
        currentBounces = 0;
    }

    public bool IsAvailable()
    {
        return !isLaunched && !isReturning;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetMaxBounces(int newMaxBounces)
    {
        maxBounces = newMaxBounces;
    }   
}
