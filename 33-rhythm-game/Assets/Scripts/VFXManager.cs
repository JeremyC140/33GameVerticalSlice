using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP post-processing

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public Volume globalVolume;

    [Header("Perfect Hit Settings")]
    public float flashExposure = 1.5f;
    public float flashChromatic = 1.0f;
    public float recoverySpeed = 10f;

    private ChromaticAberration _chromatic;
    private ColorAdjustments _colorAdjustments;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(this);
    }

    private void Start()
    {
        // Reach into the Volume Profile and extract the specific overrides to tweak
        if (globalVolume.profile.TryGet(out _chromatic))
        {
            _chromatic.intensity.value = 0f;
        }

        if (globalVolume.profile.TryGet(out _colorAdjustments))
        {
            _colorAdjustments.postExposure.value = 0f;
        }
    }

    private void Update()
    {
        // Smoothly return both values back to 0 every frame
        if (_chromatic != null && _chromatic.intensity.value > 0.01f)
        {
            _chromatic.intensity.value = Mathf.Lerp(_chromatic.intensity.value, 0f, Time.deltaTime * recoverySpeed);
        }

        if (_colorAdjustments != null && _colorAdjustments.postExposure.value > 0.01f)
        {
            _colorAdjustments.postExposure.value = Mathf.Lerp(_colorAdjustments.postExposure.value, 0f, Time.deltaTime * recoverySpeed);
        }
    }

    // Call this from your LaneController when a Perfect hit occurs!
    public void TriggerPerfectFlash()
    {
        if (_chromatic != null) _chromatic.intensity.value = flashChromatic;
        if (_colorAdjustments != null) _colorAdjustments.postExposure.value = flashExposure;
    }
}