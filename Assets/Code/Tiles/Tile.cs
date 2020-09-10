using UnityEngine;

/// <summary>
/// The basic floor unit of our game
/// </summary>
public class Tile : MonoBehaviour
{
    [System.Serializable]
    public class DoorBlock
    {
        public Door doorFwd = null;
        public Door doorRight = null;
        public Door doorBwd = null;
        public Door doorLeft = null;
    }

    #region Public Attributes

    [Header("Blocked paths")]
    public DoorBlock blockedPath = new DoorBlock();

    [Header("Words configuration")]
    public TileWords tileWords = new TileWords();

    [Header("Configuration")]
    public float distanceToSide = 3.5f;

    [Header("Debug")]
    public Color defaultColor = new Color(11.0f, 155.0f, 255.0f, 255.0f);

    public Color highlightedAsNeighbourColor = Color.white;
    public bool showDirections = false;
    public bool highlightNeighbours = false;

    #endregion

    #region Protected Attributes

    #endregion

    #region Private Attributes

    private bool isEntrance = false;
    private bool isExit = false;

    // neighbours
    private Tile fwdTile = null;

    private Tile bwdTile = null;
    private Tile rightTile = null;
    private Tile leftTile = null;

    private bool beingHighlightedAsNeighbour = false;

    private MeshRenderer meshRend = null;

    #endregion

    #region Properties

    public bool IsEntrance
    {
        get { return isEntrance; }
        set { isEntrance = value; }
    }

    public bool IsExit
    {
        get { return isExit; }
        set { isExit = value; }
    }

    // neighbours
    public Tile FwdTile { get { return fwdTile; } }

    public Tile BwdTile { get { return bwdTile; } }
    public Tile RightTile { get { return rightTile; } }
    public Tile LeftTile { get { return leftTile; } }

    public bool BeingHighlightedAsNeighbour
    {
        get { return beingHighlightedAsNeighbour; }
        set
        {
            if (beingHighlightedAsNeighbour == value)
                return;

            beingHighlightedAsNeighbour = value;

            // switch the material color if needed
            if (beingHighlightedAsNeighbour)
                meshRend.material.color = highlightedAsNeighbourColor;
            else
                meshRend.material.color = defaultColor;
        }
    }

    #endregion

    #region MonoBehaviour Methods

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        ShowDebug();
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// Call all the debugging methods
    /// </summary>
    private void ShowDebug()
    {
        ShowTileDirections();
        ShowNeighbourTiles();
    }

    /// <summary>
    /// Show the tile directions (fwd, bwd, right, left)
    /// </summary>
    private void ShowTileDirections()
    {
        if (!showDirections)
            return;

        DebugUtils.DrawArrow(transform.parent.position + Vector3.up * distanceToSide, transform.parent.forward * distanceToSide, 0.2f, Color.red);
        DebugUtils.DrawArrow(transform.parent.position + Vector3.up * distanceToSide, -transform.parent.forward * distanceToSide, 0.2f, Color.red);
        DebugUtils.DrawArrow(transform.parent.position + Vector3.up * distanceToSide, transform.parent.right * distanceToSide, 0.2f, Color.red);
        DebugUtils.DrawArrow(transform.parent.position + Vector3.up * distanceToSide, -transform.parent.right * distanceToSide, 0.2f, Color.red);

        Vector3 heightOffset = Vector3.up * distanceToSide * 1.25f;
        Vector3 elevatedOwnPos = transform.parent.position + heightOffset;

        if (fwdTile != null)
            DebugUtils.DrawArrow(elevatedOwnPos, (fwdTile.transform.parent.position + heightOffset) - elevatedOwnPos, 0.2f, Color.green);

        if (bwdTile != null)
            DebugUtils.DrawArrow(elevatedOwnPos, (bwdTile.transform.parent.position + heightOffset) - elevatedOwnPos, 0.2f, Color.green);

        if (rightTile != null)
            DebugUtils.DrawArrow(elevatedOwnPos, (rightTile.transform.parent.position + heightOffset) - elevatedOwnPos, 0.2f, Color.green);

        if (leftTile != null)
            DebugUtils.DrawArrow(elevatedOwnPos, (leftTile.transform.parent.position + heightOffset) - elevatedOwnPos, 0.2f, Color.green);
    }

    /// <summary>
    /// Show the neighbour tiles
    /// </summary>
    private void ShowNeighbourTiles()
    {
        // TODO: fix this: now tiles will never go back to their default color, but with the else in false some of them
        // are forced to be not highlighted because a tile can be highlighted by 1 tile but may be
        // turned off by another one
        if (highlightNeighbours)
            HighLightNeighbourTiles(true);
        //else
        //    HighLightNeighbourTiles(false);
    }

    /// <summary>
    /// Set the state of the neighbours to be shown as highlighted or not
    /// </summary>
    /// <param name="state"></param>
    private void HighLightNeighbourTiles(bool state)
    {
        if (fwdTile != null)
            fwdTile.BeingHighlightedAsNeighbour = state;

        if (bwdTile != null)
            bwdTile.BeingHighlightedAsNeighbour = state;

        if (rightTile != null)
            rightTile.BeingHighlightedAsNeighbour = state;

        if (leftTile != null)
            leftTile.BeingHighlightedAsNeighbour = state;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        meshRend = GetComponent<MeshRenderer>();
        InitNeighbours();
    }

    /// <summary>
    /// Initialize all the neighbours tiles
    /// </summary>
    private void InitNeighbours()
    {
        Vector3 fromHeight = transform.parent.position;
        fromHeight.y += distanceToSide;

        // forward
        Vector3 fromPos = fromHeight + (transform.parent.forward * distanceToSide);
        fwdTile = GetRaycastedNeighBourTile(fromPos, Vector3.down, gameObject.layer);

        // backward
        fromPos = fromHeight + (-transform.parent.forward * distanceToSide);
        bwdTile = GetRaycastedNeighBourTile(fromPos, Vector3.down, gameObject.layer);

        // right
        fromPos = fromHeight + (transform.parent.right * distanceToSide);
        rightTile = GetRaycastedNeighBourTile(fromPos, Vector3.down, gameObject.layer);

        // left
        fromPos = fromHeight + (-transform.parent.right * distanceToSide);
        leftTile = GetRaycastedNeighBourTile(fromPos, Vector3.down, gameObject.layer);
    }

    /// <summary>
    /// Launch a raycast from a position with a direction and mask to get the tile collided with
    /// </summary>
    /// <param name="fromPos"></param>
    /// <param name="dir"></param>
    /// <param name="tileMask"></param>
    /// <returns></returns>
    private Tile GetRaycastedNeighBourTile(Vector3 fromPos, Vector3 dir, LayerMask tileMask)
    {
        dir.Normalize();
        Tile tileFound = null;

        Ray ray = new Ray(fromPos, dir);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 10.0f, ~tileMask))
            tileFound = hitInfo.collider.GetComponentInChildren<Tile>();

        return tileFound;
    }

    #endregion
}