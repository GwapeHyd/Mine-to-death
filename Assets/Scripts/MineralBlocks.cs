using UnityEngine;

public class MineralBlocks : MonoBehaviour
{
    [Header("Mineral drop Settings")]
    [SerializeField] private GameObject mineralPrefab;
    [SerializeField] private int mineralsToDrop = 1;
    [SerializeField] private int depthMultiplierInterval = 50;

    [Header("Depth Calculation")]
    [SerializeField] private float surfaceLevel = 200f;

    [Header("Drop Physics")]
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float spreadAngle = 30f;

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
        DropMinerals();
    }

    private void DropMinerals()
    {
        if (mineralPrefab == null) return;

        float currentY = transform.position.y;
        int depth = Mathf.Max(0, Mathf.FloorToInt(surfaceLevel - currentY));
        int depthMultiplier = 1 + (depth / depthMultiplierInterval);
        int mineralsToDrop = this.mineralsToDrop * depthMultiplier;

        for (int i = 0; i < mineralsToDrop; i++)
        {
            GameObject mineral = Instantiate(mineralPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = mineral.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float angle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
                Vector2 forceDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
                rb.AddForce(forceDirection * dropForce, ForceMode2D.Impulse);
            }
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