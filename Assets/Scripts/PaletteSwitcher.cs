using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaletteSwitcher : MonoBehaviour
{
    public Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    [System.Serializable]
    public class PalettePreset
    {
        public string name;
        public Color colorFilter = Color.white;
        public float saturation = 0f;
        public Color vignetteColor = Color.black;
    }
    [SerializeField] private PalettePreset[] palettes;
    
    
    private int currentPaletteIndex = 0;

    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
            postProcessVolume.profile.TryGet<Vignette>(out vignette);
        }
        ApplyPalette(currentPaletteIndex);
    }

    public void SwitchPalette()
    {
        currentPaletteIndex = (currentPaletteIndex + 1) % palettes.Length;
        ApplyPalette(currentPaletteIndex);
    }

    void ApplyPalette(int index)
    {
        if (palettes == null || index >= palettes.Length) return;

        PalettePreset palette = palettes[index];
        colorAdjustments.colorFilter.value = palette.colorFilter;
        colorAdjustments.saturation.value = palette.saturation;

        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = palette.colorFilter;
            colorAdjustments.saturation.value = palette.saturation;
        }

        if (vignette != null)
        {
            vignette.color.value = palette.vignetteColor;
        }
    }
}