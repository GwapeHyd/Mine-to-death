using UnityEngine;
using UnityEngine.Events;

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

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;
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
}