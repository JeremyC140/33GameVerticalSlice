using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class LaneController : MonoBehaviour
{
    [Header("Input")]
        [SerializeField] private KeyCode targetKey;

    [Header("Lane Visual Feedback")]
        [SerializeField] private Transform laneVisual;

        [Tooltip("How much to shrink the star when pressed (e.g., 0.85 = 85% size)")]
        public float pressedScaleMultiplier = 0.85f;

        [Tooltip("The color tint when the lane is held down")]
        public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        [Tooltip("How fast the star squishes and springs back")]
        public float visualSpringSpeed = 20f;

        public TextMeshPro keyTextLabel;

    private NoteVisual activeNote; // Assigned from NoteSpawner when a note enters this lane
    private Vector3 _originalScale;
    private Color _originalColor;
    private SpriteRenderer _laneRenderer;

    private void Start()
    {
        if (laneVisual != null)
        {
            _originalScale = laneVisual.localScale;
            _laneRenderer = laneVisual.GetComponent<SpriteRenderer>();

            if (_laneRenderer != null)
                _originalColor = _laneRenderer.color;
        }
    }

    // -------------------------------------------------------------------------
    // Public API — NoteSpawner uses this to hand a note to the lane
    // -------------------------------------------------------------------------

    public void AssignNote(NoteVisual note)
    {
        activeNote = note;
    }
    public void UpdateKeybind(KeyCode newKey)
    {
        targetKey = newKey;
        if (keyTextLabel != null)
        {
            keyTextLabel.text = targetKey.ToString();
        }
    }

    // -------------------------------------------------------------------------
    // Input Handling
    // -------------------------------------------------------------------------

    private void Update()
    {
        UpdateKeybind(targetKey);
        HandleLaneVisual();
        if (!Input.GetKeyDown(targetKey)) return;
        if (activeNote == null || !activeNote.IsActive) return;

        HitGrade grade = RhythmManager.Instance.EvaluateHit(
            activeNote.TargetRealTime,
            NoteSpawner.Instance.currentSongRealTime
        );
        // Avoid judging a note if it's too early (grade == None)
        if (grade != HitGrade.None)
        {
            activeNote.Judge(grade);
            activeNote = null;
        }
        else
        {
            // AudioManager.Instance.PlayHitSound();
            return;
        }
    }

    private void HandleLaneVisual()
    {
        if (Input.GetKey(targetKey))
        {
            // LERP towards the PRESSED state
            Vector3 targetScale = _originalScale * pressedScaleMultiplier;

            laneVisual.localScale = Vector3.Lerp(
                laneVisual.localScale, targetScale,
                Time.deltaTime * visualSpringSpeed);
            _laneRenderer.color = Color.Lerp(
                _laneRenderer.color, pressedColor, 
                Time.deltaTime * visualSpringSpeed);
        }
        else
        {
            // LERP back to the NORMAL state
            laneVisual.localScale = Vector3.Lerp(
                laneVisual.localScale, _originalScale, 
                Time.deltaTime * visualSpringSpeed);
            _laneRenderer.color = Color.Lerp(
                _laneRenderer.color, _originalColor, 
                Time.deltaTime * visualSpringSpeed);
        }
    }
}
