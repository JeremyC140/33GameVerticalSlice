using UnityEngine;

public class SimpleTestSpawner : MonoBehaviour
{
    [Header("References")]
    public LaneController spacebarLane;
    public NoteVisual notePrefab;

    [Header("Settings")]
    public float approachTime = 1.5f;

    void Update()
    {
        // Press 'Enter' to spawn a test note
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SpawnNote();
        }
    }

    private void SpawnNote()
    {
        if (spacebarLane == null || notePrefab == null)
        {
            Debug.LogWarning("Missing references in SimpleTestSpawner!");
            return;
        }

        // 1. Spawn the visual prefab as a child of the Lane Container
        NoteVisual newNote = Instantiate(notePrefab, spacebarLane.transform.position, Quaternion.identity, spacebarLane.transform);

        // 2. Calculate the exact future dspTime this note should hit 100% scale
        double targetHitTime = AudioSettings.dspTime + approachTime;

        // 3. Boot up the visual animation
        newNote.InitializeNote(targetHitTime, approachTime);

        // 4. Hand the note to the Lane Controller so it can listen for the Spacebar press
        spacebarLane.AssignNote(newNote);
    }
}
