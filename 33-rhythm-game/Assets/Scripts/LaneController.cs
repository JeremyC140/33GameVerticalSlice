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

        // dspTime is read here at the exact moment of input so the delta
        // calculated inside EvaluateHit is as tight to the keypress as possible
        HitGrade grade = RhythmManager.Instance.EvaluateHit(
            activeNote.TargetDspTime,
            AudioSettings.dspTime
        );

        activeNote.Judge(grade);

        // Clear the reference immediately — prevents any double-hit on the
        // same note even if input somehow fires twice in one frame
        activeNote = null;
    }
}
