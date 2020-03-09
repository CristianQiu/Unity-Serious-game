using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is going to help to player to write the words that are over the directions that he can move
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerWordWriter : MonoBehaviour
{
    #region Delegates

    public delegate void FinishedWordMatchEventHandler(object source, System.EventArgs args);
    public event FinishedWordMatchEventHandler FinishedWordMatch;

    #endregion

    #region Public Attributes

    // fill order = fwd, right, bwd, left
    public Text[] texts = null;
    public Color defaultTextColor = Color.white;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private bool wantToDeleteThisFrame = false;
    //private bool wantToAcceptThisFrame = false;

    // the "raw" available direction texts i.e: "Delante" null "Detrás" "Izquierda"
    private string[] availableDirTexts = new string[4];
    // the words that we may be written at this time i.e. "Delante" null "Detrás" null
    private string[] wordsBeingWritten = new string[4];
    private int atIndexOfWordBeingWritten = 0;

    private Player theGamePlayer = null;

    #endregion

    #region Properties



    #endregion

    #region MonoBehaviour Methods

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!theGamePlayer.CanWriteToMove)
            return;

        // get the input of this frame
        string thisFrameInputString = GetThisFrameInputString();

        bool hasMatchedSomethingAlready = HasMatchedSomethingAlready();
        PaintMatchedLetters(thisFrameInputString, hasMatchedSomethingAlready);
    }

    private void OnDestroy()
    {
        theGamePlayer.PlayerJustStopped -= OnPlayerJustStopped;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        theGamePlayer = GetComponent<Player>();
        theGamePlayer.PlayerJustStopped += OnPlayerJustStopped;

        EmptyStringArray(ref availableDirTexts);
        ResetAvailableDirTexts();
    }

    /// <summary>
    /// Make an array of strings be empty
    /// </summary>
    private void EmptyStringArray(ref string[] array)
    {
        if (array == wordsBeingWritten)
            atIndexOfWordBeingWritten = 0;

        for (int i = 0; i < array.Length; i++)
            array[i] = null;
    }

    /// <summary>
    /// Reset a text to its default state, the word being written will also be reset
    /// </summary>
    /// <param name="index"></param>
    private void ResetTextToDefault(int index)
    {
        texts[index].color = defaultTextColor;
        texts[index].text = wordsBeingWritten[index];
        wordsBeingWritten[index] = null;

        // if no words being written left we must restart the index of whats being written
        if (!IsAnyWordBeingWritten())
            atIndexOfWordBeingWritten = 0;
    }

    /// <summary>
    /// Fill the string array according to the dirs the player can move to, also reset the color, just in case
    /// </summary>
    public void ResetAvailableDirTexts()
    {
        //TODO: THIS IS TESTING !CHANGE IT AT SOME POINT
        //if (theGamePlayer.CanMoveFwd)
        //    availableDirTexts[0] = "Delante";
        //else
        //    availableDirTexts[0] = null;

        //if (theGamePlayer.CanMoveRight)
        //    availableDirTexts[1] = "Derecha";
        //else
        //    availableDirTexts[1] = null;

        //if (theGamePlayer.CanMoveBwd)
        //    availableDirTexts[2] = "Detrás";
        //else
        //    availableDirTexts[2] = null;

        //if (theGamePlayer.CanMoveLeft)
        //    availableDirTexts[3] = "Izquierda";
        //else
        //    availableDirTexts[3] = null;

        if (theGamePlayer.CanMoveFwd)
            availableDirTexts[0] = theGamePlayer.CurrTile.tileWords.fwdWord;
        else
            availableDirTexts[0] = null;

        if (theGamePlayer.CanMoveRight)
            availableDirTexts[1] = theGamePlayer.CurrTile.tileWords.rightWord;
        else
            availableDirTexts[1] = null;

        if (theGamePlayer.CanMoveBwd)
            availableDirTexts[2] = theGamePlayer.CurrTile.tileWords.bwdWord;
        else
            availableDirTexts[2] = null;

        if (theGamePlayer.CanMoveLeft)
            availableDirTexts[3] = theGamePlayer.CurrTile.tileWords.leftWord;
        else
            availableDirTexts[3] = null;

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = availableDirTexts[i];
            texts[i].color = defaultTextColor;
        }
    }

    /// <summary>
    /// Reset all the text state, meaning all color will be cleared, the written words will be erased
    /// and the text displayed will be "reinitialized"
    /// </summary>
    private void ResetAllTexts()
    {
        for (int i = 0; i < texts.Length; i++)
            ResetTextToDefault(i);

        ResetAvailableDirTexts();
    }

    /// <summary>
    /// Returns wether the player has managed to match any letter yet or not
    /// </summary>
    /// <returns></returns>
    private bool HasMatchedSomethingAlready()
    {
        bool hasMatchedSomethingAlready = false;

        for (int i = 0; i < wordsBeingWritten.Length; i++)
        {
            if (wordsBeingWritten[i] != null)
            {
                hasMatchedSomethingAlready = true;
                break;
            }
        }

        return hasMatchedSomethingAlready;
    }

    /// <summary>
    /// Returns wether if any word is being written
    /// </summary>
    /// <returns></returns>
    private bool IsAnyWordBeingWritten()
    {
        bool isAnyWordBeingWritten = false;
        
        for (int i = 0; i < wordsBeingWritten.Length; i++)
        {
            if (wordsBeingWritten[i] != null)
            {
                isAnyWordBeingWritten = true;
                break;
            }
        }

        return isAnyWordBeingWritten;
    }

    /// <summary>
    /// Returns wether the player finished matching a word or not
    /// </summary>
    /// <param name="currWord"></param>
    /// <param name="wordBeingWrittenIndex"></param>
    /// <returns></returns>
    private bool HasFinishedMatchingWord(string currWord, int wordBeingWrittenIndex)
    {
        bool hasFinishedMatchingAWord = false;

        if (wordBeingWrittenIndex == currWord.Length - 1)
            hasFinishedMatchingAWord = true;

        return hasFinishedMatchingAWord;
    }

    /// <summary>
    /// Paint the letters that match the string passed as the argument
    /// </summary>
    /// <param name="letterToMatch"></param>
    /// <param name="hasMatchedSomethingAlready"></param>
    private void PaintMatchedLetters(string letterToMatch, bool hasMatchedSomethingAlready)
    {
        if (wantToDeleteThisFrame && hasMatchedSomethingAlready)
        {
            // play "error" sound
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordFailed, true);
            ResetAllTexts();
            return;
        }          
            
        if (letterToMatch == null)
            return;

        bool finishedMatchingWord = false;

        // This part is becoming a monstruosity, perhaps should have a look so I can better the presentation of this chunk of code

        // CHECKS THE FIRST LETTER

        // if the player didn't match anything yet, compare against all available direction texts
        if (!hasMatchedSomethingAlready)
        {
            bool playedTapSoundAlrdy = false;

            // im going to treat lettersToMatch as a single letter atm
            for (int i = 0; i < availableDirTexts.Length; i++)
            {
                // there are 4 slots, some of them may be null
                if (availableDirTexts[i] == null)
                    continue;

                bool matchesWithLowerCase = availableDirTexts[i].StartsWith(letterToMatch.ToUpper());

                if (availableDirTexts[i].StartsWith(letterToMatch) || matchesWithLowerCase)
                {
                    hasMatchedSomethingAlready = true;
                    // initialize the array with the words he / she may be writing
                    wordsBeingWritten[i] = availableDirTexts[i];
                    PaintLetter(i, atIndexOfWordBeingWritten, letterToMatch, matchesWithLowerCase);

                    if (!playedTapSoundAlrdy && !HasFinishedMatchingWord(wordsBeingWritten[i], atIndexOfWordBeingWritten))
                    {
                        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipKeyTap, true);
                        playedTapSoundAlrdy = true;
                    }

                    // check if the player finished matching a word and if so trigger a callback
                    if (HasFinishedMatchingWord(wordsBeingWritten[i], atIndexOfWordBeingWritten))
                    {
                        OnFinishedWordMatch(wordsBeingWritten[i]);
                        finishedMatchingWord = true;
                    } 
                }
            }

            // if something did match, increase the character we are at only once
            if (hasMatchedSomethingAlready && !finishedMatchingWord)
                atIndexOfWordBeingWritten++;
        }

        // CHECKS THE FOLLOWING LETTERS

        // if something was matched already, then compare against the words that he is writing
        else
        {
            bool match = false;
            bool playedTapSoundAlrdy = false;

            for (int i = 0; i < wordsBeingWritten.Length; i++)
            {
                // there are 4 slots, some of them may be null
                if (wordsBeingWritten[i] == null)
                    continue;

                char currCharChecked = wordsBeingWritten[i][atIndexOfWordBeingWritten];
                
                if (currCharChecked.ToString().Equals(letterToMatch))
                {
                    match = true;
                    PaintLetter(i, atIndexOfWordBeingWritten, letterToMatch, false);

                    if (!playedTapSoundAlrdy && !HasFinishedMatchingWord(wordsBeingWritten[i], atIndexOfWordBeingWritten))
                    {
                        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipKeyTap, true);
                        playedTapSoundAlrdy = true;
                    }

                    // check if the player finished matching a word and if so trigger a callback
                    if (HasFinishedMatchingWord(wordsBeingWritten[i], atIndexOfWordBeingWritten))
                    {
                        OnFinishedWordMatch(wordsBeingWritten[i]);
                        finishedMatchingWord = true;
                    }     
                }
                else
                {
                    // reset this word being written and its related text to the default color
                    ResetTextToDefault(i);
                    
                    if (!IsAnyWordBeingWritten())
                    {
                        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordFailed, true);
                        GameManager.Instance.TypoErrors++;
                    }
                       
                }
            }

            // if something did match increase counter by 1 (2 words may start with "De...")
            if (match && !finishedMatchingWord)
                atIndexOfWordBeingWritten++;
        }
    }

    /// <summary>
    /// Paint a letter from one of the texts (index) at one position in its tex string
    /// </summary>
    /// <param name="atText"></param>
    /// <param name="atStringIndex"></param>
    /// <param name="letterToBeColored"></param>
    private void PaintLetter(int atText, int atStringIndex, string letterToBeColored, bool matchesWithLowerCase/*, Color color*/)
    {
        Text UIText = texts[atText];
        string textString = UIText.text;

        // get the index of the letter we want to colorize. HACK!! 24 is all the characters offset that the <color...> tag introduces
        // so we get rid of it by multiplying the real index
        if (matchesWithLowerCase)
            letterToBeColored = letterToBeColored.ToUpper();

        int replacedStringIndex = textString.IndexOf(letterToBeColored, atStringIndex * 24);

        // now remove it
        textString = textString.Remove(replacedStringIndex, 1);

        // and add it again
        letterToBeColored = "<color=#00cc00>" + letterToBeColored + "</color>";
        textString = textString.Insert(replacedStringIndex, letterToBeColored);

        UIText.text = textString;
    }

    /// <summary>
    /// Get the input that has been made this frame by the user
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

    /// <summary>
    /// Calculate the dir (0 1 2 3 fwd, right, bwd, left) the player should move to based on the word that has been written
    /// </summary>
    /// <param name="wordBeingWritten"></param>
    /// <returns></returns>
    private int CalcDirToMoveTo(string wordBeingWritten)
    {
        int index = -1;

        for (int i = 0; i < wordsBeingWritten.Length; i++)
        {
            if (wordsBeingWritten[i] == null)
                continue;

            if (wordsBeingWritten[i].Equals(wordBeingWritten))
                index = i;
        }

        return index;
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called once right after the player completes a word
    /// </summary>
    protected virtual void OnFinishedWordMatch(string wordBeingWritten)
    {
        if (FinishedWordMatch != null)
            FinishedWordMatch(this, System.EventArgs.Empty);

        // calc the dir to move
        int dir = CalcDirToMoveTo(wordBeingWritten);
        theGamePlayer.MoveTo(dir);

        // and reset the array of words that are being written and the index we are at
        EmptyStringArray(ref wordsBeingWritten);

        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordCompleted, true);
    }

    /// <summary>
    /// Called the frame when the player stops moving from tile to tile
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected virtual void OnPlayerJustStopped(object source, System.EventArgs args)
    {
        ResetAvailableDirTexts();
    }

    #endregion
}
