using UnityEngine;

public enum EnemyState
{
    Invalid = -1,

    Idle,
    Moving,

    Count
}

/// <summary>
/// The enemies that move at the same time as the players in a given path that must be entered in order
/// </summary>
public class Enemy : MonoBehaviour
{
    #region Public Attributes

    public bool preventFromFacingNextTile = false;

    public Tile startTile = null;
    public Tile[] path = null;
    public bool loopablePath = false;

    public bool startsTravelingForward = true;
    public float groundOffset = 2.0f;

    public InterpolationFunction movingFunction = InterpolationFunction.EaseInOutCubic;
    public DescriptionText descriptionText = null;

    #endregion

    #region Private Attributes

    private bool isTravelingForward = true;
    private int currPathPoint = -1;
    private EnemyState currState = EnemyState.Idle;

    private float timer = 0.0f;
    private float timeToMoveBetweenTiles = -1.0f;
    private bool textActivated = false;

    #endregion

    #region Properties

    public Tile NextTile { get { return GetNextTile(); } }

    public bool TextActivated
    {
        get
        {
            return textActivated;
        }

        set
        {
            textActivated = value;
        }
    }

    #endregion

    #region MonoBehaviour Methods

    private void Start()
    {
        Init();

        GameManager.Instance.TheGamePlayer.PlayerStartedMoving += OnPlayerStartedMoving;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        UpdateState(dt);

        descriptionText.Update(textActivated);
    }

    private void Destroy()
    {
        GameManager mgr = GameManager.Instance;

        if (mgr != null)
            GameManager.Instance.TheGamePlayer.PlayerStartedMoving -= OnPlayerStartedMoving;
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Enter the idle state
    /// </summary>
    private void EnterIdle()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Enter the moving state
    /// </summary>
    private void EnterMoving()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Swap the current state with the new one
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="force"></param>
    private void SwitchState(EnemyState newState, bool force)
    {
        if (newState == currState && !force)
            return;

        timer = 0.0f;

        switch (newState)
        {
            case EnemyState.Idle:
                EnterIdle();
                break;

            case EnemyState.Moving:
                EnterMoving();
                break;

            default:
                Debug.Log("No such state : " + newState);
                break;
        }

        currState = newState;
    }

    /// <summary>
    /// Update the idle state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateIdle(float dt)
    {
        transform.position = CalcPositionOverTile(path[currPathPoint]);

        if (preventFromFacingNextTile)
            return;
        FaceToNextTile(false, dt);
    }

    /// <summary>
    /// Update the moving state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateMoving(float dt)
    {
        float t = timer / timeToMoveBetweenTiles;
        t = Mathf.Clamp01(t);

        float s = CustomInterpolation.Interpolate(t, movingFunction);

        Vector3 initPos = CalcPositionOverTile(path[currPathPoint]);

        Tile nextTile = GetNextTile();
        Vector3 endPos = CalcPositionOverTile(nextTile);

        Vector3 newPos = Vector3.Lerp(initPos, endPos, s);
        transform.position = newPos;

        if (t >= 1.0f)
        {
            // exit moving
            SwitchState(EnemyState.Idle, false);
            RefreshCurrentTile(GetNextTile());
        }
    }

    /// <summary>
    /// Update the state
    /// </summary>
    private void UpdateState(float dt)
    {
        timer += dt;

        switch (currState)
        {
            case EnemyState.Idle:
                UpdateIdle(dt);
                break;

            case EnemyState.Moving:
                UpdateMoving(dt);
                break;

            default:
                Debug.Log("No such state : " + currState);
                break;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        if (path == null)
            return;

        descriptionText.Init();

        timeToMoveBetweenTiles = 1.0f;

        // set the proper index for the starting tile
        RefreshCurrentTile(startTile);

        isTravelingForward = startsTravelingForward;

        // set the starting position
        transform.position = CalcPositionOverTile(startTile);

        // face to the next tile
        FaceToNextTile(true, Time.deltaTime);
    }

    /// <summary>
    /// Refresh the current tile
    /// </summary>
    /// <param name="newTile"></param>
    private void RefreshCurrentTile(Tile newTile)
    {
        int index = -1;

        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == newTile)
            {
                currPathPoint = i;
                index = i;
                break;
            }
        }

        if (loopablePath)
            return;

        bool reachedEndOfPath = false;

        if (isTravelingForward)
            reachedEndOfPath = (path.Length - 1 == index);
        else
            reachedEndOfPath = (0 == index);

        isTravelingForward = reachedEndOfPath ? !isTravelingForward : isTravelingForward;
    }

    /// <summary>
    /// Get the next tile in the path
    /// </summary>
    /// <returns></returns>
    private Tile GetNextTile()
    {
        Tile next = null;

        // get the next index and check which direction we are going
        int nextIndex = isTravelingForward ? (currPathPoint + 1) : (currPathPoint - 1);

        if (loopablePath)
            nextIndex = (currPathPoint + 1) % path.Length;

        // finally return the corresponding tile
        next = path[nextIndex];
        return next;
    }

    /// <summary>
    /// Face to the next tile
    /// </summary>
    private void FaceToNextTile(bool forceInstant, float dt)
    {
        Tile nextTile = GetNextTile();

        Vector3 nextTilePos = nextTile.transform.position;
        nextTilePos.y = transform.position.y;

        Vector3 dirToLook = (nextTilePos - transform.position).normalized;

        Quaternion rot = Quaternion.identity;

        if (!forceInstant)
        {
            rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToLook), 2.25f * dt);
            transform.rotation = rot;
        }
        else
        {
            transform.forward = (nextTilePos - transform.position).normalized;
        }
    }

    /// <summary>
    /// Get the position over a tile (with the offsetted Y to avoid clipping)
    /// </summary>
    /// <param name="overTile"></param>
    /// <returns></returns>
    private Vector3 CalcPositionOverTile(Tile overTile)
    {
        return overTile.transform.position + (Vector3.up * (overTile.distanceToSide + groundOffset));
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called when the player starts moving
    /// </summary>
    protected virtual void OnPlayerStartedMoving(object source, System.EventArgs args)
    {
        SwitchState(EnemyState.Moving, false);
    }

    #endregion
}