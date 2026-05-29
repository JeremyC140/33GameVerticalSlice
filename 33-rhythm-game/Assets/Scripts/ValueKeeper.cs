using UnityEngine;

public class ValueKeeper : MonoBehaviour
{
    public static ValueKeeper Instance { get; private set; }

    public float offset = 0f;
    public SongData chosenSong;
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
