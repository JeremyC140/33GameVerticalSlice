using UnityEngine;

public class LaneController : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Serialized Fields
    // -------------------------------------------------------------------------

    [Header("Input")]
    [SerializeField] private KeyCode targetKey;

    [Header("Active Note")]
    // Assign this at runtime from your NoteSpawner when a note enters this lane
    [SerializeField] private NoteVisual activeNote;

    // -------------------------------------------------------------------------
    // Public API — NoteSpawner uses this to hand a note to the lane
    // -------------------------------------------------------------------------

    public void AssignNote(NoteVisual note)
    {
        activeNote = note;
    }

    // -------------------------------------------------------------------------
    // Input Handling
    // -------------------------------------------------------------------------

    private void Update()
    {
        if (!Input.GetKeyDown(targetKey)) return;
        if (activeNote == null || !activeNote.IsActive) return;

        HitGrade grade = RhythmManager.Instance.EvaluateHit(
            activeNote.TargetDspTime,
            NoteSpawner.Instance.currentSongRealTime
        );

        activeNote.Judge(grade);

        activeNote = null;
    }
}
