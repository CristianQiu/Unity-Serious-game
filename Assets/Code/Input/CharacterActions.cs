using UnityEngine;

/// <summary>
/// This class is going to ease the usage of our Inputs so we can retrieve actions more than the raw input. It will be easier to come here and remap controls if we need to
/// </summary>
public class CharacterActions
{
    #region Constructor

    public CharacterActions()
    {
        
    }

    #endregion

    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private Inputs input = new Inputs();

    #endregion

    #region Properties



    #endregion

    #region Input Updating Methods

    /// <summary>
    /// Should be called in the update loop from a monobehaviour class, otherwise input won't be updated
    /// </summary>
    public void Update()
    {
        input.Update();
    }

    #endregion

    #region Input Retrieving Methods

    /// <summary>
    /// Get the moving vector
    /// </summary>
    /// <param name="smoothed"></param>
    /// <param name="invertedX"></param>
    /// <param name="invertedY"></param>
    /// <returns></returns>
    public Vector2 Move(bool smoothed, bool invertedX, bool invertedY)
    {
        return input.LeftStick(smoothed, invertedX, invertedY);
    }

    /// <summary>
    /// Get the looking vector
    /// </summary>
    /// <param name="smoothed"></param>
    /// <param name="invertedX"></param>
    /// <param name="invertedY"></param>
    /// <returns></returns>
    public Vector2 Look(bool smoothed, bool invertedX, bool invertedY)
    {
        return input.RightStick(smoothed, invertedX, invertedY);
    }

    #endregion
}