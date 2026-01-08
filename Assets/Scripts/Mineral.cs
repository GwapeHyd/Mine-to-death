using UnityEngine;
using System.Collections;
using UnityEditor.EditorTools;
using Unity.VisualScripting;
using System.ComponentModel;

[RequireComponent(typeof(Collider2D))]
public class Mineral : MonoBehaviour
{
    
    [Header("Mineral Settings")]
    [SerializeField] private int mineralValue = 1;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    [Header("Spawn / Launch")]
    [Tooltip("Force applied to the mineral when spawned")]
    [SerializeField] private float launchForce = 3f;
    [Tooltip("Angle spread for the launch force")]
    [SerializeField] private float launchSpreadAngle = 45f;
    [Tooltip("Delay before the mineral can be collected")]
    [SerializeField] private float collectDelay = 0.2f;

    [Header("Scale / Pop")]
    [Tooltip("Initial scale multiplier for pop effect")]
    [SerializeField] private float popScaleMultiplier = 1.18f;
    [Tooltip("Duration of the pop effect")]
    [SerializeField] private float popDuration = 0.1f;
    [Tooltip("Delay before attraction to player starts")]
    [Header("Attraction / Magnetism")]
    [SerializeField] private float attractDelay = 0.5f;
    [Tooltip("Distance within which the mineral is attracted to the player")]
    [SerializeField] private float attractRange = 5f;
    [Tooltip("Speed at which the mineral is attracted to the player")]
    [SerializeField] private float attractSpeed = 10f;
    [Tooltip("Distance to consider the mineral collected")]
    [SerializeField] private float collectDistance = 0.25f;

    [Header("Behavior")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Should the mineral automatically be destroyed after some time?")]
    [SerializeField] private bool autoDestroy = true;
    [Tooltip("Time after which the mineral is automatically destroyed if enabled")]
    [SerializeField] private float autoDestroyTime = 15f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isAttracting = false;
    private float currentAttractSpeed;
    private bool collected = false;
    private Vector3 initialScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnSequence());
        if (autoDestroy)
        {
            Destroy(gameObject, autoDestroyTime);
        }
    }
    
    private IEnumerator SpawnSequence()
    {
        yield return StartCoroutine(PopScale());

        if (rb != null)
        {
            float angle = Random.Range(-launchSpreadAngle / 2f, launchSpreadAngle / 2f);
            Vector2 forceDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
            rb.AddForce(forceDirection * launchForce, ForceMode2D.Impulse);
        }
        else 
        {
            Debug.LogWarning("Rigidbody2D component missing on Mineral.");
        }

        yield return new WaitForSeconds(collectDelay);
        StartCoroutine(AttractWatcher());
    }

    private IEnumerator PopScale()
    {
        Vector3 targetScale = initialScale * popScaleMultiplier;
        float timer = 0f;

        while (timer < popDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, Mathf.SmoothStep(0f, 1f, timer / popDuration));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        float backDur = popDuration * 0.8f;

        while (timer < backDur)
        {
            transform.localScale = Vector3.Lerp(targetScale, initialScale, Mathf.SmoothStep(0f, 1f, timer / backDur));
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialScale;
    }

    private IEnumerator AttractWatcher()
    {
        float timer = 0f;
        while (!collected)
        {
            timer += Time.deltaTime;
            if (player == null)
            {
                var pgo = GameObject.FindGameObjectWithTag(playerTag);
                if (pgo != null)
                {
                    player = pgo.transform;
                }
            }

            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attractRange)
                {
                    StartAttraction();
                    yield break;
                }
            }
            else
            {
                if (timer >= attractDelay)
                {
                    StartAttraction();
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void StartAttraction()
    {
        if (isAttracting || collected) return;
        isAttracting = true;
        currentAttractSpeed = attractSpeed;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        StartCoroutine(AttractToPlayer());  
    }

    private IEnumerator AttractToPlayer()
    {
        while (!collected)
        {
            if (player != null)
            {
                var pgo = GameObject.FindGameObjectWithTag(playerTag);
                if (pgo != null)
                {
                    player = pgo.transform;
                }
                else
                {
                    transform.position = Vector3.up * (currentAttractSpeed * 0.15f) * Time.deltaTime;
                    yield return null;
                    continue;
                }
            }

            currentAttractSpeed += attractSpeed * Time.deltaTime;

            Vector3 dir = (player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= collectDistance)
            {
                CollectMineral();
                yield break;
            }

            Vector3 move = dir.normalized * currentAttractSpeed * Time.deltaTime;
            if(move.magnitude > distance) move = dir;
            transform.position += move;

            yield return null;
        }
    }

    private void ForceDestroy()
    {
        if (!collected)
            Destroy(gameObject);
    }

    public int GetMineralValue()
    {
        return mineralValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)  return;
        if (collision.CompareTag("Player"))
        {
            CollectMineral();
        }
    }

    private void CollectMineral()
    {
        if (collected == true) return;
        collected = true;

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        if (MineralManager.Instance != null)
        {
            MineralManager.Instance.AddMinerals(mineralValue);
        }

        StartCoroutine(CollectAndDestroy());
    }

    private IEnumerator CollectAndDestroy()
    {
        float timer = 0f;
        float duration = 0.2f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 1.25f;


        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0f, 1f, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
