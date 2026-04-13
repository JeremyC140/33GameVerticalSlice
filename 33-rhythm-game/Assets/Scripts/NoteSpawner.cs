using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Lane References")]
    [Tooltip("Drag the 7 Star Containers here.")]
    public NoteController[] noteLanes;

    [Header("Test Configuration")]
    [Tooltip("How many seconds the note takes to reach 100% scale.")]
    public float approachTime = 1.5f;

    [Tooltip("Time in seconds between each random note spawn.")]
    public float spawnInterval = 0.5f;

    private double nextSpawnTime;

    void Start()
    {
        // Set the timer for the very first note
        nextSpawnTime = AudioSettings.dspTime + 1.0; // Start 1 second after playing
    }

    void Update()
    {
        // 1. Automatic Random Spawning
        // We use dspTime for precise triggering instead of Time.deltaTime
        if (AudioSettings.dspTime >= nextSpawnTime)
        {
            SpawnRandomNote();

            // Schedule the next spawn exactly 'spawnInterval' seconds from the current target
            nextSpawnTime += spawnInterval;
        }

        // 2. Manual Testing (Press 'M' to force a spawn immediately)
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnRandomNote();
        }
    }

    // You can right-click the script in the Inspector to trigger this manually too!
    [ContextMenu("Spawn Random Note Now")]
    private void SpawnRandomNote()
    {
        if (noteLanes == null || noteLanes.Length == 0)
        {
            Debug.LogWarning("NoteSpawner: No NoteControllers assigned in the Inspector!");
            return;
        }

        // Pick a random lane from 0 to 6
        int randomIndex = Random.Range(0, noteLanes.Length);
        NoteController selectedLane = noteLanes[randomIndex];

        // The target hit time is EXACTLY the current audio time + the approach duration
        double targetHitTime = AudioSettings.dspTime + approachTime;

        // Tell the selected lane to start its approach animation
        selectedLane.InitializeNote(targetHitTime, approachTime);
    }
}
