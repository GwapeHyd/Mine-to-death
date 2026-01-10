using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.A;
    [SerializeField] private string playerTag = "Player";

    [Header("UI Elements")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject interactionFeedback;

    [Header("Events")]
    [SerializeField] private UnityEvent onShopOpened;
    [SerializeField] private UnityEvent onShopClosed;
    
    [SerializeField] private Image[] iconImages;

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;
    private bool isActivated = false;
    private PlayerController playerController;

    private void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            if (interactionFeedback != null)
            {
                interactionFeedback.SetActive(true);
                if (!isActivated)
                {
                    interactionFeedback.GetComponent<TMPro.TextMeshPro>().text = "Come back later";
                }
                else
                {
                    interactionFeedback.GetComponent<TMPro.TextMeshPro>().text = "Press E to Open Shop";
                }
            }
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

        if (shopUI != null)
            shopUI.SetActive(true);

        if (interactionFeedback != null)
            interactionFeedback.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        onShopOpened?.Invoke();
    }

    private void CloseShop()
    {
        isShopOpen = false;

        if (shopUI != null)
            shopUI.SetActive(false);

        if (isPlayerInRange && interactionFeedback != null)
            interactionFeedback.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        onShopClosed?.Invoke();
    }

    public void OnCloseButtonClicked()
    {
        CloseShop();
    }

    public void SetUIColor(Color newColor, Color newColor2)
    {
        if (shopUI != null)
        {
            Image[] images = shopUI.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                img.color = newColor2;
            }
            foreach (var icon in iconImages)
            {
                if (icon != null)
                {
                    icon.color = newColor;
                }
            }

            TMPro.TextMeshProUGUI[] texts = shopUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach (var txt in texts)
            {
                txt.color = newColor;
            }

            TMPro.TextMeshPro[] tmpTexts = interactionFeedback.GetComponentsInChildren<TMPro.TextMeshPro>();
            foreach (var tmp in tmpTexts)
            {
                tmp.color = newColor;
            }
        }
    }

    public void ActivateShop()
    {
        isActivated = true;
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("ActivateShop");
        }
    }
}