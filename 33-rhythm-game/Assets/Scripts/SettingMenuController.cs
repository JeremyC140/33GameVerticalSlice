using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class SettingMenuController : MonoBehaviour
{
    public Slider offsetSlider;
    public TextMeshProUGUI offsetDisplayQuantityText;

    public Slider speedSlider;
    public TextMeshProUGUI speedDisplayQuantityText;

    void Start()
    {
        if (ValueKeeper.Instance != null && offsetSlider != null && speedSlider != null)
        {
            offsetSlider.value = ValueKeeper.Instance.offset * 1000f;
            UpdateOffsetTextLabel(offsetSlider.value);

            speedSlider.value = ValueKeeper.Instance.speed;
            UpdateSpeedTextLabel(speedSlider.value);
        }

        offsetSlider.onValueChanged.AddListener(HandleOffsetSliderChanged);
        speedSlider.onValueChanged.AddListener(HandleSpeedSliderChanged);
    }

    private void HandleOffsetSliderChanged(float value)
    {
        if (ValueKeeper.Instance != null)
        {
            ValueKeeper.Instance.offset = value / 1000f;
            UpdateOffsetTextLabel(value);
        }
    }

    private void UpdateOffsetTextLabel(float value)
    {
        if (offsetDisplayQuantityText != null)
        {
            // Converts seconds (0.05) into readable milliseconds (+50 ms) for players
            int ms = Mathf.RoundToInt(value * 1000f);
            string sign = ms >= 0 ? "+" : "";
            offsetDisplayQuantityText.text = $"{sign}{ms} ms";
            //Debug.Log($"Offset updated: {sign}{ms} ms");
        }
    }

    private void HandleSpeedSliderChanged(float value)
    {
        if (ValueKeeper.Instance != null)
        {
            ValueKeeper.Instance.speed = value;
            UpdateSpeedTextLabel(value);
        }
    }

    private void UpdateSpeedTextLabel(float value)
    {
        if (speedDisplayQuantityText != null)
        {
            speedDisplayQuantityText.text = value.ToString("F2");
        }
    }
}
