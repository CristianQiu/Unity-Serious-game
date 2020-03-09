using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class storing the text description where the player will have to guess from how certain 
/// words are written
/// </summary>
[System.Serializable]
public class DescriptionText
{
    #region Constructor

    public DescriptionText()
    {
        
    }

    #endregion

    #region Public Attributes

    public string[] allowedStrings = null;
    public string displayedText = null;
    public string solutionString = null;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private List<int> underscoreIndexes = new List<int>();
    private int currUnderscoreIndex = 0;
    private bool wantToDeleteThisFrame = false;
    private float timer = 0.0f;

    // they're inverted to ease substitution
    private const string red = "cc0000";
    private const string green = "00cc00";
    private const string white = "000000";

    #endregion

    #region Properties



    #endregion


    #region Methods

    public void Update(bool activated)
    {
        if (!activated)
            return;

        bool hasFinished = (currUnderscoreIndex == underscoreIndexes.Count);

        if (hasFinished)
        {
            timer += Time.deltaTime;

            if (timer >= 1.8f)
            {
                GameManager.Instance.SwitchState(GameState.InLevelPlaying, false);
            }
        }


        GameManager mgr = GameManager.Instance;
        if (mgr == null || mgr.CurrState != GameState.InLevelShowingDescription || currUnderscoreIndex == underscoreIndexes.Count)
            return;

        string input = GetThisFrameInputString();

        if (input == null)
            return;

        bool allowed = AllowedString(input);

        if (!allowed)
            return;

        WriteLetterThatHasBeenInput(input);

        if (currUnderscoreIndex == underscoreIndexes.Count)
            return;

        int index = underscoreIndexes[currUnderscoreIndex];
        ColorUnderscoreIndex(green, index);
    }


    private void WriteLetterThatHasBeenInput(string letter)
    {
        bool rightAnswer = (solutionString[currUnderscoreIndex].ToString() == letter);

        int index = underscoreIndexes[currUnderscoreIndex];

        string newText = GameManager.Instance.descriptionText.text.Insert(index, letter);
        newText = newText.Remove(index+1, 1);

        GameManager.Instance.SetDescriptionText(newText);

        string color = (rightAnswer) ? green : red;

        ColorUnderscoreIndex(color, index);

        if (rightAnswer)
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordCompleted, true);
        else
        {
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordFailed, true);
            GameManager.Instance.GuessLetterErrors++;
        }
           

        if (!rightAnswer && GameManager.Instance.EnemyEvent)
            GameManager.Instance.theGamePlayer.TakeLife(-1);

        if (!rightAnswer)
            GameManager.Instance.NumFails++;

        currUnderscoreIndex++;
    }

    /// <summary>
    /// Check that a string is allowed to be written, i.e b or v for the b or v level
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private bool AllowedString(string str)
    {
        bool ok = false;

        for (int i = 0; i < allowedStrings.Length; i++)
        {
            if (str.Equals(allowedStrings[i]))
            {
                ok = true;
                break;
            }
        }

        return ok;
    }

    /// <summary>
    /// Initialization, called when the object is built
    /// </summary>
    public void Init()
    {
        InitUnderscoreIndexes();
    }

    /// <summary>
    /// Search in the displayed text for undescore, thus marking where the points in the string are
    /// </summary>
    private void InitUnderscoreIndexes()
    {
        for (int i = 0; i < displayedText.Length; i++)
        {
            // look for the underscore
            if (displayedText[i].ToString() == "_")
            {
                underscoreIndexes.Add(i);
            }
        }
    }

    /// <summary>
    /// Color an underscore position 
    /// </summary>
    /// <param name="index"></param>
    private void ColorUnderscoreIndex(string color, int index)
    {
        int firstOffsetedIndex =  index - 2;

        int numPositions = 6;
        int lastOffsetedIndex = firstOffsetedIndex - numPositions;

        int colorIndex = color.Length - 1;

        char[] text = GameManager.Instance.descriptionText.text.ToCharArray();

        for (int i = firstOffsetedIndex; i > lastOffsetedIndex; i--)
        {
            text[i] = color[colorIndex];
            colorIndex--;
        }

        GameManager.Instance.SetDescriptionText(new string(text));
    }

    /// <summary>
    /// Get the input of this frame
    /// </summary>
    /// <returns></returns>
    private string GetThisFrameInputString()
    {
        wantToDeleteThisFrame = false;
        //wantToAcceptThisFrame = false;

        string userWrote = null;
        
        foreach (char c in Input.inputString)
        {
            // backspace / delete been pressed
            if (c == '\b')
                wantToDeleteThisFrame = true;
            //// enter / return been pressed
            //else if ((c == '\n') || (c == '\r'))
            //    wantToAcceptThisFrame = true;
            else
                userWrote += c;
        }

        return userWrote;
    }

    #endregion
}
