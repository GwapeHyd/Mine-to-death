using UnityEngine;

public class CoinBlock : MonoBehaviour
{
    [Header("Coin Drop Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsToDrop = 1;
    [SerializeField] private int depthMultiplierInterval = 50;

    [Header("Depth Calculation")]
    [SerializeField] private float surfaceLevel = 200f;

    [Header("Drop Physics")]
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float spreadAngle = 30f;

    [Header("Special Block")]
    [SerializeField] private bool isSpecialBlock = false;

    private AutoTileBlock autoTileBlock;

    private void Start()
    {
        autoTileBlock = GetComponent<AutoTileBlock>();

        if (autoTileBlock != null)
        {
            autoTileBlock.onBlockDestroyed.AddListener(OnBlockDestroyed);
        }
    }

    private void OnBlockDestroyed()
    {
        DropCoins();
    }

    private void DropCoins()
    {
        if (coinPrefab == null) return;

        float currentY = transform.position.y;
        int depth = Mathf.Max(0, Mathf.FloorToInt(surfaceLevel - currentY));
        
        int depthMultiplier = 1 + (depth / depthMultiplierInterval);

        int coinsToSpawn = coinsToDrop * depthMultiplier;

        for (int i = 0; i < coinsToSpawn; i++)
        {
            SpawnCoin(i, coinsToSpawn);
        }
    }

    private void SpawnCoin(int index, int totalCoins)
    {
        Vector3 spawnPosition = transform.position;
        GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float angleStep = Random.Range(-spreadAngle, spreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, angleStep) * Vector2.up;

            float randomForce = Random.Range(dropForce * 0.8f, dropForce * 1.2f);
            rb.AddForce(direction * randomForce, ForceMode2D.Impulse);

            rb.angularVelocity = Random.Range(-180f, 180f);
        }
    }

    private void OnDestroy()
    {
        if (autoTileBlock != null)
        {
            autoTileBlock.onBlockDestroyed.RemoveListener(OnBlockDestroyed);
        }
    }
}
