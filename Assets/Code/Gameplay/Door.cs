using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Brief description of the class here
/// </summary>
public class Door : MonoBehaviour
{
    #region Public Attributes

    public DescriptionText text = new DescriptionText();

    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private bool activated = false;
    private PlayableDirector director = null;
	
    #endregion
	
    #region Properties
	
    
   
    #endregion

    #region MonoBehaviour Methods
	
    // Use this for initialization
    void Start () 
    {
        director = GetComponent<PlayableDirector>();
        text.Init();
    }
	
    // Update is called once per frame
    void Update () 
    {
        text.Update(activated);
    }

    #endregion

    #region Methods

    public void ActivateDoorBehaviour()
    {
        activated = true;
    }

    public void DeactivateDoorBehaviour()
    {
        activated = false;
    }

        public void TriggerDoorOpening()
    {
        director.Play(director.playableAsset);
    }

    #endregion
}
