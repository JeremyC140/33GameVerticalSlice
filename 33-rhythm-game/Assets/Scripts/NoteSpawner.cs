using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    private SongData currentSong;

    [Header("References")]
    public LaneController[] laneControllers;
    public NoteVisual notePrefab;

    [Header("Timing")]
    public float approachTime = 1.5f;
    private int _nextNoteIndex = 0;   // The pointer to the next note to spawn
    private double _songStartTime;
    private bool _isSongPlaying = false;

    public void StartSpawning(SongData song)
    {
        _nextNoteIndex = 0;
        _isSongPlaying = true;
        currentSong = song;
    }
    void Update()
    {
        if (!_isSongPlaying || currentSong == null) return;
        if (_nextNoteIndex < currentSong.chart.Count)
        {
            // 2. Get the data for the upcoming note
            NoteData nextNoteData = currentSong.chart[_nextNoteIndex];

            // 3. The Spawning Condition
            // We spawn the note early so it has time to "approach" the player
            if (AudioSettings.dspTime >= (nextNoteData.hitTime - approachTime))
            {
                SpawnNote(nextNoteData);
                _nextNoteIndex++;
            }
        }
    }
    void OnEnable()
    {
        // --- Event Subscription ---
        //GameController.OnPauseGame += HandlePause;
        GameController.OnRestartGame += ResetSpawner;
        //GameController.OnQuitGame += HandleQuit;
    }

    void OnDisable()
    {
        // --- Event Unsubscription ---
        //GameController.OnPauseGame -= HandlePause;
        GameController.OnRestartGame -= ResetSpawner;
        //GameController.OnQuitGame -= HandleQuit;
    }

    private void SpawnNote(NoteData note)
    {
        LaneController laneRef = laneControllers[note.laneIndex];

        NoteVisual newNote = Instantiate(notePrefab, laneRef.transform.position, Quaternion.identity, laneRef.transform);

        double targetHitTime = AudioSettings.dspTime + approachTime;

        newNote.InitializeNote(targetHitTime, approachTime);

        laneRef.AssignNote(newNote);
    }
    private void ResetSpawner()
    {
        _nextNoteIndex = 0;
        _isSongPlaying = false;
    }
}
