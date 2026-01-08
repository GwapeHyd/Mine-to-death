using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    [Tooltip("Liste centrale de tips.")]
    [SerializeField] public List<string> tips = new List<string>();

    private List<int> remainingIndices = new List<int>();
    private System.Random rng = new System.Random();

    private void Awake()
    {
        RebuildAndShufflePool();
    }

    private void RebuildAndShufflePool()
    {
        remainingIndices = Enumerable.Range(0, Mathf.Max(0, tips.Count)).ToList();
        Shuffle(remainingIndices);
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T tmp = list[k];
            list[k] = list[n];
            list[n] = tmp;
        }
    }

    public string GetNextTip()
    {
        if (tips == null || tips.Count == 0) return string.Empty;
        if (remainingIndices.Count == 0) RebuildAndShufflePool();
        int idx = remainingIndices[0];
        remainingIndices.RemoveAt(0);
        return tips[idx];
    }

}