using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting; 

public class NoteVisual : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Serialized Fields
    // -------------------------------------------------------------------------

    [Header("Visuals")]
    [SerializeField] private Transform visualNote;
    [SerializeField] private float startingScale = 0.15f;

    [Header("Rotation Settings")]
    public float startAngle = 0f;
    [Tooltip("Total degrees the star will spin during the approach. e.g., 360 for one full spin.")]
    public float totalRotationDegrees = 360f;
    [Range(0.1f, 5f)]
    [SerializeField] private float rotationEasePower = 2.5f; // 1 is linear, 2+ is exponential


    //[Header("Scale Curve Settings (Logarithmic)")]
    //[Tooltip("Controls how fast the note 'pops' at the beginning. Higher = faster start, slower end.")]
    //[Range(1f, 50f)]
    //public float logSteepness = 15f;

    [Header("Approach Scale Curve (Exponential)")]
    [Tooltip("<1 is root functions, 1 is linear, 2+ is exponential")]
    [Range(0.1f, 5f)]
    [SerializeField] private float scaleEasePower = 2.5f; // 1 is linear, 2+ is exponential

    // -------------------------------------------------------------------------
    // Public State
    // -------------------------------------------------------------------------

    // LaneController reads TargetRealTime to pass into RhythmManager.EvaluateHit
    public double TargetRealTime { get; private set; }

    // LaneController checks IsActive before accepting input for this note
    public bool IsActive { get; private set; }

    // -------------------------------------------------------------------------
    // Private Timing State
    // -------------------------------------------------------------------------

    private double _approachStart; // dspTime when growth begins
    private double _approachEnd;   // dspTime when growth completes (== TargetRealTime)

    private bool _hasBeenJudged;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        if (visualNote != null)
            _spriteRenderer = visualNote.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsActive) return;

        double now = NoteSpawner.Instance.currentSongRealTime;

        HandleAutoMiss(now);
        HandleScaleApproach(now);
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// </summary>
    /// <param name="TargetRealTime">
    ///     The exact dspTime at which the note should be hit.
    /// </param>
    /// <param name="approachTime">
    ///     How many seconds before TargetRealTime the note starts growing.
    ///     E.g. 2f means the star begins scaling up 2 seconds early.
    /// </param>
    public void InitializeNote(double targetRealTime, float approachTime)
    {
        TargetRealTime = targetRealTime;
        _approachStart = targetRealTime - approachTime; // when growth begins
        _approachEnd = targetRealTime;                  // when growth completes (scale == 1)
        Debug.Log("Initializing NoteVisual with TargetRealTime: " + targetRealTime + " and ApproachStart: " + _approachStart);

        IsActive = true;
        _hasBeenJudged = false;

        visualNote.localScale = startingScale * Vector3.one;
    }

    /// <summary>
    /// Grows the star from 0.1 → 1.0 as dspTime travels from _approachStart
    /// to _TargetRealTime.
    /// </summary>
    private void HandleScaleApproach(double now)
    {
        if (_hasBeenJudged) return;

        // Cast to float only for the Lerp — all timing comparisons stay double
        double windowDuration = _approachEnd - _approachStart;
        double elapsed = now - _approachStart;

        // 1.
        // Calculate linear t (0 to 1)
        float tLinear = Mathf.Clamp01((float)(elapsed / windowDuration));

        // 2.
        // Attempt: Apply the exponential curve
        // Using Mathf.Pow makes the growth start slow and accelerate at the end
        float tExponentialScale = Mathf.Pow(tLinear, scaleEasePower);


        // 3.
        // Lerp using the curved 't'
        float scale = Mathf.Lerp(startingScale, 1.0f, tExponentialScale);
        visualNote.localScale = new Vector3(scale, scale, 1f);

        // 4.
        // Handle Rotation
        // Mapped to tLinear so the spin speed remains constant even as the scaling slows down.
        float tExponentialRotation = Mathf.Pow(tLinear, rotationEasePower);
        float currentAngle = Mathf.Lerp(startAngle, startAngle + totalRotationDegrees, tExponentialRotation);
        visualNote.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    /// <summary>
    /// Auto-miss: if the beat has passed beyond the good window and the
    /// player never pressed the key, clean up and count it as a miss.
    /// </summary>
    private void HandleAutoMiss(double now)
    {
        if (_hasBeenJudged) return;
        if (now > TargetRealTime + RhythmManager.Instance.GoodWindow)
            Judge(HitGrade.Miss);
    }

    /// <summary>
    /// Called by LaneController with the grade resolved by RhythmManager.
    /// Also called internally by HandleAutoMiss on a timed-out note.
    /// </summary>
    public void Judge(HitGrade grade)
    {
        if (_hasBeenJudged) return;

        _hasBeenJudged = true;
        IsActive = false;
        Destroy(gameObject);

        switch (grade)
        {
            case HitGrade.Perfect:
                Debug.Log($"[{gameObject.name}] PERFECT!");
                GameController.Instance.triggerPerfectHit();
                VFXManager.Instance.TriggerPerfectFlash();
                break;

            case HitGrade.Good:
                Debug.Log($"[{gameObject.name}] Good");
                GameController.Instance.triggerGoodHit();
                break;

            case HitGrade.Miss:
                Debug.Log($"[{gameObject.name}] Miss");
                GameController.Instance.triggerMissHit();
                break;
        }
    }
}
