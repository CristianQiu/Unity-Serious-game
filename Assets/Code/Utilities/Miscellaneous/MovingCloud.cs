using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for the moving background clouds
/// </summary>
public class MovingCloud : MonoBehaviour
{
    #region Public Attributes

    public Transform cloudEnterPos = null;
    public Transform cloudExitPos = null;
    public float xSpd = -0.4f;
    public float altXSpd = -0.2f;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private float currXSpeed = 0.0f;
    private float xOffsetFromEnterToExit = 0.0f;
	
    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        RandomizeStartingSpeed();
        xOffsetFromEnterToExit = (cloudExitPos.position - cloudEnterPos.position).magnitude;
    }
	
    // Update is called once per frame
    void Update () 
    {
        float dt = Time.deltaTime;
        Move(dt, xSpd);
    }

    private void OnTriggerEnter(Collider other)
    {
        CloudTrigger trigger = other.GetComponent<CloudTrigger>();

        if (trigger != null)
        {
            ResetPos(xOffsetFromEnterToExit);
            AlternateXSpeed();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Randomize the starting x speed from all the available options
    /// </summary>
    private void RandomizeStartingSpeed()
    {
        int num = Random.Range(0, 2);

        switch (num)
        {
            case 0:
                currXSpeed = xSpd;
                break;
            case 1:
                currXSpeed = altXSpd;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Alternate the xSpeed
    /// </summary>
    private void AlternateXSpeed()
    {
        currXSpeed = (currXSpeed == xSpd) ? altXSpd : xSpd;
    }

    /// <summary>
    /// Move the cloud
    /// </summary>
    private void Move(float dt, float xSpeed)
    {
        Vector3 pos = transform.position;
        pos.x += currXSpeed * dt;

        transform.position = pos;
    }

    /// <summary>
    /// Reset the cloud positon
    /// </summary>
    private void ResetPos(float xOffsetFromNow)
    {
        // just shift the cloud to its right
        Vector3 pos = transform.position;
        pos.x += xOffsetFromNow;

        transform.position = pos;
    }

    #endregion
}
