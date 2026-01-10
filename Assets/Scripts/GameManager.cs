using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("EndGame Settings")]
    [SerializeField] private GameObject chestContainer;
    [SerializeField] private GameObject bonusBlock;

    [Header("Hint Settings")]
    [SerializeField] private GameObject hintFarFeedbackGO;
    [SerializeField] private GameObject hintCloseFeedbackGO;
    [SerializeField] private GameObject endMenuUI;
    public int maxHintBlocks;
    private int currentHintBlocks = 0;
    public int hintFarBlocksDestroyed;
    public int hintCloseBlocksDestroyed;
    private int hintBlocksDestroyed => hintFarBlocksDestroyed + hintCloseBlocksDestroyed;

    private TextMeshPro hintFarText;
    private TextMeshPro hintCloseText;
    public bool gameOver = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        hintCloseFeedbackGO.SetActive(false);
        hintFarFeedbackGO.SetActive(false);
        hintFarText = hintFarFeedbackGO.GetComponentInChildren<TextMeshPro>();
        hintCloseText = hintCloseFeedbackGO.GetComponentInChildren<TextMeshPro>();

        currentHintBlocks = maxHintBlocks;

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerHealth.onDeath.AddListener(ActivateShopOnFirstDeath);

        StartGame();
    }

    private void Update()
    {
        if (gameOver)
        {
            StartCoroutine(EndGameCoroutine());
            return;
        }
        if (bonusBlock == null || chestContainer == null) return;
        

        if (hintBlocksDestroyed < maxHintBlocks && bonusBlock.activeSelf)
        {
            bonusBlock.SetActive(false);
        }
        else if (hintBlocksDestroyed >= maxHintBlocks && !bonusBlock.activeSelf)
        {
            bonusBlock.SetActive(true);
        }

        UpdateHintFeedback();
    }

    private void ActivateShopOnFirstDeath()
    {
        Shop shop = FindFirstObjectByType<Shop>();
        if (shop != null)
        {
            shop.ActivateShop();
        }
    }

    private void UpdateHintFeedback()
    {
        if (hintFarBlocksDestroyed > 0)
        {
            if (!hintFarFeedbackGO.activeSelf)
            {
                hintFarFeedbackGO.SetActive(true);
            }

            if (hintFarText != null)
            {
                hintFarText.text = "x" + hintFarBlocksDestroyed.ToString();
            }
        }
        else 
        {
            if (hintFarFeedbackGO.activeSelf)
                hintFarFeedbackGO.SetActive(false);
        }

        if (hintCloseBlocksDestroyed > 0)
        {
            if (!hintCloseFeedbackGO.activeSelf)
            {
                hintCloseFeedbackGO.SetActive(true);
            }

            if (hintCloseText != null)
            {
                hintCloseText.text = "x" + hintCloseBlocksDestroyed.ToString();
            }
        }
        else 
        {
            if (hintCloseFeedbackGO.activeSelf)
                hintCloseFeedbackGO.SetActive(false);
        }
    }
    
    public void IncrementHintBlocksDestroyed(bool isFarBlock)
    {
        if (isFarBlock)
        {
            hintFarBlocksDestroyed++;
        }
        else
        {
            hintCloseBlocksDestroyed++;
        }
    }


    [ContextMenu("Delete All PlayerPrefs")]
    public void DeleteAllPlayersPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been deleted.");
    }

    public void StartGame()
    {
        chestContainer.SetActive(true);
    }

    public void WinGame()
    {
        chestContainer.SetActive(false);
        ActivateFairyShop();
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableDoubleJump();
        }
    }

    public void QuitGame()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // redirige vers la page d'accueil ou une page "merci"
        Application.OpenURL("https://gwapehyd.itch.io/mine-to-death-test-build");
#else
        Application.Quit();
#endif
    }  

    public void TogglePauseMenu()
    {
        bool isPaused = Time.timeScale == 0f;

        if (isPaused)
        {
            Time.timeScale = 1f;
            endMenuUI.SetActive(false);
        }
        else
        {
            endMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void ActivateFairyShop()
    {
        FairyShop fairyShop = FindFirstObjectByType<FairyShop>();
        if (fairyShop != null)
        {
            fairyShop.isLeveledUp = true;
        }
    }

    private IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        endMenuUI.SetActive(true);
            Time.timeScale = 0f;
    }
}
