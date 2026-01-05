using UnityEngine;

public class BlockTriggerZone : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private AutoTileBlock parentBlock; // Ou AutoTileBlock selon votre choix

    private void Start()
    {
        parentBlock = GetComponentInParent<AutoTileBlock>();
        
        if (parentBlock == null)
        {
            Debug.LogError("BlockTriggerZone must be child of AutoTileBlock!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && parentBlock != null)
        {
            parentBlock.OnPlayerEnterRange();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && parentBlock != null)
        {
            parentBlock.OnPlayerExitRange();
        }
    }
}