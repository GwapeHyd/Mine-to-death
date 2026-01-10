using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FairyShop : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.A;
    [SerializeField] private string playerTag = "Player";

    [Header("UI Elements")]
    [SerializeField] private GameObject fairyShopUI;
    [SerializeField] private GameObject interactionFeedback;

    [Header("Events")]
    [SerializeField] private UnityEvent onFairyShopOpened;
    [SerializeField] private UnityEvent onFairyShopClosed;

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;
    private PlayerController playerController;
    public bool isLeveledUp = false;

    private void Start()
    {
        if (fairyShopUI != null)
            fairyShopUI.SetActive(false);

        if (interactionFeedback != null)
            interactionFeedback.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (isShopOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }

        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }

        if (isLeveledUp)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.SetTrigger("Upgrade");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            if (interactionFeedback != null)
                interactionFeedback.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            if (interactionFeedback != null)
                interactionFeedback.SetActive(false);

            if (isShopOpen)
            {
                CloseShop();
            }
        }
    }

    private void OpenShop()
    {
        isShopOpen = true;

        if (fairyShopUI != null)
            fairyShopUI.SetActive(true);

        if (interactionFeedback != null)
            interactionFeedback.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        onFairyShopOpened?.Invoke();
    }

    private void CloseShop()
    {
        isShopOpen = false;

        if (fairyShopUI != null)
            fairyShopUI.SetActive(false);

        if (isPlayerInRange && interactionFeedback != null)
            interactionFeedback.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        onFairyShopClosed?.Invoke();
    }

    public void OnCloseButtonClicked()
    {
        CloseShop();
    }

    public void SetUIColor(Color newColor, Color newColor2)
    {
        Debug.Log("Setting Fairy Shop UI colors.");
        if (fairyShopUI != null)
        {
            Image[] images = fairyShopUI.GetComponentsInChildren<Image>();
            Image backgroundImage = fairyShopUI.GetComponent<Image>(); 
            foreach (var img in images)
            {
                img.color = newColor2;
            }
            foreach (var icon in images)
            {
                if (icon != null)
                {
                    icon.color = newColor;
                }
            }
            backgroundImage.color = newColor2;

            TMPro.TextMeshProUGUI[] texts = fairyShopUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach (var txt in texts)
            {
                txt.color = newColor;
            }

            TMPro.TextMeshPro[] tmpTexts = interactionFeedback.GetComponentsInChildren<TMPro.TextMeshPro>();
            foreach (var tmp in tmpTexts)
            {
                tmp.color = newColor2;
            }

            GameObject interactTextObj = interactionFeedback.transform.Find("InteractText")?.gameObject;
            if (interactTextObj != null)
            {
                TMPro.TextMeshProUGUI interactText = interactTextObj.GetComponent<TMPro.TextMeshProUGUI>();
                if (interactText != null)
                {
                    interactText.color = newColor;
                }
            }
        }
    }
}