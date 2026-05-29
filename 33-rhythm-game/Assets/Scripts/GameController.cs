using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // --- Singleton Setup ---
    public static GameController Instance { get; private set; }

    public GameObject visualScriptingTarget;

    [Header("Level Data")]
    public SongData currentSong;

    [Header("User Settings")]
    [Tooltip("Global audio offset in seconds. Positive = audio plays later; Negative = audio plays earlier.")]
    public float offset = 0f; // Time in seconds to shift all notes (positive = later, negative = earlier)

    private int currentCombo = 0;

    // --- Events ---
    public static event Action OnPauseGame;
    public static event Action OnRestartGame;
    public static event Action OnQuitGame;

    private float _gameDefaultStartTime = 2f;
    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        offset = ValueKeeper.Instance.offset;
        currentSong = ValueKeeper.Instance.chosenSong;
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySong(currentSong, AudioSettings.dspTime + _gameDefaultStartTime);
            Debug.Log($"Playing song: {currentSong.songName}");
        }
        NoteSpawner noteSpawner = FindAnyObjectByType<NoteSpawner>();
        if (noteSpawner != null)
        {
            noteSpawner.StartSpawning(currentSong, offset);
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
        if (Input.GetKeyDown(KeyCode.Q)) {
            QuitToMenu();
        }
    }

    public void triggerPerfectHit() {
        currentCombo++;
        CustomEvent.Trigger(visualScriptingTarget, "PerfectHit", currentCombo.ToString());
    }
    public void triggerGoodHit()
    {
        currentCombo++;
        CustomEvent.Trigger(visualScriptingTarget, "GoodHit", currentCombo.ToString());
    }
    public void triggerMissHit()
    {
        currentCombo = 0;
        CustomEvent.Trigger(visualScriptingTarget, "MissHit", currentCombo.ToString());
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        OnPauseGame?.Invoke();
        if (_isPaused)
        {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        OnQuitGame?.Invoke();
        SceneController.Instance.LoadScene("SelectionMenu");
    }
}
