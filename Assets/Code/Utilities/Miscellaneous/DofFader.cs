using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// A script to make things out of focus progressively
/// </summary>
public class DofFader : MonoBehaviour
{
    #region Public Attributes 

    [Header("Values")]
    [Range(0.1f, 5.0f)]
    public float fadedOutValue = 0.1f;
    [Range(0.1f, 5.0f)]
    public float fadedInValue = 2.0f;

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

    private DepthOfField dof = null;
    private FadeState currState = FadeState.Invalid;
    private float currValue = 0.0f;
    private float timer = 0.0f;

    #endregion

    #region Properties

    public float CurrValue { get { return dof.focusDistance.value; } }
    public bool FadedOut { get { return currState == FadeState.FadedOut; } }
    public bool FadingIn { get { return currState == FadeState.FadingIn; } }
    public bool FadedIn { get { return currState == FadeState.FadedIn; } }
    public bool FadingOut { get { return currState == FadeState.FadingOut; } }

    #endregion

    #region MonoBehaviour Methods

    // Use this for initialization
    void Start()
    {
        Init();
        GameManager.Instance.ShowDescription += OnShowDescription;
        GameManager.Instance.HideDescription += OnHideDescription;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        UpdateState(dt);
    }

    private void OnDestroy()
    {
        //GameManager.Instance.ShowDescription -= OnShowDescription;
        //GameManager.Instance.HideDescription -= OnHideDescription;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        PostProcessVolume postFxVolume = GetComponent<PostProcessVolume>();
        postFxVolume.sharedProfile.TryGetSettings(out dof);
        Debug.Assert(dof != null, "There's no depth of field in the postprocessing volume and this the DofFader won't work!");
        StartFade(FadeState.FadingOut, true);
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
        SetCurrFocusDistance(fadedOutValue);
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
        SetCurrFocusDistance(fadedInValue);
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
        SetCurrFocusDistance(fadedOutValue);
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
        SetCurrFocusDistance(fadedInValue);
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
        s = Mathf.Lerp(fadedOutValue, fadedInValue, s);

        SetCurrFocusDistance(s);

        if (t >= 1.0f)
            SwitchState(FadeState.FadedIn, 0.0f, false);
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
        s = Mathf.Lerp(fadedOutValue, fadedInValue, s);

        SetCurrFocusDistance(s);

        if (t <= 0.0f)
            SwitchState(FadeState.FadedOut, 0.0f, false);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Set the current alpha value
    /// </summary>
    /// <param name="newValue"></param>
    private void SetCurrFocusDistance(float newValue)
    {
        currValue = newValue;
        dof.focusDistance.value = currValue; 
    }

    /// <summary>
    /// Instantly fades out
    /// </summary>
    private void InstantFadeOut()
    {
        SetCurrFocusDistance(fadedOutValue);
        SwitchState(FadeState.FadedOut, 0.0f, true);
    }

    /// <summary>
    /// Instantly fades in
    /// </summary>
    private void InstantFadeIn()
    {
        SetCurrFocusDistance(fadedInValue);
        SwitchState(FadeState.FadedIn, 0.0f, true);
    }

    /// <summary>
    /// Start the fading process if forceInstant is true then the new state will be already faded to the newState
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
    /// Called once when showing the description
    /// </summary>
    protected virtual void OnShowDescription(object source, System.EventArgs args)
    {
        StartFade(FadeState.FadingIn, false);
    }

    /// <summary>
    /// Called once when hiding the description
    /// </summary>
    protected virtual void OnHideDescription(object source, System.EventArgs args)
    {
        StartFade(FadeState.FadingOut, false);
    }

    #endregion
}
