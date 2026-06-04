using UnityEngine;

public class ValueKeeper : MonoBehaviour
{
    public static ValueKeeper Instance { get; private set; }

    public float offset = 0f;
    public float speed = 1.75f;
    public SongData chosenSong;

    public int perfectCount = 0;
    public int goodCount = 0;
    public int missCount = 0;
    public int totalCount = 0;
    public float accuracy = 0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Forcefully destroy the duplicate GameObject immediately 
            // before it can mess up any other scripts
            Destroy(gameObject);
        }
    }
}
