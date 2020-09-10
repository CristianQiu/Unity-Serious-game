using UnityEngine;

/// <summary>
/// Class containing additional debugging utilities
/// </summary>
public static class DebugUtils
{
    #region Methods

    /// <summary>
    /// Draw a debug arrow
    /// </summary>
    /// <param name="from"></param>
    /// <param name="v"></param>
    /// <param name="tipLengthFactor"></param>
    /// <param name="color"></param>
    public static void DrawArrow(Vector3 from, Vector3 v, float tipLengthFactor, Color color)
    {
        float magnitude = v.magnitude;

        if (magnitude <= Maths.GreaterEpsilon)
            return;

        // draw the main line
        Vector3 to = from + v;
        Debug.DrawLine(from, to, color);

        // now calc the left tip vector
        Vector3 leftTip = Quaternion.LookRotation(v) * (new Vector3(-magnitude, 0.0f, -magnitude) * tipLengthFactor);
        Debug.DrawLine(to, to + leftTip, color);

        // and the right tip vector
        Vector3 rightTip = Quaternion.LookRotation(v) * (new Vector3(magnitude, 0.0f, -magnitude) * tipLengthFactor);
        Debug.DrawLine(to, to + rightTip, color);
    }

    #endregion
}