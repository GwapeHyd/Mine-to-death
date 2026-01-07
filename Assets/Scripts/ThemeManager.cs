using System;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Serializable]
    public class SpritePair
    {
        public string key;
        public Sprite spriteSetA;
        public Sprite spriteSetB;
    }

    [Header("Sprite database (key => setA/setB)")]
    public SpritePair[] spritePairs;
    private Dictionary<string, SpritePair> map;
    public int CurrentIndex { get; private set; } = 0;

    public event Action<int> OnThemeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildMap()
    {
        map = new Dictionary<string, SpritePair>();
        foreach (var pair in spritePairs)
        {
            if(!string.IsNullOrEmpty(pair.key) && !map.ContainsKey(pair.key))
                map[pair.key] = pair;
        }
    }

    public Sprite GetSprite(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        if (map == null)
            BuildMap();

        if (map != null && map.TryGetValue(key, out var pair))
        {
            return CurrentIndex == 0 ? pair.spriteSetA : pair.spriteSetB;
        }
        return null;
    }

    public void SetIndex(int index)
    {
        index = (index == 0) ? 0 : 1;
        if (index == CurrentIndex) return;
        CurrentIndex = index;
        OnThemeChanged?.Invoke(CurrentIndex);
    }

    public void ToggleTheme()
    {
        SetIndex(1 - CurrentIndex);
    }
}
