using TMPro;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField] private GameObject interactionEffect;
    private string promptText;

    private void Start()
    {
        if (interactionEffect != null)
        {
            Debug.Log("Interaction effect found and assigned.");
            promptText = interactionEffect.GetComponent<TextMeshPro>().text;
            interactionEffect.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            EditPromptText($"You have died {playerHealth.NumberOfDeaths} times.");
            if (interactionEffect != null && promptText != null)
            {
                interactionEffect.GetComponent<TextMeshPro>().text = promptText;
            }
            else
            {
                Debug.LogWarning("Prompt text component is missing.");
            }
            if (interactionEffect != null)
            {
                interactionEffect.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionEffect != null)
            {
                interactionEffect.SetActive(false);
            }
        }
    }

    private void EditPromptText(string newText)
    {
        if (promptText != null)
        {
            promptText = newText;
        }
    }

    public void SetUIColor(Color newColor)
    {
        TextMeshPro tmp = interactionEffect.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.color = newColor;
        }
    }
}
