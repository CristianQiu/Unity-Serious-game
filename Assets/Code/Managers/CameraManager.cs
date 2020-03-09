using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The camera manager that controls the cameras of all the game
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
    #region Public Attributes

    //[SerializeField]
    //private Camera gameCam = null;
    [SerializeField]
    private Camera worldUICam = null;

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
        
    }
	
    #endregion
	
    #region Methods

    /// <summary>
    /// Set the world UI camera to on or off
    /// </summary>
    /// <param name="state"></param>
    public void SwitchWorldUICam(bool state)
    {
        worldUICam.enabled = state;
    }

    public void Reset()
    {
        
    }

    #endregion
}
