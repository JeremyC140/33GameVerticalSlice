using UnityEngine;
using Unity.VisualScripting;

public class NoteController : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Serialized Fields
    // -------------------------------------------------------------------------

    [Header("Input")]
    [SerializeField] private KeyCode targetKey;

    [Header("Timing Windows (seconds)")]
    [SerializeField] private float perfectWindow = 0.05f;
    [SerializeField] private float goodWindow = 0.12f;

    [Header("Visuals")]
    [SerializeField] private Transform visualNote;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color hitColor = Color.cyan;
    [SerializeField] private Color missColor = Color.red;

    // -------------------------------------------------------------------------
    // Private State
    // -------------------------------------------------------------------------

    private double _targetDspTime;  // The exact dspTime beat this note lives on
    private double _approachStart;  // The dspTime when the note began approaching
    private double _approachEnd;    // Cached: _targetDspTime

    private bool _isActive;         // True while a note is live and awaiting input
    private bool _hasBeenJudged;    // Guards against double-judging a single note

    private SpriteRenderer _spriteRenderer;

    // Visual pulse state
    private bool _isPulsing;
    private float _pulseTimer;
    private const float PulseDuration = 0.08f;
    private const float PulseScalePeak = 1.25f;

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
        if (!_isActive) return;

        double now = AudioSettings.dspTime;

        HandleAutoMiss(now);
        HandleScaleApproach(now);
        HandleInput(now);
        HandlePulse();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Arms this controller for an incoming note.
    /// Call this from NoteSpawner whenever a note
    /// is scheduled for this lane.
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
        _targetDspTime = targetDspTime;
        _approachStart = targetDspTime - approachTime; // when growth begins
        _approachEnd = targetDspTime;                // when growth completes (scale == 1)

        _isActive = true;
        _hasBeenJudged = false;

        if (_spriteRenderer != null)
            _spriteRenderer.color = idleColor;

        // Start invisible; HandleScaleApproach will take over from here
        visualNote.localScale = Vector3.zero;
    }

    // -------------------------------------------------------------------------
    // Core Logic
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

        float t = Mathf.Clamp01((float)(elapsed / windowDuration));
        float scale = Mathf.Lerp(0.1f, 1.0f, t);

        visualNote.localScale = new Vector3(scale, scale, 1f);
    }

    private void HandleInput(double now)
    {
        if (_hasBeenJudged || !Input.GetKeyDown(targetKey)) return;

        double delta = System.Math.Abs(now - _targetDspTime);

        if (delta <= perfectWindow)
            Judge(HitGrade.Perfect);
        else if (delta <= goodWindow)
            Judge(HitGrade.Good);
        else
            Judge(HitGrade.Miss);
    }

    /// <summary>
    /// Auto-miss: if the beat has passed beyond the good window and the
    /// player never pressed the key, clean up and count it as a miss.
    /// </summary>
    private void HandleAutoMiss(double now)
    {
        if (_hasBeenJudged) return;
        if (now > _targetDspTime + goodWindow)
            Judge(HitGrade.Miss);
    }

    // -------------------------------------------------------------------------
    // Judgement & Feedback
    // -------------------------------------------------------------------------

    private enum HitGrade { Perfect, Good, Miss }

    private void Judge(HitGrade grade)
    {
        _hasBeenJudged = true;
        _isActive = false;

        switch (grade)
        {
            case HitGrade.Perfect:
                Debug.Log($"[{gameObject.name}] PERFECT!");
                ApplyColor(hitColor);
                TriggerPulse();
                break;

            case HitGrade.Good:
                Debug.Log($"[{gameObject.name}] Good");
                ApplyColor(hitColor);
                TriggerPulse();
                break;

            case HitGrade.Miss:
                Debug.Log($"[{gameObject.name}] Miss");
                ApplyColor(missColor);
                ResetVisuals();
                break;
        }
    }

    private void TriggerPulse()
    {
        _isPulsing = true;
        _pulseTimer = 0f;
        visualNote.localScale = new Vector3(PulseScalePeak, PulseScalePeak, 1f);
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

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void ApplyColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    private void ResetVisuals()
    {
        if (visualNote != null)
            visualNote.localScale = Vector3.zero;

        ApplyColor(idleColor);
    }
}