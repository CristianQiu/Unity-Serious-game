using UnityEngine;

/// <summary>
/// A manager for the cube tiles to ease accessing to some info more coherently
/// </summary>
public class TileManager : MonoBehaviour
{
    #region Public Attributes

    public Tile levelEntranceTile = null;
    public Tile levelExitTile = null;

    #endregion

    #region Private Attributes

    private Tile[] tiles = null;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        ObjRegistry.RegisterObj(this);
    }

    // Use this for initialization
    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        ObjRegistry.UnregisterObj(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        tiles = GetComponentsInChildren<Tile>();
        InitEntranceAndExit();
    }

    /// <summary>
    /// Initialize the entrance and the exiting tiles
    /// </summary>
    private void InitEntranceAndExit()
    {
        Debug.Assert(levelEntranceTile != null && levelExitTile != null, "There must be an starting tile and exiting tile in the scene");

        for (int i = 0; i < tiles.Length; i++)
        {
            if (levelEntranceTile == tiles[i])
                tiles[i].IsEntrance = true;
            else if (levelExitTile == tiles[i])
                tiles[i].IsExit = true;
        }
    }

    #endregion
}