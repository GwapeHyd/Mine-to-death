using UnityEngine;

public class HintBlock : MonoBehaviour
{
    [Header("Hint Block Type")]
    [SerializeField] private HintBlockType blockType;

    public enum HintBlockType
    {
        Far,
        Near
    }

    private AutoTileBlock autoTileBlock;

    private void Start()
    {
        autoTileBlock = GetComponent<AutoTileBlock>();

        if (autoTileBlock != null)
        {
            autoTileBlock.onBlockDestroyed.AddListener(OnBlockDestroyed);
        }
        else
        {
            Debug.LogError($"AutoTileBlock component missing on {gameObject.name}");
        }
    }

    private void OnBlockDestroyed()
    {
        if (SanityManager.Instance != null)
        {
            switch (blockType)
            {
                case HintBlockType.Far:
                    SanityManager.Instance.hintFarBlocksDestroyed++;
                    Debug.Log($"Hint Far Block destroyed. Total: {SanityManager.Instance.hintFarBlocksDestroyed}");
                    break;

                case HintBlockType.Near:
                    SanityManager.Instance.hintCloseBlocksDestroyed++;
                    Debug.Log($"Hint Near Block destroyed. Total: {SanityManager.Instance.hintCloseBlocksDestroyed}");
                    break;
            }
        }
        else
        {
            Debug.LogError("SanityManager instance not found!");
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