using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brief description of the class here
/// </summary>
public class RotateToForward : MonoBehaviour
{
    #region Public Attributes



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
        transform.forward = -Vector3.forward;
    }
	
    #endregion
	
    #region Methods



    #endregion
}
