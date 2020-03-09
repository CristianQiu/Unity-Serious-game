using UnityEngine;

/// <summary>
/// A class used to alpha-fade a canvas renderer in a symmetrical way. That is because if using different functions or time
/// to fade in and out, the process could make some unwanted jumps in time
/// </summary>

public class ScaleFader : MonoBehaviour
{
    #region Delegates

    // TODO: This delegates should be in a manager holding all the UI elements that could be faded
    public delegate void FinishedFadingInEventHandler(object source, System.EventArgs args);
    public event FinishedFadingInEventHandler FinishedFadingIn;

    public delegate void FinishedFadingOutEventHandler(object source, System.EventArgs args);
    public event FinishedFadingOutEventHandler FinishedFadingOut;

    #endregion

    #region Public Attributes 

    [Header("Values")]

    public Vector3 fadedOutValue = Vector3.zero;

    private Vector3 fadedInValue = Vector3.one;
    
    [Header("Time")]
    public float timeToFade = 0.75f;

    [Header("Type")]
    public InterpolationFunction fadeType = InterpolationFunction.Linear;

    [Header("Starting state")]
    public FadeState startState = FadeState.FadedIn;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private CanvasRenderer canvasRend = null;
    private FadeState currState = FadeState.Invalid;
    private Vector3 currValue = Vector3.zero;
    private float timer = 0.0f;

    #endregion

    #region Properties

