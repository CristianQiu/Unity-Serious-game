using UnityEngine;

/// <summary>
/// Different functions to interpolate
/// </summary>
public enum InterpolationFunction
{
    Invalid = -1,

    Linear,
    Exponential,
    ExponentialSquared,
    Cubic,
    Cubic2,
    EaseInOutCubic,
    SuperSmooth,
    //...

    Count
}

/// <summary>
/// Static class to map interpolation t parameter belonging to [0..1] according to mathematical functions to smooth in, smooth out, etc...
/// </summary>
public static class CustomInterpolation
{
    public static float Interpolate(float t, InterpolationFunction function)
    {
        float s = 0.0f;

        // choose the right function and map t
        switch (function)
        {
            case InterpolationFunction.Linear:
                s = t; 
                break;
            case InterpolationFunction.Exponential:
                s = t * t;
                break;
            case InterpolationFunction.ExponentialSquared:
                s = Mathf.Pow((t * t), 2);
                break;
            case InterpolationFunction.Cubic:
                s = t * t * t;
                break;
            case InterpolationFunction.Cubic2:
                s = ((t - 3.0f) * t + 3.0f) * t;
                break;
            case InterpolationFunction.EaseInOutCubic:
                s = t < 0.5f ? 4.0f * t * t * t : (t - 1.0f) * (2.0f * t - 2.0f) * (2.0f * t - 2.0f) + 1.0f;
                break;
            case InterpolationFunction.SuperSmooth:
                s = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
                break;
            default:
                Debug.Log("Invalid InterpolationFunction : " +function);
                break;
        }

        return s;
    }
}
