using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int coinValue = 1;

    [Header("Movement Settings")]
    [SerializeField] private float attractSpeed = 8f;
    [SerializeField] private float attractRange = 4f;
    [SerializeField] private bool autoAtract = true;

    [Header("Visual Settings")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float rotationSpeed = 180f;
    private Vector3 startPosition;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    private Transform playerTransform;
    private bool isBeingCollected = false;

    private void Start()
    {
        startPosition = transform.position;
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (isBeingCollected || playerTransform == null) return;

        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        if (autoAtract)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attractRange)
            {
                float distance = Vector2.Distance(transform.position, playerTransform.position);

                if (distance < attractRange)
                {
                    transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && !isBeingCollected)
        {
            CollectCoin(collision.gameObject);
        }
    }

    private void CollectCoin(GameObject player)
    {
        isBeingCollected = true;

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();
        if (coinManager != null)
        {
            coinManager.AddCoins(coinValue);
        }

        Destroy(gameObject);
    }
    
    
    



}
