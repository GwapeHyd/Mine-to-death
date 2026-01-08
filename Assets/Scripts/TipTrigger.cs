using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;


public class TipTrigger : MonoBehaviour
{
    [Tooltip("Liste locale de tips spécifiques à ce trigger. Si vide, utilisera le TipManager global.")]
    [SerializeField] private List<string> localTips = new List<string>();
    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered = false;
    [SerializeField] private GameObject tipUI;

    private TipManager tipManager;

    private void Awake()
    {
        tipManager = FindFirstObjectByType<TipManager>();
    }
    public string GetFirstTip()
    {
        if (localTips != null && localTips.Count > 0)
        {
            return localTips[0];
        }
        else if (tipManager != null)
        {
            return tipManager.GetNextTip();
        }
        return string.Empty;
    }

    public string GetTip()
    {
        if (localTips != null && localTips.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, localTips.Count);
            return localTips[randomIndex];
        }
        else if (tipManager != null)
        {
            return tipManager.GetNextTip();
        }
        return string.Empty;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            string tip = GetTip();
            if (tip != localTips[0])
            {
                if (triggerOnce && !hasTriggered)
                {
                    tip = GetFirstTip();
                    hasTriggered = true;
                }
            }
            
            if (!string.IsNullOrEmpty(tip))
            {
                tipUI.SetActive(true);
                TextMeshPro tipText = tipUI.GetComponentInChildren<TextMeshPro>();
                if (tipText != null)
                {
                    Debug.Log("Displaying tip: " + tip);
                    tipText.text = tip;
                }
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tipUI.SetActive(false);
        }
    }



    
}