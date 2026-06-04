using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public float approachSpeed = 1.5f;

    [Header("Audio Components")]
    public AudioSource sfxSource;
    public AudioClip perfectSoundEffect;
    public AudioClip hitSoundEffect;

    private int currentCombo = 0;
    private int numPerfectHit = 0;
    private int numGoodHit = 0;
    private int numMissHit = 0;
    private int numTotalHit;

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
        approachSpeed = ValueKeeper.Instance.speed;
        currentSong = ValueKeeper.Instance.chosenSong;

        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySong(currentSong, AudioSettings.dspTime + _gameDefaultStartTime);
            Debug.Log($"Playing song: {currentSong.songName}");
        }

        StartCoroutine(DesignatedWait(_gameDefaultStartTime));
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
        numPerfectHit++;
        numTotalHit++;
        currentCombo++;
        sfxSource.PlayOneShot(perfectSoundEffect);
        CustomEvent.Trigger(visualScriptingTarget, "PerfectHit", currentCombo.ToString());
    }
    public void triggerGoodHit()
    {
        numGoodHit++;
        numTotalHit++;
        currentCombo++;
        sfxSource.PlayOneShot(perfectSoundEffect, 0.5f);
        CustomEvent.Trigger(visualScriptingTarget, "GoodHit", currentCombo.ToString());
    }
    public void triggerMissHit()
    {
        numMissHit++;
        numTotalHit++;
        currentCombo = 0;
        CustomEvent.Trigger(visualScriptingTarget, "MissHit", currentCombo.ToString());
    }

    public void playHitSoundEffect() {
        sfxSource.PlayOneShot(hitSoundEffect);
    }

    public void HandleGameResultsAndTransition()
    {
        ValueKeeper.Instance.perfectCount = numPerfectHit;
        ValueKeeper.Instance.goodCount = numGoodHit;
        ValueKeeper.Instance.missCount = numMissHit;
        ValueKeeper.Instance.totalCount = numTotalHit;
        ValueKeeper.Instance.accuracy = (float)(numPerfectHit + 0.9 * numGoodHit) / numTotalHit;

        SceneController.Instance.LoadScene("ResultScene");
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

    IEnumerator DesignatedWait(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        NoteSpawner noteSpawner = FindAnyObjectByType<NoteSpawner>();
        if (noteSpawner != null)
        {
            noteSpawner.StartSpawning(currentSong, offset, approachSpeed);
            Debug.Log("Started Note Spawner");
        }
    }
}   
