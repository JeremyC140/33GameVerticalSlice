using UnityEngine;

public class SimpleTestSpawner : MonoBehaviour
{
    [Header("References")]
    public LaneController[] laneControllers;
    public NoteVisual notePrefab;

    [Header("Settings")]
    public float approachTime = 1.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnNote(laneControllers[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnNote(laneControllers[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnNote(laneControllers[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnNote(laneControllers[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpawnNote(laneControllers[4]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SpawnNote(laneControllers[5]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SpawnNote(laneControllers[6]);
        }
    }

    private void SpawnNote(LaneController laneRef)
    {
        if (laneRef == null || notePrefab == null)
        {
            Debug.LogWarning("Missing references in SimpleTestSpawner!");
            return;
        }

        // 1. Spawn the visual prefab as a child of the Lane Container
        NoteVisual newNote = Instantiate(notePrefab, laneRef.transform.position, Quaternion.identity, laneRef.transform);

        // 2. Calculate the exact future dspTime this note should hit 100% scale
        double targetHitTime = AudioSettings.dspTime + approachTime;

        // 3. Boot up the visual animation
        newNote.InitializeNote(targetHitTime, approachTime);

        // 4. Hand the note to the Lane Controller so it can listen for the Spacebar press
        laneRef.AssignNote(newNote);
    }
}
