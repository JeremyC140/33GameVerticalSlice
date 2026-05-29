using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class SettingMenuController : MonoBehaviour
{
    public Slider offsetSlider;
    public TextMeshProUGUI offsetDisplayQuantityText;

    void Start()
    {
        if (ValueKeeper.Instance != null && offsetSlider != null)
        {
            offsetSlider.value = ValueKeeper.Instance.offset;
            UpdateTextLabel(offsetSlider.value);
        }

        offsetSlider.onValueChanged.AddListener(HandleOffsetSliderChanged);
    }

    private void HandleOffsetSliderChanged(float value)
    {
        if (ValueKeeper.Instance != null)
        {
            ValueKeeper.Instance.offset = value;
            UpdateTextLabel(value);
        }
    }

    private void UpdateTextLabel(float value)
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
}
