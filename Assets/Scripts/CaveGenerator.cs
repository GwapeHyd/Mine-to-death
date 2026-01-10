using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CaveGenerator : MonoBehaviour
{
    [Header("Cave Dimensions")]
    [SerializeField] private int caveWidth = 100;
    [SerializeField] private int caveHeight = 100;
    [SerializeField] private float tileSize = 1f;

    [Header("Generation Settings")]
    [SerializeField] private float fillPercent = 0.45f;
    [SerializeField] private int seed = 0;
    
    [Header("Smoothing Settings")]
    [SerializeField] private int smoothingIterations = 5;

    [Header("Blocks Prefab")]
    [SerializeField] private GameObject defaultBlockPrefab;
    [SerializeField] private GameObject bonusBlockPrefab;
    [SerializeField] private GameObject hintBlockFarPrefab;
    [SerializeField] private GameObject hintBlockNearPrefab;
    [SerializeField] private GameObject coinBlockPrefab;
    [SerializeField] private GameObject mineralBlockPrefab;
    [SerializeField] private GameObject mediumMineralBlockPrefab;
    [SerializeField] private GameObject largeMineralBlockPrefab;
    
    [SerializeField] private Transform blocksParent;

    [Header("Bonus Block Settings")]
    [SerializeField] private float bonusZoneMinPercent = .4f;
    [SerializeField] private float bonusZoneMaxPercent = .6f;

    [Header("Hint Blocks Settings")]
    [SerializeField] private int hintBlockFarCount = 10;
    [SerializeField] private int hintBlockNearCount = 3;
    [SerializeField] private float farHintMinDistance = 30f;
    [SerializeField] private float nearHintMinDistance = 15f;

    [Header("Coin Block Settings")]
    [SerializeField] private float coinBlockSpawnChance = .05f;
    [SerializeField] private float surfaceLevel = 200f;
    [SerializeField] private int minDepthForCoinBlocks = 20;

    [Header("Minerals Settings")]
    [SerializeField, Range(0f, 1f)] private float mineralSpawnChance = 0.05f;
    [SerializeField] private int minDepthForMineralBlocks = 5;
    [SerializeField, Range(0f, 1f)] private float mediumMineralSpawnChance = 0.03f;
    [SerializeField] private int minDepthForMediumMineralBlocks = 30;
    [SerializeField, Range(0f, 1f)] private float largeMineralSpawnChance = 0.01f;
    [SerializeField] private int minDepthForLargeMineralBlocks = 60;

    [SerializeField] private int minNeighborsForMineral = 8;

    [SerializeField] private AutoTileSpriteSet mineralSpriteSet;
    [SerializeField] private AutoTileSpriteSet mediumMineralSpriteSet;
    [SerializeField] private AutoTileSpriteSet largeMineralSpriteSet;
    [SerializeField] private AutoTileSpriteSet bonusBlockSpriteSet;

    private System.Random random;
    private int[,] caveMap;
    private List<GameObject> spawnedBlocks = new List<GameObject>();
    private Vector2Int bonusBlockPosition;
    private bool bonusBlockPlaced = false;
    private List<Vector2Int> wallPositions = new List<Vector2Int>();

    [ContextMenu("Generate Cave")]
    public void GenerateCave()
    {
        ClearCave();
        bonusBlockPlaced = false;
        wallPositions.Clear();

        if (seed == 0)
            seed = Random.Range(int.MinValue, int.MaxValue);

        random = new System.Random(seed);
        Random.InitState(seed);

        GenerateCaveMap();
    
        PlaceMineralBlocks();
        PlaceMediumMineralBlocks();
        PlaceLargeMineralBlocks();

        PlaceBonusBlock();
        PlaceHintBlocks();

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
                    caveMap[x, y] = 1; 
                }
                else
                {
                    caveMap[x, y] = (random.NextDouble() < fillPercent) ? 1 : 0;
                }
            }
        }

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        wallPositions.Clear();

        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                if (caveMap[x, y] == 1)
                {
                    wallPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void PlaceBonusBlock()
    {
        int minY = Mathf.RoundToInt(caveHeight * bonusZoneMinPercent);
        int maxY = Mathf.RoundToInt(caveHeight * bonusZoneMaxPercent);

        List<Vector2Int> validPositions = wallPositions.Where(pos => pos.y > minY && pos.y <= maxY).ToList();

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found for bonus block.");
            return;
        }

        int randomIndex = random.Next(validPositions.Count);
        bonusBlockPosition = validPositions[randomIndex];
        bonusBlockPlaced = true;

        caveMap[bonusBlockPosition.x, bonusBlockPosition.y] = 2;
        Debug.Log($"Placed bonus block at {bonusBlockPosition}");
    }

    private void PlaceMineralBlocks()
    {
        if (mineralBlockPrefab == null) return;

        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (Vector2Int wallPos in wallPositions)
        {
            if (IsEdgePosition(wallPos)) continue;

            if (wallPos.y < minDepthForMineralBlocks) continue;

            if (caveMap[wallPos.x, wallPos.y] != 1) continue;

            int neighborWalls = GetSurroundingWallCount(wallPos.x, wallPos.y);
            if (neighborWalls >= minNeighborsForMineral)
            {
                candidates.Add(wallPos);
            }
        }

        if (candidates.Count == 0)
            {
                Debug.LogWarning("No valid positions found for mineral blocks.");
                return;
            }

        int targetCount = Mathf.FloorToInt(candidates.Count * mineralSpawnChance);
        targetCount = Mathf.Clamp(targetCount, 0, candidates.Count);
        if (mineralSpawnChance > 0f && targetCount == 0)
            targetCount = 1; // Ensure at least one if percentage > 0
        
        HashSet<int> chosenIndices = new HashSet<int>();
        int attempts = 0;
        while (chosenIndices.Count < targetCount && attempts < candidates.Count * 3)
        {
            int randomIndex = random.Next(candidates.Count);
            chosenIndices.Add(randomIndex);
            attempts++;
        }

        foreach (int index in chosenIndices)
        {
            Vector2Int pos = candidates[index];
            if (!IsEdgePosition(pos))
                caveMap[pos.x, pos.y] = 6; 
        }

        Debug.Log($"Placed {chosenIndices.Count} mineral blocks.");
    
    }
    private void PlaceMediumMineralBlocks()
    {
        if (mediumMineralBlockPrefab == null) return;

        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (Vector2Int wallPos in wallPositions)
        {
            if (IsEdgePosition(wallPos)) continue;

            if (wallPos.y < minDepthForMediumMineralBlocks) continue;

            if (caveMap[wallPos.x, wallPos.y] != 1) continue;

            int neighborWalls = GetSurroundingWallCount(wallPos.x, wallPos.y);
            if (neighborWalls >= minNeighborsForMineral)
            {
                candidates.Add(wallPos);
            }
        }

        if (candidates.Count == 0)
            {
                Debug.LogWarning("No valid positions found for medium mineral blocks.");
                return;
            }

        int targetCount = Mathf.FloorToInt(candidates.Count * mediumMineralSpawnChance);
        targetCount = Mathf.Clamp(targetCount, 0, candidates.Count);
        if (mediumMineralSpawnChance > 0f && targetCount == 0)
            targetCount = 1; // Ensure at least one if percentage > 0
        
        HashSet<int> chosenIndices = new HashSet<int>();
        int attempts = 0;
        while (chosenIndices.Count < targetCount && attempts < candidates.Count * 3)
        {
            int randomIndex = random.Next(candidates.Count);
            chosenIndices.Add(randomIndex);
            attempts++;
        }

        foreach (int index in chosenIndices)
        {
            Vector2Int pos = candidates[index];
            if (!IsEdgePosition(pos))
                caveMap[pos.x, pos.y] = 7; 
        }

        Debug.Log($"Placed {chosenIndices.Count} medium mineral blocks.");
    
    }
    private void PlaceLargeMineralBlocks()
    {
        if (largeMineralBlockPrefab == null) return;

        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (Vector2Int wallPos in wallPositions)
        {
            if (IsEdgePosition(wallPos)) continue;
            if (wallPos.y < minDepthForLargeMineralBlocks) continue;
            if (caveMap[wallPos.x, wallPos.y] != 1) continue;

            int neighborWalls = GetSurroundingWallCount(wallPos.x, wallPos.y);
            if (neighborWalls >= minNeighborsForMineral)
            {
                candidates.Add(wallPos);
            }
        }

        if (candidates.Count == 0)
            {
                Debug.LogWarning("No valid positions found for large mineral blocks.");
                return;
            }

        int targetCount = Mathf.FloorToInt(candidates.Count * largeMineralSpawnChance);
        targetCount = Mathf.Clamp(targetCount, 0, candidates.Count);
        if (largeMineralSpawnChance > 0f && targetCount == 0)
            targetCount = 1; // Ensure at least one if percentage > 0
        
        HashSet<int> chosenIndices = new HashSet<int>();
        int attempts = 0;
        while (chosenIndices.Count < targetCount && attempts < candidates.Count * 3)
        {
            int randomIndex = random.Next(candidates.Count);
            chosenIndices.Add(randomIndex);
            attempts++;
        }

        foreach (int index in chosenIndices)
        {
            Vector2Int pos = candidates[index];
            if (!IsEdgePosition(pos))
                caveMap[pos.x, pos.y] = 8; 
        }

        Debug.Log($"Placed {chosenIndices.Count} large mineral blocks.");
    
    }
    private void PlaceHintBlocks()
    {
        if (!bonusBlockPlaced)
        {
            Debug.LogWarning("Bonus block not placed. Cannot place hint blocks.");
            return;
        }

        PlaceHintBlocksAtDistance(hintBlockFarCount, farHintMinDistance, float.MaxValue, 3);

        PlaceHintBlocksAtDistance(hintBlockNearCount, 0, nearHintMinDistance, 4);
    }

    private void PlaceHintBlocksAtDistance(int count, float minDistance, float maxDistance, int blockTypeValue)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        foreach (Vector2Int wallPos in wallPositions)
        {
            if (wallPos == bonusBlockPosition) continue;

            if (IsEdgePosition(wallPos)) continue;

            if (caveMap[wallPos.x, wallPos.y] != 1) continue;

            float distance = Vector2Int.Distance(wallPos, bonusBlockPosition);
            if (distance >= minDistance && distance < maxDistance)
            {
                validPositions.Add(wallPos);
            }
        }

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No valid positions found for hint blocks.");
            return;
        }

        List<Vector2Int> chosenPositions = SelectDirectionalHints(validPositions, count);

        foreach (Vector2Int pos in chosenPositions)
        {
            caveMap[pos.x, pos.y] = blockTypeValue;
        }

        Debug.Log($"Placed {chosenPositions.Count} hint blocks of type {blockTypeValue}.");
    }

    private List<Vector2Int> SelectDirectionalHints(List<Vector2Int> candidates, int count)
    {
        List<Vector2Int> selected = new List<Vector2Int>();

        if (candidates.Count == 0) return selected;

        Vector2 bonusPos = new Vector2(bonusBlockPosition.x, bonusBlockPosition.y);

        Dictionary<int, List<Vector2Int>> sectors = new Dictionary<int, List<Vector2Int>>();
        
        for(int i = 0; i < 8; i++)
        {
            sectors[i] = new List<Vector2Int>();
        }

        foreach (Vector2Int candidate in candidates)
        {
            Vector2 direction = new Vector2(candidate.x - bonusPos.x, candidate.y - bonusPos.y).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int sectorIndex = Mathf.FloorToInt(angle / 45f) % 8;
            sectors[sectorIndex].Add(candidate);
        }

        int attemptsPerBlock = 100;
        while (selected.Count < count && attemptsPerBlock > 0)
        {
            List<int> availableSectors = sectors.Where(s => s.Value.Count > 0).Select(s => s.Key).ToList();

            if (availableSectors.Count == 0) break;

            int randomSector = availableSectors[random.Next(availableSectors.Count)];
            List<Vector2Int> sectorCandidates = sectors[randomSector];

            if (sectorCandidates.Count > 0)
            {
                int randomIndex = random.Next(sectorCandidates.Count);
                Vector2Int chosen = sectorCandidates[randomIndex];

                if (!selected.Any(s => Vector2.Distance(s, chosen) < 5f))
                {
                    selected.Add(chosen);
                    sectorCandidates.RemoveAt(randomIndex);
                }
            }

            attemptsPerBlock--;
        }

        return selected;
    }

    private void SpawnBlocks()
    {
        if (defaultBlockPrefab == null) 
        {
            Debug.LogWarning("Default block prefab is not assigned. Cannot spawn blocks.");
            return;
        }

        Debug.Log("Spawning cave blocks...");
        if (blocksParent == null)
        {
            GameObject blocksParentGO = new GameObject("Cave Blocks");
            blocksParent = blocksParentGO.transform;
        }

        System.Random coinRandom = new System.Random(seed + 1000);

        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                int blockType = caveMap[x, y];
                if (blockType > 0)
                {
                    Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                    GameObject prefabToSpawn = GetPrefabForBlockType(blockType);
                    
                    float blockDepth = surfaceLevel - position.y;

                    if (blockType == 1 && !IsEdgePosition(new Vector2Int(x,y)) && coinBlockPrefab != null && blockDepth >= minDepthForCoinBlocks)
                    {
                        if (coinRandom.NextDouble() < coinBlockSpawnChance)
                        {
                            prefabToSpawn = coinBlockPrefab;
                            blockType = 5;
                        }
                    }

                    GameObject block = Instantiate(prefabToSpawn, position, Quaternion.identity, blocksParent);
                    block.name = $"{GetBlockTypeName(blockType)}_{x}_{y}";

                    spawnedBlocks.Add(block);

                    AutoTileBlock atb = block.GetComponent<AutoTileBlock>();
                    if (atb != null)
                    {
                        atb.RegisterGridPosition(new Vector2Int(x, y));
                        if (blockType == 2 && bonusBlockSpriteSet != null)
                        {
                            atb.SetSpriteSet(bonusBlockSpriteSet);
                        }

                        if (blockType == 8 && largeMineralSpriteSet != null)
                        {
                            atb.SetSpriteSet(largeMineralSpriteSet);
                        }
                        if (blockType == 7 && mediumMineralSpriteSet != null)
                        {
                            atb.SetSpriteSet(mediumMineralSpriteSet);
                        }

                        if (blockType == 6 && mineralSpriteSet != null)
                        {
                            atb.SetSpriteSet(mineralSpriteSet);
                        }

                        atb.RegisterGridPosition(new Vector2Int(x, y));

                        if (blockType == 3 || blockType == 4 || blockType == 5) 
                        {
                        if (atb != null)
                        {
                            var field = atb.GetType().GetField("isSpecialBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                            if (field != null)
                            {
                                field.SetValue(atb, true);
                            }
                        }
                        }
                    }
                    SpriteRenderer[] srs = block.GetComponentsInChildren<SpriteRenderer>(true);
            if (srs == null || srs.Length == 0)
                Debug.LogWarning($"{block.name} : no SpriteRenderer found on prefab. Check prefab structure.");
            else
            {
                foreach (var sr in srs)
                {
                    Debug.Log($"{block.name} : SpriteRenderer on '{sr.gameObject.name}' sprite={(sr.sprite != null ? sr.sprite.name : "null")} enabled={sr.enabled} color={sr.color} sortingLayer={sr.sortingLayerName} order={sr.sortingOrder} activeInHierarchy={sr.gameObject.activeInHierarchy}");
                }
            }

                }
            }
        }

        foreach (var go in spawnedBlocks)
        {
            if (go != null)
            {
               // DestroyImmediate(go);
            }
        }
        spawnedBlocks.Clear();
    }

    private GameObject GetPrefabForBlockType(int blockType)
    {
        switch (blockType)
        {
            case 1 : return defaultBlockPrefab;
            case 2 : return bonusBlockPrefab;
            case 3 : return hintBlockFarPrefab;
            case 4 : return hintBlockNearPrefab;
            case 5 : return coinBlockPrefab;
            case 6 : return mineralBlockPrefab;
            case 7 : return mediumMineralBlockPrefab;
            case 8 : return largeMineralBlockPrefab;
            default: return defaultBlockPrefab;
        }
    }

    private string GetBlockTypeName(int blockType)
    {
        switch (blockType)
        {
            case 1 : return "DefaultBlock";
            case 2 : return "BonusBlock";
            case 3 : return "HintBlockFar";
            case 4 : return "HintBlockNear";
            case 5 : return "CoinBlock";
            case 6 : return "MineralBlock";
            case 7 : return "MediumMineralBlock";
            case 8 : return "LargeMineralBlock";
            default: return "UnknownBlock";
        }
    }


    [ContextMenu("Clear Cave")]
    public void ClearCave()
    {
        if (blocksParent != null)
        {
            for (int i = blocksParent.childCount -1; i >= 0; i--)
            {
                DestroyImmediate(blocksParent.GetChild(i).gameObject);
            }
        }

        spawnedBlocks.Clear();
        wallPositions.Clear();
        bonusBlockPlaced = false;
    }
    
    [ContextMenu("SmoothMap")]
    private void SmoothMap()
    {
        int[,] smoothMap = new int[caveWidth, caveHeight];

        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                int neighborWallTiles = GetSurroundingWallCount(x, y);

                if (neighborWallTiles > 4)
                    smoothMap[x, y] = 1;
                else if (neighborWallTiles < 4)
                    smoothMap[x, y] = 0;
                else
                    smoothMap[x, y] = caveMap[x, y];
            }
        }

        caveMap = smoothMap;
    }

    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < caveWidth && neighborY >= 0 && neighborY < caveHeight)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += caveMap[neighborX, neighborY] > 0 ? 1 : 0;
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(caveWidth * tileSize / 2f, caveHeight * tileSize / 2f, 0);
        Vector3 size = new Vector3(caveWidth * tileSize, caveHeight * tileSize, 0.1f);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = new Color(1f, 0.84f, 0f, 0.3f);
        int minY = Mathf.RoundToInt(caveWidth * bonusZoneMinPercent);
        int maxY = Mathf.RoundToInt(caveWidth * bonusZoneMaxPercent);
        float zoneHeight = (maxY - minY) * tileSize;
        Vector3 bonusZoneCenter = new Vector3(caveWidth * tileSize / 2f, (minY * tileSize) + (zoneHeight / 2f), 0);
        Vector3 bonusZoneSize = new Vector3(caveWidth * tileSize, zoneHeight, 0.1f);
        Gizmos.DrawCube(bonusZoneCenter, bonusZoneSize);

        if (bonusBlockPosition != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 bonusPos = new Vector3(bonusBlockPosition.x * tileSize, bonusBlockPosition.y * tileSize, 0);
            Gizmos.DrawSphere(bonusPos, tileSize * 2f);

            Gizmos.color = Color.cyan;
            DrawCircle(bonusPos, farHintMinDistance * tileSize, 32);

            Gizmos.color = Color.magenta;
            DrawCircle(bonusPos, nearHintMinDistance * tileSize, 32);



        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 previousPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }

    private bool IsEdgePosition(Vector2Int pos)
    {
        return pos.x == 0 || pos.x == caveWidth-1 || pos.y == 0 || pos.y == caveHeight-1;
    }

}