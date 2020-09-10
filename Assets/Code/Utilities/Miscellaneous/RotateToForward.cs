using UnityEngine;

/// <summary>
/// Simple script to rotate towards -Vector3.forward
/// </summary>
public class RotateToForward : MonoBehaviour
{
    private void Update()
    {
        transform.forward = -Vector3.forward;
    }
}