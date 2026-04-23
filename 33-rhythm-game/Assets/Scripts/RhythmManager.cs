using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    // -------------------------------------------------------------------------
    // Timing Windows
    // -------------------------------------------------------------------------

    [Header("Timing Windows (seconds)")]
    [SerializeField] private float perfectWindow = 0.05f; // 50ms
    [SerializeField] private float goodWindow = 0.12f; // 120ms

    // Public getters so NoteVisual can read them without exposing the setters
    public float PerfectWindow => perfectWindow;
    public float GoodWindow => goodWindow;

    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // -------------------------------------------------------------------------
    // Judgement
    // -------------------------------------------------------------------------

    /// <summary>
    /// Evaluates a hit attempt by comparing the absolute delta between the
    /// note's target dspTime and the moment the player pressed the key.
    /// Returns the appropriate HitGrade based on the global timing windows.
    /// </summary>
    public HitGrade EvaluateHit(double targetTime, double hitTime)
    {
        double delta = System.Math.Abs(hitTime - targetTime);

        if (delta <= perfectWindow) return HitGrade.Perfect;
        if (delta <= goodWindow) return HitGrade.Good;

        return HitGrade.Miss;
    }
}

// Defined here so both LaneController and NoteVisual share the same type
// without any script needing to reference the other directly.
public enum HitGrade { Perfect, Good, Miss }
