using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Coins")]
    [SerializeField] private int totalCoins = 0;

    
    public UnityEvent<int> onCoinCountChanged;
    public UnityEvent<int> onCoinsAdded;
    
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

        if (onCoinCountChanged == null)
        {
            onCoinCountChanged = new UnityEvent<int>();
        }
    }

    private void Start()
    {
        onCoinCountChanged?.Invoke(totalCoins);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"Added {amount} coins. Total now: {totalCoins}");
        onCoinsAdded?.Invoke(amount);
        Debug.Log("onCoinsAdded event invoked.");
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            TotalCoins -= amount;
            onCoinCountChanged?.Invoke(totalCoins);
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        totalCoins = 0;
        onCoinCountChanged?.Invoke(totalCoins);
    }

    public int TotalCoins
    {
        get => totalCoins;
        private set
        {
            totalCoins = value;
            onCoinCountChanged?.Invoke(totalCoins);
        }
    }

    

}
