#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AutoTileBlockSwapper : EditorWindow
{
    private AutoTileSpriteSet newSpriteSet;

    [MenuItem("Tools/AutoTile Block Swapper")]
    public static void ShowWindow()
    {
        GetWindow<AutoTileBlockSwapper>("Sprite Set Swapper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Swap all AutoTile Sprites", EditorStyles.boldLabel);
        
        newSpriteSet = (AutoTileSpriteSet)EditorGUILayout.ObjectField(
            "New Sprite Set", 
            newSpriteSet, 
            typeof(AutoTileSpriteSet), 
            false
        );

        if (GUILayout.Button("Swap All Blocks in Scene"))
        {
            SwapAllBlocks();
        }
    }

    private void SwapAllBlocks()
    {
        if (newSpriteSet == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Sprite Set first!", "OK");
            return;
        }

        AutoTileBlock[] allBlocks = FindObjectsByType<AutoTileBlock>(FindObjectsSortMode.None);
        
        Undo.RecordObjects(allBlocks, "Swap Sprite Sets");
        
        foreach (AutoTileBlock block in allBlocks)
        {
            block.SetSpriteSet(newSpriteSet);
            EditorUtility.SetDirty(block);
        }

        Debug.Log($"Swapped sprite sets for {allBlocks.Length} blocks!");
    }
}
#endif