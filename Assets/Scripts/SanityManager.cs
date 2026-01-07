using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Sanity Settings")]
    [SerializeField] private int maxSanity = 100;
    [SerializeField] private int sanityDecreaseRate = 1;

    [SerializeField] private GameObject bonusBlock;
    

    [Header("Hint Settings")]
    [SerializeField] private GameObject hintFarFeedbackGO;
    [SerializeField] private GameObject hintCloseFeedbackGO;
    public int maxHintBlocks;
    private int currentHintBlocks = 0;
    public int hintFarBlocksDestroyed;
    public int hintCloseBlocksDestroyed;
    private int hintBlocksDestroyed => hintFarBlocksDestroyed + hintCloseBlocksDestroyed;

    private TextMeshPro hintFarText;
    private TextMeshPro hintCloseText;


    private int currentSanity;
    private float sanityDecreaseTimer = 1f;
    private GameObject player;

    public UnityEvent<int, int> onSanityChanged;

    private void Awake()
    {
        Debug.Log("SanityManager Awake called.");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            onSanityChanged = new UnityEvent<int, int>();
            currentSanity = maxSanity;

            Debug.Log("SanityManager instance set.");
        }
        else
        {
            Debug.LogWarning("Duplicate SanityManager instance found. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bonusBlock.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        hintCloseFeedbackGO.SetActive(false);
        hintFarFeedbackGO.SetActive(false);
        hintFarText = hintFarFeedbackGO.GetComponentInChildren<TextMeshPro>();
        hintCloseText = hintCloseFeedbackGO.GetComponentInChildren<TextMeshPro>();

        currentHintBlocks = maxHintBlocks;

        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    private void Update()
    {
        if (currentHintBlocks <= 0)
        {
            bonusBlock.SetActive(true);
        }

        if (player.transform.position.y < 350)
        {
            sanityDecreaseTimer -= Time.deltaTime;
            if (sanityDecreaseTimer <= 0f)
            {
                DecreaseSanity(sanityDecreaseRate);
                sanityDecreaseTimer = 1f;
            }
        }

        if (currentSanity <= 0)
        {
            if (player != null)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Die();
                    currentSanity = maxSanity;
                    onSanityChanged?.Invoke(currentSanity, maxSanity);
                    // lose game logic
                }
            }
        }

        if (hintBlocksDestroyed < maxHintBlocks)
        {
            bonusBlock.SetActive(false);
        }
        else
        {
            bonusBlock.SetActive(true);
        }


        UpdateHintFeedback();
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
    public void DecreaseSanity(int amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Max(currentSanity, 0);
        Debug.Log($"Sanity decreased by {amount}. Current sanity: {currentSanity}/{maxSanity}");

        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    public void IncreaseSanity(int amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Min(currentSanity, maxSanity);
        Debug.Log($"Sanity increased by {amount}. Current sanity: {currentSanity}/{maxSanity}");

        onSanityChanged?.Invoke(currentSanity, maxSanity);
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

    public int CurrentSanity => currentSanity;
    public int MaxSanity => maxSanity;
    public int CurrentHintBlocks => currentHintBlocks;

}
