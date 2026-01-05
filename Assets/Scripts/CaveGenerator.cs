using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    [Header("Cave Dimensions")]
    [SerializeField] private int caveWidth = 100;
    [SerializeField] private int caveHeight = 100;
    [SerializeField] private float tileSize = 1f;

    [Header("Generation Settings")]
    [SerializeField] private float fillPercent = 0.45f;
    [SerializeField] private int seed = 0;

    [Header("Block Prefab")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform blocksParent;

    private System.Random random;
    private int[,] caveMap;
    private List<GameObject> spawnedBlocks = new List<GameObject>();

    [ContextMenu("Generate Cave")]
    public void GenerateCave()
    {
        ClearCave();

        if (seed == 0)
            seed = Random.Range(int.MinValue, int.MaxValue);

        random = new System.Random(seed);
        Random.InitState(seed);

        GenerateCaveMap();

        SpawnBlocks();
    }

    private void GenerateCaveMap()
    {
        caveMap = new int[caveWidth, caveHeight];

        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                if (x == 0 || x == caveWidth - 1 || y == 0 || y == caveHeight - 1)
                {
                    caveMap[x, y] = 1; // Mur
                }
                else
                {
                    caveMap[x, y] = (random.NextDouble() < fillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void SpawnBlocks()
    {
        if (blockPrefab == null) return;

        if (blocksParent == null)
        {
            GameObject blocksParentGO = new GameObject("Cave Blocks");
            blocksParent = blocksParentGO.transform;
        }

        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                if (caveMap[x, y] == 1)
                {
                    Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                    GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, blocksParent);
                    block.name = $"Block_{x}_{y}";
                    spawnedBlocks.Add(block);
                }
            }
        }
    }

    [ContextMenu("Clear Cave")]
    public void ClearCave()
    {
        foreach (GameObject block in spawnedBlocks)
        {
            if (block != null)
                DestroyImmediate(block);
        }
        spawnedBlocks.Clear();

        if (blocksParent != null)
        {
            foreach (Transform child in blocksParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(caveWidth * tileSize / 2f, caveHeight * tileSize /2f, 0);
        Vector3 size = new Vector3(caveWidth * tileSize, caveHeight * tileSize, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }




    

}