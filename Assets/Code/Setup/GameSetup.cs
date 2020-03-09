using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the information related to the game, such as the number of players, the default controller type, etc...
/// </summary>
public class GameSetup : Singleton<GameSetup>
{
    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private const int totalPlayersInGame = 1;
    private PlayerSetup[] players = new PlayerSetup[totalPlayersInGame];
	
    #endregion
	
    #region Properties
	
    public PlayerSetup[] Players { get { return players; } }
	
    #endregion
	
    #region MonoBehaviour Methods
	
    // Use this for initialization
    private void Start () 
    {
        Init();
    }
	
    // Update is called once per frame
    private void Update () 
    {
		
    }
	
    #endregion
	
    #region Methods
	
    /// <summary>
    /// Initialize
    /// </summary>
    private void Init()
    {
        GenDefaultInfo();
    }

    /// <summary>
    /// Generate default information
    /// </summary>
    private void GenDefaultInfo()
    {
        // fill the player/s with default info
        for (int i = 0; i < players.Length; i++)
        {
            string name = "Player "+ i;
            int num = i;
            players[i] = new PlayerSetup(name, num);
        }
    }
	
    #endregion
}
