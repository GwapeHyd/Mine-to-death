using UnityEngine;
using UnityEngine.Events;

public class MineralManager : MonoBehaviour
{
    public static MineralManager Instance;

    [Header("Minerals")]
    [SerializeField] private int totalMinerals;
    public int TotalMinerals
    {
        get { return totalMinerals; }
        set
        {
            totalMinerals = Mathf.Max(0, value);
        }
    }
    public UnityEvent<int> onMineralCountChanged;
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

        if (onMineralCountChanged == null)
        {
            onMineralCountChanged = new UnityEvent<int>();
        }
    }

    private void Start()
    {
        LoadMinerals();
    }

    public void AddMinerals(int amount)
    {
        TotalMinerals += amount;
        onMineralCountChanged?.Invoke(totalMinerals);
    }

    public bool SpendMinerals(int amount)
    {
        if (totalMinerals >= amount)
        {
            TotalMinerals -= amount;
            onMineralCountChanged?.Invoke(totalMinerals);
            return true;
        }
        return false;
    }

    public void ResetMinerals()
    {
        totalMinerals = 0;
        onMineralCountChanged?.Invoke(totalMinerals);
    }

    public void SaveMinerals()
    {
        PlayerPrefs.SetInt("TotalMinerals", totalMinerals);
        PlayerPrefs.Save();
        Debug.Log($"Saved total minerals: {totalMinerals}");
    }

    private void LoadMinerals()
    {
        totalMinerals = PlayerPrefs.GetInt("TotalMinerals", 0);
        Debug.Log($"Loaded total minerals: {totalMinerals}");
        onMineralCountChanged?.Invoke(totalMinerals);
    }
}