    public Vector3 CurrValue { get { return currValue; } }
    public bool FadedOut { get { return currState == FadeState.FadedOut; } }
    public bool FadingIn { get { return currState == FadeState.FadingIn; } }
    public bool FadedIn { get { return currState == FadeState.FadedIn; } }
    public bool FadingOut { get { return currState == FadeState.FadingOut; } }

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        fadedInValue = transform.localScale;
        canvasRend = GetComponent<CanvasRenderer>();
        SwitchState(startState, 0.0f, true);
    }

    // Use this for initialization
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        float dt = Time.deltaTime;

        UpdateState(dt);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {

    }

    #endregion

    #region State Methods

    /// <summary>
    /// Switch the current state for a new one
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="forcedStartTime"></param>
    /// <param name="force"></param>
    private void SwitchState(FadeState newState, float forcedStartTime, bool force)
    {
        if (currState == newState && !force)
            return;

        switch (currState)
        {
            case FadeState.FadedOut:
                ExitFadedOut();
                break;
            case FadeState.FadingIn:
                ExitFadingIn();
                break;
            case FadeState.FadedIn:
                ExitFadedIn();
                break;
            case FadeState.FadingOut:
                ExitFadingOut();
                break;
            default:
                Debug.Log("Invalid FadeState : " + currState);
                break;
        }

        switch (newState)
        {
            case FadeState.FadedOut:
                EnterFadedOut();
                break;
            case FadeState.FadingIn:
                EnterFadingIn(forcedStartTime);
                break;
            case FadeState.FadedIn:
                EnterFadedIn();
                break;
            case FadeState.FadingOut:
                EnterFadingOut(forcedStartTime);
                break;
            default:
                Debug.Log("Invalid FadeState : " + newState);
                break;
        }

        currState = newState;
    }

    /// <summary>
    /// Enter the faded out state
    /// </summary>
    private void EnterFadedOut()
    {
        timer = 0.0f;
        SetCurrScale(fadedOutValue);
    }

    /// <summary>
    /// Enter the fading in state
    /// </summary>
    /// <param name="forcedStartTime"></param>
    private void EnterFadingIn(float forcedStartTime)
    {
        timer = forcedStartTime;
    }

    /// <summary>
    /// Enter the faded in state
    /// </summary>
    private void EnterFadedIn()
    {
        timer = 0.0f;
        SetCurrScale(fadedInValue);
    }

    /// <summary>
    /// Enter the fading out state
    /// </summary>
    /// <param name="forcedStartTime"></param>
    private void EnterFadingOut(float forcedStartTime)
    {
        timer = forcedStartTime;
    }

    /// <summary>
    /// Exit the faded out state
    /// </summary>
    private void ExitFadedOut()
    {
        timer = 0.0f;
        SetCurrScale(fadedOutValue);
    }

    /// <summary>
    /// Exit the fading in state
    /// </summary>
    private void ExitFadingIn()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Exit the faded in state
    /// </summary>
    private void ExitFadedIn()
    {
        timer = 0.0f;
        SetCurrScale(fadedInValue);
    }

    /// <summary>
    /// Exit the fading out state
    /// </summary>
    private void ExitFadingOut()
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
            case FadeState.FadedOut:
                break;
            case FadeState.FadingIn:
                UpdateFadingIn(dt);
                break;
            case FadeState.FadedIn:
                break;
            case FadeState.FadingOut:
                UpdateFadingOut(dt);
                break;
            default:
                Debug.Log("Invalid FadeState : " + currState);
                break;
        }
    }

    /// <summary>
    /// Update the fade in state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateFadingIn(float dt)
    {
        float t = timer / timeToFade;

        t = Mathf.Clamp01(t);
        float s = CustomInterpolation.Interpolate(t, fadeType);
        Vector3 newScale = Vector3.Lerp(fadedOutValue, fadedInValue, s);

        SetCurrScale(newScale);

        if (t >= 1.0f)
        {
            OnFinishedFadingIn();
            SwitchState(FadeState.FadedIn, 0.0f, false);
        }
    }

    /// <summary>
    /// Update the fade out state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateFadingOut(float dt)
    {
        float t = timer / timeToFade;

        t = Mathf.Clamp01(t);
        t = 1.0f - t;
        float s = CustomInterpolation.Interpolate(t, fadeType);
        Vector3 newScale = Vector3.Lerp(fadedOutValue, fadedInValue, s);

        SetCurrScale(newScale);

        if (t <= 0.0f)
        {
            OnFinishedFadingOut();
            SwitchState(FadeState.FadedOut, 0.0f, false);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Set the current alpha value
    /// </summary>
    /// <param name="newValue"></param>
    private void SetCurrScale(Vector3 newScale)
    {
        currValue = newScale;
        transform.localScale = newScale;
    }

    /// <summary>
    /// Instantly fades out
    /// </summary>
    private void InstantFadeOut()
    {
        SetCurrScale(fadedOutValue);
        SwitchState(FadeState.FadedOut, 0.0f, true);

        OnFinishedFadingOut();
    }

    /// <summary>
    /// Instantly fades in
    /// </summary>
    private void InstantFadeIn()
    {
        SetCurrScale(fadedInValue);
        SwitchState(FadeState.FadedIn, 0.0f, true);

        OnFinishedFadingIn();
    }

    /// <summary>
    /// Start the fading process
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="forceInstant"></param>
    public void StartFade(FadeState newState, bool forceInstant)
    {
        bool dontAllow = (newState != FadeState.FadingIn) && (newState != FadeState.FadingOut) ||
                         (newState == FadeState.FadingIn && FadedIn) ||
                         (newState == FadeState.FadingOut && FadedOut) ||
                         (newState == FadeState.FadingIn && FadingIn) ||
                         (newState == FadeState.FadingOut && FadingOut);

        if (dontAllow)
            return;

        // handle the instant face
        if (forceInstant)
        {
            if (newState == FadeState.FadingOut)
                InstantFadeOut();
            else
                InstantFadeIn();

            return;
        }
            
        // if trying to reverse the fade while already fading we are going to set the timer to reverse the direction smoothly
        float timeLeftToFinish = 0.0f;

        if (FadingIn || FadingOut)
            timeLeftToFinish = timeToFade - timer;

        SwitchState(newState, timeLeftToFinish, false);
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called when the fading in finishes
    /// </summary>
    protected virtual void OnFinishedFadingIn()
    {
        if (FinishedFadingIn != null)
            FinishedFadingIn(this, System.EventArgs.Empty);
    }

    /// <summary>
    /// Called when the fading out finishes
    /// </summary>
    protected virtual void OnFinishedFadingOut()
    {
        if (FinishedFadingOut != null)
            FinishedFadingOut(this, System.EventArgs.Empty);

        Destroy(gameObject);
    }

    #endregion
}
