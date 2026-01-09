using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Sanity Settings")]
    [SerializeField] private int maxSanity = 100;
    [SerializeField] private int sanityDecreaseRate = 1;

    [SerializeField] private GameObject fairyShopGO;
    
    private int currentSanity;
    private float sanityDecreaseTimer = 1f;
    private GameObject player;

    public UnityEvent<int, int> onSanityChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            onSanityChanged = new UnityEvent<int, int>();
            currentSanity = maxSanity;
        }
        else
        {
            Debug.LogWarning("Duplicate SanityManager instance found. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    private void Update()
    {

        if (player.transform.position.y < 0f)
        {
            sanityDecreaseTimer -= Time.deltaTime * 3f;
            if (sanityDecreaseTimer <= 0f)
            {
                DecreaseSanity(sanityDecreaseRate);
                sanityDecreaseTimer = 1f;
            }
        }
        else if (player.transform.position.y < 60f)
        {
            sanityDecreaseTimer -= Time.deltaTime * 2f;
            if (sanityDecreaseTimer <= 0f)
            {
                DecreaseSanity(sanityDecreaseRate);
                sanityDecreaseTimer = 1f;
            }
        }
        else if (player.transform.position.y < 120f)
        {
            sanityDecreaseTimer -= Time.deltaTime * 1f;
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
                    fairyShopGO.SetActive(true);
                    currentSanity = maxSanity;
                    onSanityChanged?.Invoke(currentSanity, maxSanity);
                }
            }
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


    public void AddMadnessPsychosis()
    {
        SetMaxSanity(maxSanity + 5);
        IncreaseSanity(5);
        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    public void SetMaxSanity(int newMaxSanity)
    {
        maxSanity = newMaxSanity;
        currentSanity = Mathf.Min(currentSanity, maxSanity);
        onSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    public int CurrentSanity => currentSanity;
    public int MaxSanity => maxSanity;

}
