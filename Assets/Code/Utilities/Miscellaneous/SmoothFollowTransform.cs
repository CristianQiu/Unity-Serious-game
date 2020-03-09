using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class makes possible for a transform to follow smoothly another transform
/// </summary>
public class SmoothFollowTransform : MonoBehaviour
{
    #region Public Attributes

    public Transform target;

    [Range(0.0f, 0.25f)]
    public float smoothingXZ = 0.04f;
    [Range(0.0f, 1.0f)]
    public float smoothingY = 0.1f;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private float xSpeed = 0.0f;
    private float ySpeed = 0.0f;
    private float zSpeed = 0.0f;
	
    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        Init();
    }
	
    // Update is called once per frame
    void Update () 
    {
        float dt = Time.deltaTime;

        UpdatePosition(dt);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        SetTarget(target);
    }

    /// <summary>
    /// Update the position
    /// </summary>
    /// <param name="dt"></param>
    private void UpdatePosition(float dt)
    {
        if (target == null)
            return;

        // we are having different values of smoothing for the Y and the XZ

        // first get the current ones
        float currX = transform.position.x;
        float currY = transform.position.y;
        float currZ = transform.position.z;

        // now the target ones
        float targetX = target.position.x;
        float targetY = target.position.y;
        float targetZ = target.position.z;

        // apply smoothing
        float x = Mathf.SmoothDamp(currX, targetX, ref xSpeed, smoothingXZ, Mathf.Infinity, dt);
        float y = Mathf.SmoothDamp(currY, targetY, ref ySpeed, smoothingY, Mathf.Infinity, dt);
        float z = Mathf.SmoothDamp(currZ, targetZ, ref zSpeed, smoothingXZ, Mathf.Infinity, dt);

        // update the position
        transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// Sets a new target
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget)
    {
        if (newTarget == target)
            return;

        target = newTarget;
    }

    #endregion
}
