using UnityEngine;
/// <summary>
///  Handles fall damage for the player.
///  if the player fall speed exceeds a certain threshold, apply damage.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFallDamage : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [Tooltip("Minimum fall speed to start taking damage.")]
    [SerializeField] private float fallSpeedThreshold = 10f;
    [Tooltip("Damage multiplier applied to the fall speed exceeding the threshold.")]
    [SerializeField] private float damageMultiplier = 5;
    [Tooltip("Minimum damage that can be applied from a fall.")]
    [SerializeField] private float minDamage = 1f;
    [Tooltip("Interval between 2 fall damage checks.")]
    [SerializeField] private float damageCheckInterval = 0.5f;

    Rigidbody2D rb;
    private Vector2 previousVelocity;
    private float lastImpactTime = -2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        previousVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float verticalSpeed = -previousVelocity.y;
        if (verticalSpeed <= 0f) return;

        if (verticalSpeed < fallSpeedThreshold) return;

        if (Time.time - lastImpactTime < damageCheckInterval)
        {
            return;
        }

        lastImpactTime = Time.time;

        int damage = (int)Mathf.Max(minDamage, Mathf.FloorToInt((verticalSpeed - fallSpeedThreshold) * damageMultiplier));

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.up) < 0.4f)
                continue;
            
            var block = contact.collider.GetComponent<AutoTileBlock>();
            if (block == null)
            {
                Collider2D[]hits = Physics2D.OverlapPointAll(contact.point, LayerMask.GetMask("Blocks"));
                foreach (var hit in hits)
                {
                    block = hit.GetComponent<AutoTileBlock>();
                    if (block != null)
                        break;
            }
        }

        if (block != null)
            {
                block.TakeDamage(damage);
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
