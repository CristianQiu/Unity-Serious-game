using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brief description of the class here
/// </summary>
public class SetPosAtStart : MonoBehaviour
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
        transform.position = new Vector3(10.72f, 3.79f, 19.42f);
        GetComponent<MeshRenderer>().enabled = true;
    }
	
    // Update is called once per frame
    void Update () 
    {
        
    }
	
    #endregion
	
    #region Methods



    #endregion
}
