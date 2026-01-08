using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFallDamage : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [Tooltip("Minimum fall height (units in Unity units) to start taking damage.")]
    [SerializeField] private float fallHeightThreshold = 3f;
    [Tooltip("Damage multiplier applied to the fall height exceeding the threshold.")]
    [SerializeField] private float damageMultiplier = 5f;
    [Tooltip("Minimum damage that can be applied from a fall.")]
    [SerializeField] private float minDamage = 1f;
    [Tooltip("Interval between 2 fall damage checks.")]
    [SerializeField] private float damageCheckInterval = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;

    // Optional: layer name used when falling onto blocks (used in OverlapPointAll fallback)
    [SerializeField] private string blocksLayerName = "Blocks";

    Rigidbody2D rb;

    // Tracks colliders considered as "ground" (contact normals pointing sufficiently upward)
    private readonly System.Collections.Generic.HashSet<Collider2D> groundColliders = new System.Collections.Generic.HashSet<Collider2D>();
    private bool grounded = false;

    // Y position where the player last left the ground (start height of the fall)
    private float leaveGroundY;

    private float lastImpactTime = -2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        leaveGroundY = transform.position.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check each contact for a ground-like normal
        bool hasGroundContact = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.4f)
            {
                hasGroundContact = true;
                break;
            }
        }

        if (hasGroundContact)
        {
            // Add to ground colliders set
            if (!groundColliders.Contains(collision.collider))
                groundColliders.Add(collision.collider);

            // If we were previously airborne, this is a landing
            if (!grounded)
            {
                // Prevent repeated processing in a short time
                if (Time.time - lastImpactTime >= damageCheckInterval)
                {
                    // We'll consider the first contact point that is reasonably "ground"
                    ContactPoint2D? landingContact = null;
                    foreach (ContactPoint2D contact in collision.contacts)
                    {
                        if (Vector2.Dot(contact.normal, Vector2.up) > 0.4f)
                        {
                            landingContact = contact;
                            break;
                        }
                    }

                    if (landingContact.HasValue)
                    {
                        float landingY = landingContact.Value.point.y;
                        float fallHeight = leaveGroundY - landingY;

                        if (fallHeight > fallHeightThreshold)
                        {
                            lastImpactTime = Time.time;

                            int damage = (int)Mathf.Max(minDamage, Mathf.FloorToInt(((fallHeight+1) - fallHeightThreshold) * damageMultiplier));

                            // Try to find AutoTileBlock on the contacted collider first
                            AutoTileBlock block = landingContact.Value.collider.GetComponent<AutoTileBlock>();

                            // If not found try an OverlapPointAll on the Blocks layer (fallback)
                            if (block == null && !string.IsNullOrEmpty(blocksLayerName))
                            {
                                int layer = LayerMask.NameToLayer(blocksLayerName);
                                if (layer >= 0)
                                {
                                    Collider2D[] hits = Physics2D.OverlapPointAll(landingContact.Value.point, 1 << layer);
                                    foreach (var hit in hits)
                                    {
                                        block = hit.GetComponent<AutoTileBlock>();
                                        if (block != null)
                                            break;
                                    }
                                }
                                else
                                {
                                    // If layer name not found, fallback to OverlapPointAll without filter
                                    Collider2D[] hits = Physics2D.OverlapPointAll(landingContact.Value.point);
                                    foreach (var hit in hits)
                                    {
                                        block = hit.GetComponent<AutoTileBlock>();
                                        if (block != null)
                                            break;
                                    }
                                }
                            }

                            if (block != null)
                            {
                                block.TakeDamage(damage);

                                if (hitEffectPrefab != null)
                                {
                                    Instantiate(hitEffectPrefab, landingContact.Value.point, Quaternion.identity);
                                }
                                if (hitSound != null && AudioManager.Instance != null)
                                {
                                    // volume kept low like original (0.1f)
                                    AudioManager.Instance.PlaySound(hitSound, 0.1f);
                                }
                            }
                        }
                    }
                }
            }

            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If we were counting this collider as ground contact, remove it.
        if (groundColliders.Remove(collision.collider))
        {
            // If no more ground colliders, player becomes airborne
            if (groundColliders.Count == 0)
            {
                grounded = false;
                // record the Y position where we left the ground (start of potential fall)
                leaveGroundY = transform.position.y;
            }
        }
    }

    // Optional: keep leaveGroundY updated if player jumps (e.g., using velocity), but OnCollisionExit2D handles typical cases.
    // You can also track leaving ground via other triggers or logic (e.g., jumping code) and set leaveGroundY there.
}