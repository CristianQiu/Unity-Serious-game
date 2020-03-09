using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brief description of the class here
/// </summary>
public class Rotator : MonoBehaviour
{
    #region Public Attributes

    public float yRotSpeed = 45.0f;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes


	
    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        
    }
	
    // Update is called once per frame
    void Update () 
    {
        transform.Rotate(new Vector3(0.0f, yRotSpeed, 0.0f) * Time.deltaTime, Space.World);
    }
	
    #endregion
	
    #region Methods



    #endregion
}
