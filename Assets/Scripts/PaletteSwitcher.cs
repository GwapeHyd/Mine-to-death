using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaletteSwitcher : MonoBehaviour
{
    public Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;
    
    [System.Serializable]
    public struct GameBoyPalette
    {
        public string name;
        public Color tintColor;
        public float contrast;
    }
    
    public GameBoyPalette[] palettes = new GameBoyPalette[]
    {
        new GameBoyPalette { name = "Red", tintColor = new Color(0.8f, 0.2f, 0.3f), contrast = 20f },
        new GameBoyPalette { name = "Classic", tintColor = Color.white, contrast = 30f }
    };
    
    private int currentPaletteIndex = 0;

    void Start()
    {
        if (postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            ApplyPalette(0);
        }
    }

    public void SwitchPalette()
    {
        currentPaletteIndex = (currentPaletteIndex + 1) % palettes.Length;
        ApplyPalette(currentPaletteIndex);
    }

    void ApplyPalette(int index)
    {
        colorAdjustments. colorFilter. value = palettes[index].tintColor;
        colorAdjustments.contrast. value = palettes[index].contrast;
    }
}