using UnityEngine;

/// <summary>
/// Class wrapping unity's input system so we can manage the changes that we may want to include in a centralized way, instead going through every script that used Input from unity's system
/// </summary>
public class Inputs
{
    #region Constructor

    public Inputs()
    {

    }

    #endregion

    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    // TODO: add the keyboard

    // TODO: add buttons, triggers, dpad...

    // gamepad
    private const string leftStickHorName = "LeftStickHorizontal";
    private const string leftStickVerName = "LeftStickVertical";
    private const string rightStickHorName = "RightStickHorizontal";
    private const string rightStickVerName = "RightStickVertical";

    private Vector2 leftStick = Vector2.zero;
    private Vector2 rightStick = Vector2.zero;

    private Vector2 leftStickRaw = Vector2.zero;
    private Vector2 rightStickRaw = Vector2.zero;

    #endregion

    #region Properties



    #endregion

    #region Input Updating Methods

    /// <summary>
    /// Update the left stick input
    /// </summary>
    private void UpdateLeftStickInput()
    {
        // first the smoothed
        float x = Input.GetAxis(leftStickHorName);
        float y = Input.GetAxis(leftStickVerName);

        leftStick = new Vector2(x ,y);

        // now the raw
        x = Input.GetAxisRaw(leftStickHorName);
        y = Input.GetAxisRaw(leftStickVerName);
        
        leftStickRaw = new Vector2(x, y);
    }

    /// <summary>
    /// Update the right stick input
    /// </summary>
    private void UpdateRightStickInput()
    {
        // first the smoothed
        float x = Input.GetAxis(rightStickHorName);
        float y = Input.GetAxis(rightStickVerName);

        rightStick = new Vector2(x, y);

        // now the raw
        x = Input.GetAxisRaw(rightStickHorName);
        y = Input.GetAxisRaw(rightStickVerName);

        rightStickRaw = new Vector2(x, y);
    }

    /// <summary>
    /// Should be called in the update loop from a monobehaviour class, otherwise input won't be updated
    /// </summary>
    public void Update()
    {
        UpdateLeftStickInput();
        UpdateRightStickInput();
    }

    #endregion

    #region Input Retrieving Methods

    /// <summary>
    /// Get the left stick input
    /// </summary>
    /// <param name="smoothed"></param>
    /// <param name="invertedX"></param>
    /// <param name="invertedY"></param>
    /// <returns></returns>
    public Vector2 LeftStick(bool smoothed, bool invertedX, bool invertedY)
    {
        Vector2 input = smoothed ? leftStick : leftStickRaw;

        input.x = invertedX ? -input.x : input.x;
        input.y = invertedY ? -input.y : input.y;

        return input;
    }

    /// <summary>
    /// Get the right stick input
    /// </summary>
    /// <param name="smoothed"></param>
    /// <param name="invertedX"></param>
    /// <param name="invertedY"></param>
    /// <returns></returns>
    public Vector2 RightStick(bool smoothed, bool invertedX, bool invertedY)
    {
        Vector2 input = smoothed ? rightStick : rightStickRaw;

        input.x = invertedX ? -input.x : input.x;
        input.y = invertedY ? -input.y : input.y;

        return input;
    }

    #endregion
}