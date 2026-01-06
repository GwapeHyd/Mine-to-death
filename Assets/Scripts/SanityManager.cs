using UnityEngine;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance;
    [SerializeField] private int maxCave = 4;
    public int currentHintBlock = 0;

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
}
