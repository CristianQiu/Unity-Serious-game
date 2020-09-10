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

    #region Methods

    /// <summary>
    /// Set the world UI camera to on or off
    /// </summary>
    /// <param name="state"></param>
    public void SwitchWorldUICam(bool state)
    {
        worldUICam.enabled = state;
    }

    #endregion
}