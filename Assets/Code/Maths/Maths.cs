using UnityEngine;

/// <summary>
/// Class containing all math-related utilities
/// </summary>
public static class Maths
{
    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes



    #endregion

    #region Properties

    public static float GreaterEpsilon { get { return 0.00001f; } }

    #endregion

    #region Damping Methods

    /// <summary>
    /// A smoothing frame rate independant function which internally uses Mathf.Lerp
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="smoothing"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static float LerpDamp(float a, float b, float smoothing, float dt)
    {
        return Mathf.Lerp(a, b, 1.0f - Mathf.Pow(smoothing, dt));
    }

    /// <summary>
    /// Same as LerpDamp but internally uses Mathf.LerpAngle to work fine when wrapping angles
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="smoothing"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static float LerpDampAngle(float a, float b, float smoothing, float dt)
    {
        return Mathf.LerpAngle(a, b, 1.0f - Mathf.Pow(smoothing, dt));
    }

    /// <summary>
    /// A smoothing frame rate independant function which internally uses Vector2.Lerp
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="smoothing"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector2 LerpDamp(Vector2 a, Vector2 b, float smoothing, float dt)
    {
        return Vector2.Lerp(a, b, 1.0f - Mathf.Pow(smoothing, dt));
    }

    /// <summary>
    /// A smoothing frame rate independant function which internally uses Vector3.Lerp
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="smoothing"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector3 LerpDamp(Vector3 a, Vector3 b, float smoothing, float dt)
    {
        return Vector3.Lerp(a, b, 1.0f - Mathf.Pow(smoothing, dt));
    }

    /// <summary>
    /// A smoothing frame rate independant function which internally uses Vector3.Slerp
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="smoothing"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector3 SlerpDamp(Vector3 a, Vector3 b, float smoothing, float dt)
    {
        return Vector3.Slerp(a, b, 1.0f - Mathf.Pow(smoothing, dt));
    }

    #endregion

    #region Angle Methods

    /// <summary>
    /// Normalize an angle given in degrees in the range [0.0f,360.0f)
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float NormalizeAnglePositive360Degs(float angle)
    {
        while (angle >= 360.0f)
            angle -= 360.0f;

        while (angle < 0.0f)
            angle += 360.0f;

        return angle;
    }

    #endregion

    #region Polar / Cartesian Coords Methods

    /// <summary>
    /// Convert cartesian coords to polar coords
    /// </summary>
    /// <param name="v"></param>
    /// <param name="r"></param>
    /// <param name="elevation"></param>
    /// <param name="azimuth"></param>
    /// <param name="getBackInDegs"></param>
    public static void CartesianToPolarCoords(Vector3 v, out float r, out float elevation, out float azimuth, bool getBackInDegs)
    {
        // get the radius
        r = v.magnitude;
        elevation = 0.0f;
        azimuth = 0.0f;

        // if there's no radius just keep elevation and azimuth zero
        if (v.sqrMagnitude <= GreaterEpsilon)
            return;

        // calculate elevation
        float y = v.y / r;
        elevation = Mathf.Asin(y);
        
        // and azimuth
        azimuth = Mathf.Atan2(v.z, v.x);

        if (getBackInDegs)
        {
            elevation *= Mathf.Rad2Deg;
            azimuth *= Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Convert polar coords to cartesian coords
    /// </summary>
    /// <param name="r"></param>
    /// <param name="elevation"></param>
    /// <param name="azimuth"></param>
    /// <param name="givenInDegs"></param>
    /// <returns></returns>
    public static Vector3 PolarToCartesian(float r, float elevation, float azimuth, bool givenInDegs)
    {
        if (givenInDegs)
        {
            elevation *= Mathf.Deg2Rad;
            azimuth *= Mathf.Deg2Rad;
        }

        float y = Mathf.Sin(elevation) * r;

        float h = Mathf.Cos(elevation) * r;

        float x = Mathf.Cos(azimuth) * h;
        float z = Mathf.Sin(azimuth) * h;

        Vector3 v = new Vector3(x, y, z);

        return v;
    }
	
    #endregion
}
