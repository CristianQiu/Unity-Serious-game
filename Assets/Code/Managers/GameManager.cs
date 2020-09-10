using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Invalid = -1,

    InMenu,
    InLevelPlaying,
    InLevelShowingDescription,
    Paused,
    GameOverByDying,
    GameOverByWin,
    //...

    Count,
}

/// <summary>
/// The game manager that controls the states of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
    #region Delegates

    public delegate void ShowDescriptionEventHandler(object source, System.EventArgs args);

    public event ShowDescriptionEventHandler ShowDescription;

    public delegate void HideDescriptionEventHandler(object source, System.EventArgs args);

    public event HideDescriptionEventHandler HideDescription;

    #endregion

    #region Public Attributes

    public float minTimeSpentReadingDescription = 2.0f;
    public Player theGamePlayer = null;

    public CanvasRendererSymmetricAlphaFader blackBg = null;
    public AudioSourceVolumeFader musicFader = null;

    // ScreenSpace UI Parent
    public GameObject screenSpaceUiParent = null;

    public Text descriptionText = null;
    public Text DIEDText = null;

    public GameObject winUiParent = null;

    public Text calificationText = null;
    public Text foundTrophiesText = null;
    public Text typoErrorsText = null;
    public Text guessLetterErrorsText = null;

    public Tile exitTile = null;

    #endregion

    #region Protected Attributes

    #endregion

    #region Private Attributes

    private int typoErrors = 0;
    private int guessLetterErrors = 0;

    private bool enemyEvent = false;
    private GameState currState = GameState.Invalid;
    private float timer = 0.0f;
    private Tile nextPlayerTile = null;
    private List<Enemy> enemies = new List<Enemy>();
    private int numFails = 0;
    private CanvasRendererSymmetricAlphaFader[] canvasRendererAlphaFaders = null;

    #endregion

    #region Properties

    public List<Enemy> Enemies { get { return enemies; } }
    public GameState CurrState { get { return currState; } }
    public Player TheGamePlayer { get { return theGamePlayer; } }

    public Tile NextPlayerTile
    {
        get
        {
            return nextPlayerTile;
        }

        set
        {
            nextPlayerTile = value;
        }
    }

    public bool EnemyEvent
    {
        get
        {
            return enemyEvent;
        }

        set
        {
            enemyEvent = value;
        }
    }

    public int NumFails
    {
        get
        {
            return numFails;
        }

        set
        {
            numFails = value;
        }
    }

    public int TypoErrors
    {
        get
        {
            return typoErrors;
        }

        set
        {
            typoErrors = value;
        }
    }

    public int GuessLetterErrors
    {
        get
        {
            return guessLetterErrors;
        }

        set
        {
            guessLetterErrors = value;
        }
    }

    #endregion

    #region MonoBehaviour Methods

    private void Start()
    {
        blackBg.FinishedFadingIn += OnFinishFadingIn;
        Cursor.visible = false;
        Init();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            theGamePlayer.Disabled = true;
            blackBg.StartFade(FadeState.FadingIn, false);
            musicFader.StartFade(FadeState.FadingOut, false);
        }

        UpdateState(dt);
    }

    private void OnDestroy()
    {
        blackBg.FinishedFadingIn -= OnFinishFadingIn;
    }

    #endregion

    #region Methods

    public void Reset()
    {
        Init();
        nextPlayerTile = null;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        Enemy[] theEnemies = FindObjectsOfType<Enemy>();
        enemyEvent = false;
        timer = 0.0f;
        nextPlayerTile = null;
        numFails = 0;

        for (int i = 0; i < theEnemies.Length; i++)
            enemies.Add(theEnemies[i]);

        canvasRendererAlphaFaders = screenSpaceUiParent.GetComponentsInChildren<CanvasRendererSymmetricAlphaFader>(true);
        FadeScreenSpaceUI(FadeState.FadingOut, true);
        Debug.Assert(theGamePlayer != null, "The game manager does not have the player referenced !");
        SwitchState(GameState.InLevelPlaying, true);
    }

    /// <summary>
    /// Switch the current state for a new one
    /// </summary>
    /// <param name="dt"></param>
    public void SwitchState(GameState newState, bool force)
    {
        if (currState == newState && !force)
            return;

        switch (currState)
        {
            case GameState.Invalid:
                Debug.Log("Initializing the gamestate from Invalid, ignore this message if you just started the game");
                break;

            case GameState.InMenu:
                ExitMenu();
                break;

            case GameState.InLevelPlaying:
                ExitInLevelPlaying();
                break;

            case GameState.InLevelShowingDescription:
                ExitInLevelShowingDescription();
                break;

            case GameState.Paused:
                ExitPaused();
                break;

            case GameState.GameOverByDying:
                timer = 0.0f;
                break;

            case GameState.GameOverByWin:
                timer = 0.0f;
                break;

            default:
                Debug.Log("No such state " + currState);
                break;
        }

        switch (newState)
        {
            case GameState.InMenu:
                EnterMenu();
                break;

            case GameState.InLevelPlaying:
                EnterInLevelPlaying();
                break;

            case GameState.InLevelShowingDescription:
                EnterInLevelShowingDescription();
                break;

            case GameState.Paused:
                EnterPaused();
                break;

            case GameState.GameOverByDying:
                DIEDText.enabled = true;
                theGamePlayer.Disabled = true;
                blackBg.StartFade(FadeState.FadingIn, false);
                musicFader.StartFade(FadeState.FadingOut, false);
                timer = 0.0f;
                break;

            case GameState.GameOverByWin:

                theGamePlayer.Disabled = true;
                typoErrorsText.text = "Errores de tipografía: " + typoErrors;
                guessLetterErrorsText.text = "Errores de adivinar letra: " + guessLetterErrors;

                int numTrophies = System.Convert.ToInt32(TheGamePlayer.secretsText.text);

                float trophyScore = (numTrophies / 2.0f) * 10.0f;
                typoErrors = Mathf.Clamp(typoErrors, 0, 20);
                float typoErrorsScore = (1.0f - (typoErrors / 20.0f)) * 10.0f;

                guessLetterErrors = Mathf.Clamp(guessLetterErrors, 0, 10);
                float guessLetterErrorsScore = (1.0f - (guessLetterErrors / 10.0f)) * 10.0f;

                float finalScore = (trophyScore + typoErrorsScore + guessLetterErrorsScore) / 3.0f;

                foundTrophiesText.text = "Has encontrado " + numTrophies + " / 2 letras trofeo";
                winUiParent.SetActive(true);
                calificationText.text = "Calificación : " + finalScore.ToString("0.00") + " / 10";

                timer = 0.0f;
                break;

            default:
                Debug.Log("No such state " + newState);
                break;
        }

        currState = newState;
    }

    /// <summary>
    /// Enter the menu state (the main menu, not "pause")
    /// </summary>
    private void EnterMenu()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Enter the level state
    /// </summary>
    private void EnterInLevelPlaying()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Enter the showing a description state
    /// </summary>
    private void EnterInLevelShowingDescription()
    {
        timer = 0.0f;

        OnShowDescription();
    }

    /// <summary>
    /// Called when entering the pause
    /// </summary>
    private void EnterPaused()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Exit the menu state (the main menu, not "pause")
    /// </summary>
    private void ExitMenu()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Exit the level state
    /// </summary>
    private void ExitInLevelPlaying()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Exit the showing a description state
    /// </summary>
    private void ExitInLevelShowingDescription()
    {
        timer = 0.0f;

        OnHideDescription();
    }

    /// <summary>
    /// Called when exiting the pause
    /// </summary>
    private void ExitPaused()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Update the state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateState(float dt)
    {
        timer += dt;

        switch (currState)
        {
            case GameState.InMenu:
                UpdateInMenu(dt);
                break;

            case GameState.InLevelPlaying:
                UpdateInLevelPlaying(dt);
                break;

            case GameState.InLevelShowingDescription:
                UpdateInLevelShowingDescription(dt);
                break;

            case GameState.Paused:
                // unscaled dt?
                UpdateGamePaused(dt);
                break;

            case GameState.GameOverByDying:
                timer += dt;
                break;

            case GameState.GameOverByWin:
                timer += 0.0f;
                break;

            default:
                Debug.Log("No such state " + currState);
                break;
        }
    }

    /// <summary>
    /// Update while in the main menu
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateInMenu(float dt)
    {
        timer += dt;
    }

    /// <summary>
    /// Update while in the level
    /// </summary>
    private void UpdateInLevelPlaying(float dt)
    {
        timer += dt;
    }

    /// <summary>
    /// Update the game while showing a description
    /// </summary>
    private void UpdateInLevelShowingDescription(float dt)
    {
        timer += dt;

        if (timer < minTimeSpentReadingDescription)
            return;

        //if (Input.GetKeyDown(KeyCode.Return))
        //    SwitchState(GameState.InLevelPlaying, false);
    }

    /// <summary>
    /// Update the game while its paused
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateGamePaused(float dt)
    {
        timer += dt;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Fade the screen space UI
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="forceInstant"></param>
    private void FadeScreenSpaceUI(FadeState newState, bool forceInstant)
    {
        for (int i = 0; i < canvasRendererAlphaFaders.Length; i++)
        {
            CanvasRendererSymmetricAlphaFader alphaFader = canvasRendererAlphaFaders[i];

            if (!forceInstant)
            {
                alphaFader.StartFade(newState, false);
            }
            else
            {
                if (newState == FadeState.FadingIn)
                    alphaFader.StartFade(FadeState.FadingIn, true);
                else
                    alphaFader.StartFade(FadeState.FadingOut, true);
            }
        }
    }

    /// <summary>
    /// set the description text
    /// </summary>
    /// <param name="newText"></param>
    public void SetDescriptionText(string newText)
    {
        descriptionText.text = newText;
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called when the game starts showing a description or whatever after completing a word
    /// </summary>
    protected virtual void OnShowDescription()
    {
        if (ShowDescription != null)
            ShowDescription(this, System.EventArgs.Empty);

        FadeScreenSpaceUI(FadeState.FadingIn, false);
        CameraManager.Instance.SwitchWorldUICam(false);
    }

    /// <summary>
    /// Called when the game starts hiding the description shown in OnShowDescription
    /// </summary>
    protected virtual void OnHideDescription()
    {
        if (HideDescription != null)
            HideDescription(this, System.EventArgs.Empty);

        FadeScreenSpaceUI(FadeState.FadingOut, false);
        CameraManager.Instance.SwitchWorldUICam(true);

        // now tell the player to move
        theGamePlayer.TileMovingTo = nextPlayerTile;
        nextPlayerTile = null;
        theGamePlayer.SwitchState(PlayerState.MovingToTile, false);
    }

    protected virtual void OnFinishFadingIn(object source, System.EventArgs args)
    {
        SceneManager.LoadScene("menu");
    }

    #endregion
}