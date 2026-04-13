using UnityEngine;

[ExecuteInEditMode]
public class PatternGenerator : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Serialized Fields
    // -------------------------------------------------------------------------

    [Header("Star References")]
    [SerializeField] private Transform centerStar;
    [SerializeField] private Transform[] leftColumnStars;
    [SerializeField] private Transform[] rightColumnStars;

    [Header("Layout Tuning")]
    [SerializeField] private float horizontalSeparation = 3f;
    [SerializeField] private float verticalSpacing = 2f;
    [SerializeField] private float columnTiltOffset = 0.5f;

    private void OnValidate() => ApplyLayout();
    private void Awake() => ApplyLayout();

    private void ApplyLayout()
    {
        if (!ValidateReferences()) return;

        centerStar.localPosition = Vector3.zero;

        PositionColumn(leftColumnStars, isLeft: true);
        PositionColumn(rightColumnStars, isLeft: false);
    }

    private void PositionColumn(Transform[] column, bool isLeft)
    {
        int middle = column.Length / 2; // Index 1 for a 3-star column

        for (int i = 0; i < column.Length; i++)
        {
            if (column[i] == null)
            {
                Debug.LogWarning($"PatternGenerator: Null entry at index {i} in {(isLeft ? "left" : "right")} column.");
                continue;
            }

            // Y: top is positive, middle is zero, bottom is negative
            float y = -(middle - i) * verticalSpacing;

            // Tilt offset increases per index (top has least inward offset, bottom has most)
            float tilt = i * columnTiltOffset;

            // Left column: starts left, tilt pushes further inward down the column
            // Right column: starts right, tilt pushes further inward down the column
            float x = isLeft
                ? -horizontalSeparation - tilt
                : horizontalSeparation + tilt;

            column[i].localPosition = new Vector3(x, y, 0f);
        }
    }

    private bool ValidateReferences()
    {
        if (centerStar == null)
        {
            Debug.LogWarning("PatternGenerator: centerStar is not assigned.");
            return false;
        }

        if (!ValidateColumn(leftColumnStars, "leftColumnStars")) return false;
        if (!ValidateColumn(rightColumnStars, "rightColumnStars")) return false;

        return true;
    }

    private static bool ValidateColumn(Transform[] column, string fieldName)
    {
        if (column == null || column.Length == 0)
        {
            Debug.LogWarning($"PatternGenerator: {fieldName} is empty.");
            return false;
        }

        if (column.Length != 3)
        {
            Debug.LogWarning($"PatternGenerator: {fieldName} must contain exactly 3 entries (found {column.Length}).");
            return false;
        }

        return true;
    }
}
