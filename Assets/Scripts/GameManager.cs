using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    [ContextMenu("Delete All PlayerPrefs")]
    public void DeleteAllPlayersPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been deleted.");
    }
}
