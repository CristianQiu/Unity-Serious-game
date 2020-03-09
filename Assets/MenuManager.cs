using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

/// <summary>
/// Brief description of the class here
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region Typedefs

    public enum ChosenOption
    {
        Invalid = -1,

        Tutorial,
        StartGame,
        ExitApp,

        Count
    }

    #endregion

    #region Public Attributes

    public Text[] options = null;
    public CinemachineVirtualCamera[] vcams = null;

    public Color colorSelected = Color.green;
    public Color colorDeselected = Color.white;

    public AudioSourceVolumeFader musicFader = null;
    public CanvasRendererSymmetricAlphaFader blackBg = null;

    public GameObject parentVideosObj = null;

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private const int DefaultOption = 1;
    private int currSelectedOption = DefaultOption;

    private bool hasControl = true;
    private CanvasRendererSymmetricAlphaFader[] textFaders = null;

    private CanvasRendererSymmetricAlphaFader[] videoTutorialsFaders = null;

    private ChosenOption chosenOption = ChosenOption.Invalid;
	
    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        Cursor.visible = false;
        Init();
    }
	
    // Update is called once per frame
    void Update () 
    {
        if (!hasControl && chosenOption != ChosenOption.Tutorial)
            return;

        if (!hasControl && chosenOption == ChosenOption.Tutorial)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hasControl = true;
                for(int i = 0; i < videoTutorialsFaders.Length; i++)
                {
                    videoTutorialsFaders[i].StartFade(FadeState.FadingOut, false);
                }
            }
            return;
        }

        int lastOption = currSelectedOption;
        int nextOption = GetDesiredOption();

        if (WantsToAcceptOption())
        {
            SelectOption(currSelectedOption);
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipWordCompleted, true);
        }
            

        if (nextOption == currSelectedOption)
            return;

        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipKeyTap, true);

        // fade out last option
        textFaders[lastOption].StartFade(FadeState.FadingOut, false);
        options[lastOption].color = colorDeselected;

        // fade in next option
        textFaders[nextOption].StartFade(FadeState.FadingIn, false);
        options[nextOption].color = colorSelected;

        // update current option
        currSelectedOption = nextOption;

        // activate proper cam
        //vcam.LookAt = options[currSelectedOption].gameObject.transform;
        vcams[lastOption].enabled = false;
        vcams[nextOption].enabled = true;
    }

    private void OnDestroy()
    {
        blackBg.FinishedFadingIn -= OnFinishFadingIn;
    }

    #endregion

    #region Methods

    private void Init()
    {
        vcams[DefaultOption].enabled = true;

        textFaders = new CanvasRendererSymmetricAlphaFader[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            textFaders[i] = options[i].GetComponent<CanvasRendererSymmetricAlphaFader>();
        }

        for (int i = 0; i < options.Length; i++)
        {
            if (i == DefaultOption)
            {
                textFaders[i].StartFade(FadeState.FadedIn, true);
                options[i].color = colorSelected;
            }
            else
            {
                textFaders[i].StartFade(FadeState.FadedOut, true);
                options[i].color = colorDeselected;
            }
        }

        videoTutorialsFaders = parentVideosObj.GetComponentsInChildren<CanvasRendererSymmetricAlphaFader>();

        blackBg.FinishedFadingIn += OnFinishFadingIn;
    }

    private void SelectOption(int option)
    {
        switch (option)
        {
            case 0:
                // tutorial
                GoTutorial();
                break;
            case 1:
                // game
                StartGame();
                break;
            case 2:
                // exit
                CloseApp();
                break;
            default:
                break;
        }
    }

    private void GoTutorial()
    {
        hasControl = false;

        chosenOption = ChosenOption.Tutorial;

        for (int i = 0; i < videoTutorialsFaders.Length; i++)
        {
            videoTutorialsFaders[i].StartFade(FadeState.FadingIn, false);
        }
    }

    private void StartGame()
    {
        chosenOption = ChosenOption.StartGame;
        hasControl = false;
        blackBg.StartFade(FadeState.FadingIn, false);
        musicFader.StartFade(FadeState.FadingOut, false);
    }

    private void CloseApp()
    {
        chosenOption = ChosenOption.ExitApp;
        hasControl = false;
        blackBg.StartFade(FadeState.FadingIn, false);
        musicFader.StartFade(FadeState.FadingOut, false);
    }

    private bool WantsToAcceptOption()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }

    private int GetDesiredOption()
    {
        int newOption = currSelectedOption;

        int dir = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            dir = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            dir = +1;

        newOption += dir;

        if (newOption >= options.Length)
            newOption = (newOption % options.Length);
        else if (newOption < 0)
            newOption = options.Length - 1;

        return newOption;
    }

    #endregion

    #region Callbacks

    protected virtual void OnFinishFadingIn(object source, System.EventArgs args)
    {
        switch (chosenOption)
        {
            case ChosenOption.Tutorial:
                //. ..
                break;
            case ChosenOption.StartGame:
                SceneManager.LoadScene("Level 1");
                break;
            case ChosenOption.ExitApp:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    #endregion
}
