using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;


public class TipTrigger : MonoBehaviour
{
    [Tooltip("Liste locale de tips spécifiques à ce trigger. Si vide, utilisera le TipManager global.")]
    [SerializeField] private string[] localTips;
    [SerializeField] private float textSpeed;
    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered = false;
    [SerializeField] private GameObject tipUI;

    private int index;

    private void Awake()
    {
        index = -1;
    }

    private void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {    
        if (localTips == null || localTips.Length == 0)
        {
            Debug.LogWarning("No local tips assigned to this TipTrigger.");
            yield break;
        }

        if (index < 0 || index >= localTips.Length)
        {
            Debug.LogWarning("Tip index out of range.");
            yield break;
        }

        if (tipUI == null)
        {
            Debug.LogWarning("Tip UI GameObject is not assigned.");
            yield break;
        }

        TextMeshPro tipText = tipUI.GetComponentInChildren<TextMeshPro>();
        if (tipText == null)
        {
            Debug.LogWarning("No TextMeshPro component found in Tip UI.");
            yield break;
        }
        tipText.text = "";

        foreach (char letter in localTips[index].ToCharArray())
        {
            tipText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            if (localTips == null || localTips.Length == 0)
            {
                Debug.LogWarning("No local tips assigned to this TipTrigger.");
                return;
            }
            if (tipUI == null)
            {
                Debug.LogWarning("Tip UI GameObject is not assigned.");
                return;
            }

            index++;

            if (index >= localTips.Length)
            {
                if (triggerOnce)
                {
                    hasTriggered = true;
                    return;
                }
                else
                {
                    index = 0; 
                }
                
            }
            tipUI.SetActive(true);  
            StartDialogue();          
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