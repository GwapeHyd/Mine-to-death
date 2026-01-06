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
    }

    private void Start()
    {
        onCoinCountChanged?.Invoke(totalCoins);
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log($"Added {amount} coins. Total coins: {totalCoins}");

        onCoinsAdded?.Invoke(amount);
        onCoinCountChanged?.Invoke(totalCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
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

    public int TotalCoins => totalCoins;

    

}
