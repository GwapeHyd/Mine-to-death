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
        TextMeshPro tipText = tipUI.GetComponentInChildren<TextMeshPro>();
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
            index++;
            tipUI.SetActive(true);  
            StartDialogue();     
            if (index >= localTips.Length)
            {
                hasTriggered = true;
                index = 0;
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