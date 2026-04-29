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
    //[SerializeField] private Color idleColor = Color.cyan;
    //[SerializeField] private Color hitColor = Color.green;
    //[SerializeField] private Color missColor = Color.red;

    [Header("Depth Tuning")]
    [Range(1f, 5f)]
    [SerializeField] private float scaleEasePower = 2.5f; // 1 is linear, 2+ is exponential

    // -------------------------------------------------------------------------
    // Public State
    // -------------------------------------------------------------------------

    // LaneController reads TargetDspTime to pass into RhythmManager.EvaluateHit
    public double TargetDspTime { get; private set; }

    // LaneController checks IsActive before accepting input for this note
    public bool IsActive { get; private set; }

    // -------------------------------------------------------------------------
    // Private Timing State
    // -------------------------------------------------------------------------

    private double _approachStart; // dspTime when growth begins
    private double _approachEnd;   // dspTime when growth completes (== TargetDspTime)

    private bool _hasBeenJudged;

    // -------------------------------------------------------------------------
    // Private Visual State
    // -------------------------------------------------------------------------

    private SpriteRenderer _spriteRenderer;
    private bool _isPulsing;
    private float _pulseTimer;
    private const float PulseDuration = 0.08f;
    private const float PulseScalePeak = 1.25f;

    //public TextMeshProUGUI displayText;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (visualNote != null)
            _spriteRenderer = visualNote.GetComponent<SpriteRenderer>();

        ResetVisuals();
    }

    private void Update()
    {
        if (!IsActive) return;

        double now = NoteSpawner.Instance.currentSongRealTime;
        //Debug.Log("Current Note Visual Real Time: " + now);

        HandleAutoMiss(now);
        HandleScaleApproach(now);
        HandlePulse();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// </summary>
    /// <param name="targetDspTime">
    ///     The exact dspTime at which the note should be hit.
    /// </param>
    /// <param name="approachTime">
    ///     How many seconds before targetDspTime the note starts growing.
    ///     E.g. 2f means the star begins scaling up 2 seconds early.
    /// </param>
    public void InitializeNote(double targetDspTime, float approachTime)
    {
        TargetDspTime = targetDspTime;
        _approachStart = targetDspTime - approachTime; // when growth begins
        _approachEnd = targetDspTime;                  // when growth completes (scale == 1)
        Debug.Log("Initializing NoteVisual with TargetDspTime: " + TargetDspTime + " and ApproachStart: " + _approachStart);

        IsActive = true;
        _hasBeenJudged = false;

        //ApplyColor(idleColor);

        // Start invisible; HandleScaleApproach will take over from here
        visualNote.localScale = Vector3.zero;
    }

    // -------------------------------------------------------------------------
    // Core Visual Logic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Grows the star from 0.1 → 1.0 as dspTime travels from _approachStart
    /// to _targetDspTime.
    /// </summary>
    private void HandleScaleApproach(double now)
    {
        if (_hasBeenJudged || _isPulsing) return;

        // Cast to float only for the Lerp — all timing comparisons stay double
        double windowDuration = _approachEnd - _approachStart;
        double elapsed = now - _approachStart;

        // 1. Calculate linear t (0 to 1)
        float tLinear = Mathf.Clamp01((float)(elapsed / windowDuration));

        // 2. Apply the exponential curve
        // Using Mathf.Pow makes the growth start slow and accelerate at the end
        float tExponential = Mathf.Pow(tLinear, scaleEasePower);

        // 3. Lerp using the curved 't'
        float scale = Mathf.Lerp(0.1f, 1.0f, tExponential);

        visualNote.localScale = new Vector3(scale, scale, 1f);
    }

    /// <summary>
    /// Auto-miss: if the beat has passed beyond the good window and the
    /// player never pressed the key, clean up and count it as a miss.
    /// </summary>
    private void HandleAutoMiss(double now)
    {
        if (_hasBeenJudged) return;
        if (now > TargetDspTime + RhythmManager.Instance.GoodWindow)
            Judge(HitGrade.Miss);
    }

    /// <summary>
    /// Decays the pulse scale back to zero after PulseDuration seconds.
    /// Kept out of HandleScaleApproach so the two never fight over localScale.
    /// </summary>
    private void HandlePulse()
    {
        if (!_isPulsing) return;

        _pulseTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_pulseTimer / PulseDuration);
        float scale = Mathf.Lerp(PulseScalePeak, 0f, t);

        visualNote.localScale = new Vector3(scale, scale, 1f);

        if (t >= 1f)
        {
            _isPulsing = false;
            visualNote.localScale = Vector3.zero;
        }
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
                EventBus.Trigger("PerfectHit");
                //displayText.text = "RESULT: PERFECT!";
                //ApplyColor(hitColor);
                //TriggerPulse();
                break;

            case HitGrade.Good:
                Debug.Log($"[{gameObject.name}] Good");
                EventBus.Trigger("GoodHit");
                //displayText.text = "RESULT: GOOD!";
                //ApplyColor(hitColor);
                //TriggerPulse();
                break;

            case HitGrade.Miss:
                Debug.Log($"[{gameObject.name}] Miss");
                EventBus.Trigger("MissHit");
                //displayText.text = "RESULT: MISS!";
                //ApplyColor(missColor);
                ResetVisuals();
                break;
        }
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void TriggerPulse()
    {
        _isPulsing = true;
        _pulseTimer = 0f;
        visualNote.localScale = new Vector3(PulseScalePeak, PulseScalePeak, 1f);
    }

    private void ApplyColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    private void ResetVisuals()
    {
        if (visualNote != null)
            visualNote.localScale = Vector3.zero;

        //ApplyColor(idleColor);
    }
}
