using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent] //empeche la creation du meme component plusieurs fois
public class PlayerVisibilityHalo : MonoBehaviour
{
    //declare le rayon du halo
    [Header("Radius")]
    [SerializeField] private int blocksVisible = 4;
    [SerializeField] private float blockSizeInUnits = 1f;
    [SerializeField] private float innerRadius = 0f;

    //declare la couleur de la lumiere + intensite
    [Header("Light")]
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float lightIntensity = 1f;

    //declare la nuit autour du halo
    [Header("Darkness")]
    [SerializeField] private bool ensureGlobalDarkness = true;
    [SerializeField] private float globalIntensity = 0f;

    private const string HaloName = "VisibilityHalo";
    //Light2D = type lumiere, couleur, intensite, rayon, layers affectés
    private Light2D haloLight;

    private void Awake()
    {
        haloLight = GetOrCreateHaloLight();
        ApplyHaloSettings();

        if (ensureGlobalDarkness)
        {
            EnsureGlobalLight();
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        haloLight = TryGetExistingHaloLight();
        ApplyHaloSettings();
    }

    private Light2D GetOrCreateHaloLight()
    {
        Transform existing = transform.Find(HaloName);
        if (existing == null)
        {
            GameObject halo = new GameObject(HaloName);
            halo.transform.SetParent(transform, false);
            existing = halo.transform;
        }

        Light2D light = existing.GetComponent<Light2D>();
        if (light == null)
        {
            light = existing.gameObject.AddComponent<Light2D>();
        }

        return light;
    }

    private Light2D TryGetExistingHaloLight()
    {
        Transform existing = transform.Find(HaloName);
        if (existing == null)
        {
            return null;
        }

        return existing.GetComponent<Light2D>();
    }

    private void ApplyHaloSettings()
    {
        if (haloLight == null)
        {
            return;
        }

        haloLight.lightType = Light2D.LightType.Point;
        haloLight.pointLightOuterRadius = Mathf.Max(0f, blocksVisible * blockSizeInUnits);
        haloLight.pointLightInnerRadius = Mathf.Max(0f, innerRadius);
        haloLight.intensity = Mathf.Max(0f, lightIntensity);
        haloLight.color = lightColor;
    }

    private void EnsureGlobalLight()
    {
        Light2D global = null;
        Light2D[] lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (Light2D light in lights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                global = light;
                break;
            }
        }

        if (global == null)
        {
            GameObject globalLight = new GameObject("Global Light 2D");
            global = globalLight.AddComponent<Light2D>();
            global.lightType = Light2D.LightType.Global;
        }

        global.intensity = Mathf.Clamp01(globalIntensity);
    }
}
