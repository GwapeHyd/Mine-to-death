using UnityEngine;
using UnityEngine.Events;


public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Sanity Settings")]
    [SerializeField] private int maxSanity = 100;
    [SerializeField] private int sanityDecreaseRate = 1;
    [Header("Hint Block Settings")]
    [SerializeField] private int maxCave = 4;
    [SerializeField] private GameObject bonusBlockPrefab;
    

    [Header("Hint Settings")]
    [SerializeField] private GameObject hintFarFeedbackGO;
    [SerializeField] private GameObject hintCloseFeedbackGO;
    [SerializeField] private int currentHintBlocks = 0;
    public int hintFarBlocksDestroyed;
    public int hintCloseBlocksDestroyed;


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
        bonusBlockPrefab.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        hintCloseFeedbackGO.SetActive(false);
        hintFarFeedbackGO.SetActive(false);

        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    private void Update()
    {
        if (currentHintBlocks <= 0)
        {
            bonusBlockPrefab.SetActive(true);
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
                }
            }
        }

        if (hintFarBlocksDestroyed >= 1)
        {
            hintFarFeedbackGO.SetActive(true);
        }
        else
        {
            hintFarFeedbackGO.SetActive(false);
        }

        if (hintCloseBlocksDestroyed >= 1)
        {
            hintCloseFeedbackGO.SetActive(true);
        }
        else
        {
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

    public int CurrentSanity => currentSanity;
    public int MaxSanity => maxSanity;

}
