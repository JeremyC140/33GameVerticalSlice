using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner Instance { get; private set; }
    private SongData currentSong;

    [Header("References")]
    public LaneController[] laneControllers;
    public NoteVisual notePrefab;

    [Header("Timing")]
    private float approachSpeed;
    private int _nextNoteIndex = 0;   // The pointer to the next note to spawn
    public float currentSongRealTime = 0f;

    private List<NoteData> _tempChart = new List<NoteData>();
    private int numOfNotes = 50;
    private float songbpm;
    private float secondsPerFourthBeat;
    private float chartOffset = 0f;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    public void StartSpawning(SongData song, float offset, float speed)
    {
        _nextNoteIndex = 0;
        currentSong = song;
        songbpm = song.songBPM;
        secondsPerFourthBeat = 60f / songbpm * 4;
        chartOffset = offset;
        approachSpeed = 3.5f - speed;
        currentSongRealTime += -chartOffset;

        for (int i = 0; i < numOfNotes; i++)
        {
            // Calculate the target hit timing anchor for this specific beat cycle
            double beatTime = secondsPerFourthBeat * (i + 2);

            // --- NOTE 1: The Primary Base Note ---
            NoteData primaryNote = new NoteData();
            primaryNote.hitTime = beatTime;
            primaryNote.laneIndex = Random.Range(0, 7); // Generates 0 through 6
            primaryNote.type = NoteType.Tap;
            _tempChart.Add(primaryNote);

            // --- NOTE 2: The Conditional Double Note ---
            if (i % 5 == 0 && i != 0)
            {
                NoteData doubleNote = new NoteData();
                doubleNote.hitTime = beatTime; // Shares the exact same timing layout grid array
                doubleNote.type = NoteType.Tap;

                // Loop to find a lane index that does not match the primary note's lane
                int alternateLane;
                do
                {
                    alternateLane = Random.Range(0, 7);
                }
                while (alternateLane == primaryNote.laneIndex);

                doubleNote.laneIndex = alternateLane;
                _tempChart.Add(doubleNote);
            }
        }

        //_tempChart = new NoteData[numOfNotes];
        //for (int i = 0; i < numOfNotes; i++)
        //{
        //    NoteData newNote = new NoteData();
        //    newNote.hitTime = secondsPerFourthBeat * (i + 3);
        //    newNote.laneIndex = Random.Range(0, 7);
        //    newNote.type = NoteType.Tap;
        //    _tempChart[i] = newNote;
        //}
    }
    void Update()
    {
        if (!AudioManager.Instance._audioSource.isPlaying || currentSong == null) return;
        if (_nextNoteIndex >= _tempChart.Count) return;
        currentSongRealTime += Time.deltaTime;
        NoteData nextNoteData = _tempChart[_nextNoteIndex];
        if (currentSongRealTime >= nextNoteData.hitTime - approachSpeed)
        {
            Debug.Log($"Prepare to spawn the {_nextNoteIndex}th note");
            SpawnNote(_tempChart[_nextNoteIndex]);
            _nextNoteIndex++;
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

        newNote.InitializeNote(note.hitTime, approachSpeed);

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
