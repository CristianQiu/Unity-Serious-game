using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all the relevant info for a single player
/// </summary>
public class PlayerSetup
{
    #region Constructor

    public PlayerSetup(string name, int playerNum)
    {
        this.name = name;
        this.playerNum = playerNum;
    }

    #endregion

    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private string name = null;
    private int playerNum = 1;
	
    #endregion
	
    #region Properties
	
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int PlayerNum
    {
        get { return playerNum; }
        set { playerNum = value; }
    }
   
    #endregion

    #region Methods



    #endregion
}
