using UnityEngine;
using System;
using Unity.VectorGraphics; // Required for 'Action' events

public class GameController : MonoBehaviour
{
    // --- Singleton Setup ---
    public static GameController Instance { get; private set; }

    [Header("Level Data")]
    public SongData currentSong;

    // --- Events ---
    public static event Action OnPauseGame;
    public static event Action OnRestartGame;
    public static event Action OnQuitGame;

    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance != this && Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySong(currentSong);
            Debug.Log($"Playing song: {currentSong.songName}");
        }
        NoteSpawner noteSpawner = FindAnyObjectByType<NoteSpawner>();
        if (noteSpawner != null)
        {
            noteSpawner.StartSpawning(currentSong);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.Z)) { 
            RestartLevel();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            OnPauseGame?.Invoke();
            Time.timeScale = 0f;
        }
        else { 
            Time.timeScale = 1f;
        }
        // set Time.timeScale to 0 (pause) or 1 (resume)!
    }

    public void RestartLevel()
    {
        OnRestartGame?.Invoke();
        // HINT: You might want to reload the scene or just reset the timers/notes.
    }

    public void QuitToMenu()
    {
        OnQuitGame?.Invoke();
    }
}
