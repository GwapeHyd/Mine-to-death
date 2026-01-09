using System.Collections;
using TMPro;
using UnityEngine;

public class TheDoor : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] Animator doorAnimator;
    private bool isDoorOpen = false;

    private void Update()
    {
        if (interactionPrompt != null && interactionPrompt.activeSelf && !isDoorOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionPrompt.SetActive(false);
                interactionPrompt.GetComponent<TextMeshPro>().text = "Press E to Close Door";
                isDoorOpen = true;
                doorAnimator.SetTrigger("OpenDoor");
                StartCoroutine(WaitForAnimationToDesactivateColliderCoroutine(.8f));
                StartCoroutine(CloseDoorAfterDelayCoroutine(5.0f));
            }
        }

        if (interactionPrompt != null && interactionPrompt.activeSelf && isDoorOpen)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                interactionPrompt.SetActive(false);
                interactionPrompt.GetComponent<TextMeshPro>().text = "Press E to Open Door";
                isDoorOpen = false;
                doorAnimator.SetTrigger("CloseDoor");
                StartCoroutine(WaitForAnimationToActivateColliderCoroutine(1.0f));
            }
        }
    }

    private IEnumerator WaitForAnimationToActivateColliderCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        doorCollider.enabled = true;
    }

    private IEnumerator WaitForAnimationToDesactivateColliderCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        doorCollider.enabled = false;
    }

    private IEnumerator CloseDoorAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        isDoorOpen = false;
        doorAnimator.SetTrigger("CloseDoor");
        StartCoroutine(WaitForAnimationToActivateColliderCoroutine(1.0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}