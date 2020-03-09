using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A floating behaviour noise / sin like
/// </summary>
public class Float : MonoBehaviour
{
    #region Public Attributes

    public Vector3 floatFrequencies = Vector3.zero;
    public Vector3 floatAmplitudes = Vector3.zero;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private float timer = 0.0f;

    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        RandomizeTimer();
    }
	
    // Update is called once per frame
    void Update () 
    {
        float dt = Time.deltaTime;

        DoFloat(dt);
    }
	
    #endregion
	
    #region Methods

    /// <summary>
    /// Randomize the timer
    /// </summary>
    private void RandomizeTimer()
    {
        timer = Random.Range(0.0f, 1.0f);
    }

    /// <summary>
    /// Do the floating
    /// </summary>
    private void DoFloat(float dt)
    {
        timer += dt;

        float x = floatAmplitudes.x * (Mathf.Sin(floatFrequencies.x * timer));
        float y = floatAmplitudes.y * (Mathf.Sin(floatFrequencies.y * timer));
        float z = floatAmplitudes.z * (Mathf.Sin(floatFrequencies.z * timer));

        transform.localPosition -= new Vector3(x, y, z) * dt;
    }

    #endregion
}
