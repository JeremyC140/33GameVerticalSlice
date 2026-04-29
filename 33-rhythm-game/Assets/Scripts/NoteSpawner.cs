using Newtonsoft.Json.Bson;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner Instance { get; private set; }
    private SongData currentSong;

    [Header("References")]
    public LaneController[] laneControllers;
    public NoteVisual notePrefab;

    [Header("Timing")]
    public float approachTime = 1.5f;
    private int _nextNoteIndex = 0;   // The pointer to the next note to spawn
    public float currentSongRealTime = 0f;

    private NoteData[] _tempChart;
    private float songbpm;
    private float secondsPerEightBeat;
    private int currentBeat;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void StartSpawning(SongData song)
    {
        _nextNoteIndex = 0;
        currentSong = song;
        songbpm = song.songBPM;
        secondsPerEightBeat = 60f / songbpm * 8;
        currentBeat = 0;
        currentSongRealTime = 0f;

        _tempChart = new NoteData[20];

        for (int i = 0; i < 20; i++)
        {
            NoteData newNote = new NoteData();
            newNote.hitTime = secondsPerEightBeat * i;
            newNote.laneIndex = Random.Range(0, 7);
            newNote.type = NoteType.Tap;
            _tempChart[i] = newNote;
        }
    }
    void Update()
    {
        if (!AudioManager.Instance.isSongPlaying || currentSong == null) return;
        currentSongRealTime += Time.deltaTime;
        //Debug.Log("Current Song Real Time: " + currentSongRealTime);
        //if (_nextNoteIndex < currentSong.chart.Count)
        //{
        //    NoteData nextNoteData = currentSong.chart[_nextNoteIndex];

        //    //if (currentSongRealTime >= (nextNoteData.hitTime - approachTime))
        //    //{
        //    //    SpawnNote(nextNoteData);
        //    //    _nextNoteIndex++;
        //    //}
        //}
        if (currentSongRealTime >= secondsPerEightBeat * currentBeat - approachTime && currentBeat < 20)
        {
            Debug.Log($"Prepare to spawn the {currentBeat}th eight beat ");
            SpawnNote(_tempChart[currentBeat]);
            currentBeat++;
        }
    }
        
    void OnEnable()
    {
        // --- Event Subscription ---
        GameController.OnPauseGame += TogglePauseSpawner;
        GameController.OnRestartGame += ResetSpawner;
        //GameController.OnQuitGame += HandleQuit;
    }

    void OnDisable()
    {
        // --- Event Unsubscription ---
        GameController.OnPauseGame -= TogglePauseSpawner;
        GameController.OnRestartGame -= ResetSpawner;
        //GameController.OnQuitGame -= HandleQuit;
    }

    private void SpawnNote(NoteData note)
    {
        LaneController laneRef = laneControllers[note.laneIndex];

        NoteVisual newNote = Instantiate(notePrefab, laneRef.transform.position, Quaternion.identity, laneRef.transform);

        newNote.InitializeNote(note.hitTime, approachTime);

        laneRef.AssignNote(newNote);
    }

    private void TogglePauseSpawner()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
    private void ResetSpawner()
    {
        _nextNoteIndex = 0;
    }
}
