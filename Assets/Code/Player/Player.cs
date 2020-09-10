using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The states the player may be in
/// </summary>
public enum PlayerState
{
    Invalid = -1,

    Idle,
    MovingToTile,

    Count,
}

/// <summary>
/// The script that will hold most of the functionality of the player
/// </summary>
public class Player : MonoBehaviour
{
    #region Delegates

    public delegate void PlayerStartedMovingEventHandler(object source, System.EventArgs e);

    public event PlayerStartedMovingEventHandler PlayerStartedMoving;

    public delegate void PlayerJustStoppedEventHandler(object source, System.EventArgs e);

    public event PlayerJustStoppedEventHandler PlayerJustStopped;

    #endregion Delegates

    #region Public Attributes

    public Text secretsText = null;
    public GameObject explosionPrefab = null;
    public Transform playerVisuals = null;

    [Header("Snap to tile smoothness")]
    public float snapIdlePlayerToTileSmooth = 0.1f;

    [Header("Ground positioning")]
    public float groundOffset = 0.2f;

    [Header("Moving between tiles settings")]
    public float minTimeToWaitBeforeMovingAgain = 0.5f;

    public float timeToMoveBetweenTiles = 1.0f;
    public InterpolationFunction interpFunction = InterpolationFunction.SuperSmooth;

    [Header("UI")]
    public CanvasRendererSymmetricAlphaFader[] canvasFaders = null;

    public RawImage keySprite = null;
    public RawImage[] lifes = new RawImage[MaxLifes];

    #endregion

    #region Private Attributes

    private const int MaxLifes = 3;
    private PlayerState currState = PlayerState.Invalid;
    private float stateTimer = 0.0f;
    private Tile currTile = null;
    private Tile tileMovingTo = null;
    private int movingDir = -1;

    private Vector3 startPos = Vector3.zero;

    private Vector3 snapIdlePlayerToTileVel = Vector3.zero;
    private Vector3 moveFrom = Vector3.zero;
    private Vector3 moveTo = Vector3.zero;

    private float timeMovingToTile = 0.0f;

    private Vector3 posToLook = Vector3.zero;
    private bool rotating = false;
    private Quaternion visualsPlayerRot = Quaternion.identity;

    private Door currDoor = null;
    private bool hasKey = false;
    private float beforeMovingTimer = 0.0f;
    private float timeToWaitDoorToOpen = 0.5f;

    private Enemy enemyClashedWith = null;
    private bool shouldWaitBeforeMovingToTile = false;
    private bool disabled = false;

    #endregion

    #region Properties

    public Tile CurrTile { get { return currTile; } }

    public bool CanWriteToMove
    {
        get
        {
            return currState == PlayerState.Idle &&
                   stateTimer >= minTimeToWaitBeforeMovingAgain &&
                   GameManager.Instance.CurrState == GameState.InLevelPlaying &&
                   disabled == false;
        }
    }

    public bool CanMoveFwd { get { return currTile.FwdTile != null && (!currTile.blockedPath.doorFwd || hasKey); } }
    public bool CanMoveRight { get { return currTile.RightTile != null && (!currTile.blockedPath.doorRight || hasKey); } }
    public bool CanMoveBwd { get { return currTile.BwdTile != null && (!currTile.blockedPath.doorBwd || hasKey); } }
    public bool CanMoveLeft { get { return currTile.LeftTile != null && (!currTile.blockedPath.doorLeft || hasKey); } }

    public Tile TileMovingTo
    {
        get
        {
            return tileMovingTo;
        }

        set
        {
            tileMovingTo = value;
        }
    }

    public bool Disabled
    {
        get
        {
            return disabled;
        }

        set
        {
            disabled = value;
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
        float dt = Time.deltaTime;

        if (disabled)
            return;

        UpdateState(dt);
    }

    private void OnDestroy()
    {
        GameManager mgr = GameManager.Instance;
        if (mgr != null)
            mgr.HideDescription -= OnHideDescription;
    }

    private void OnTriggerEnter(Collider other)
    {
        // key

        Key key = other.GetComponent<Key>();

        if (key != null)
        {
            hasKey = true;

            ScaleFader fader = key.gameObject.GetComponent<ScaleFader>();
            fader.StartFade(FadeState.FadingOut, false);

            // play sfx
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipCatchKey, true);

            // show the UI key
            keySprite.gameObject.SetActive(true);
        }

        // enemy
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);

            Destroy(explosion, 8.0f);

            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipExplosion, true, enemy.transform.position);
            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipSkeletonDeath, true, enemy.transform.position);

            Destroy(enemy.gameObject);
            GameManager.Instance.Enemies.Remove(enemy);
        }

        // hint
        Hint hint = other.GetComponent<Hint>();

        if (hint != null)
        {
            int num = System.Convert.ToInt32(secretsText.text);
            int newNum = num + 1;
            secretsText.text = "" + newNum;

            SfxManager.Instance.PlaySfx(SfxManager.Instance.clipBip, true);
            Destroy(hint.gameObject);
        }

        // health

        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            if (GetCurrentLifes() == MaxLifes)
                return;

            TakeLife(+1);

            Destroy(health.gameObject);
        }
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Switch the current state for a new one
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="force"></param>
    public void SwitchState(PlayerState newState, bool force)
    {
        if (currState == newState && !force)
            return;

        switch (currState)
        {
            case PlayerState.Invalid:
                Debug.Log("Initializing Player from invalid state, ignore this if it happened when the game just starts");
                break;

            case PlayerState.Idle:
                ExitIdle();
                break;

            case PlayerState.MovingToTile:
                ExitMovingToTile();
                break;

            default:
                Debug.Log("No such state " + currState);
                break;
        }

        switch (newState)
        {
            case PlayerState.Idle:
                EnterIdle();
                break;

            case PlayerState.MovingToTile:
                EnterMovingToTile();
                break;

            default:
                Debug.Log("No such state " + newState);
                break;
        }

        stateTimer = 0.0f;
        currState = newState;
    }

    /// <summary>
    /// Called when the player exits the idle state
    /// </summary>
    private void ExitIdle()
    {
        moveFrom = transform.position;
        moveTo = CalcPlayerPositionOverTile(tileMovingTo);
    }

    /// <summary>
    /// Called when the player exits the moving to tile state
    /// </summary>
    private void ExitMovingToTile()
    {
        // reset timer
        timeMovingToTile = 0.0f;

        // update tiles
        currTile = tileMovingTo;
        tileMovingTo = null;

        OnPlayerJustStopped();
    }

    /// <summary>
    /// Called when the player enters the idle state
    /// </summary>
    private void EnterIdle()
    {
    }

    /// <summary>
    /// Called when the player enters the moving to tile state
    /// </summary>
    private void EnterMovingToTile()
    {
        OnPlayerStartedMoving();
    }

    /// <summary>
    /// Update according to the state we are in
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateState(float dt)
    {
        stateTimer += dt;

        switch (currState)
        {
            case PlayerState.Idle:
                UpdateIdle(dt);
                break;

            case PlayerState.MovingToTile:
                UpdateMovingToTile(dt);
                break;

            default:
                Debug.Log("No such state " + currState);
                break;
        }
    }

    /// <summary>
    /// Update the idle state
    /// </summary>
    private void UpdateIdle(float dt)
    {
        Vector3 pos = CalcPlayerPositionOverTile(currTile);
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref snapIdlePlayerToTileVel, snapIdlePlayerToTileSmooth);

        if (stateTimer < minTimeToWaitBeforeMovingAgain)
            return;

        bool shouldEnterMovingToTile = (tileMovingTo != null && !BlockedDir(movingDir));
        bool shouldEnterSolveDoorEnigma = (tileMovingTo != null && BlockedDir(movingDir));

        if (shouldEnterMovingToTile)
        {
            // is going to clash with an enemy? if so, we may want to join the
            // InLevelShowingDescription state of the game manager
            bool clashWithEnemy = IsGoingToClashWithEnemy(out enemyClashedWith);
            GameManager.Instance.EnemyEvent = clashWithEnemy;

            if (clashWithEnemy)
            {
                enemyClashedWith.TextActivated = true;
                GameManager.Instance.SetDescriptionText(enemyClashedWith.descriptionText.displayedText);
                GameManager.Instance.NextPlayerTile = tileMovingTo;
                GameManager.Instance.SwitchState(GameState.InLevelShowingDescription, false);
                tileMovingTo = null;
            }
            else
                SwitchState(PlayerState.MovingToTile, false);
        }
        else if (shouldEnterSolveDoorEnigma)
        {
            currDoor = GetDirDoor(movingDir);
            currDoor.ActivateDoorBehaviour();
            GameManager.Instance.SwitchState(GameState.InLevelShowingDescription, false);

            // get the door description and set the ui desc with it
            GameManager.Instance.SetDescriptionText(currDoor.text.displayedText);
            GameManager.Instance.NextPlayerTile = tileMovingTo;

            tileMovingTo = null;
        }
    }

    /// <summary>
    /// Update the moving to tile state
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateMovingToTile(float dt)
    {
        if (shouldWaitBeforeMovingToTile)
        {
            beforeMovingTimer += dt;

            if (beforeMovingTimer >= timeToWaitDoorToOpen)
            {
                shouldWaitBeforeMovingToTile = false;
                beforeMovingTimer = 0.0f;
                return;
            }
        }

        timeMovingToTile += dt;

        float t = timeMovingToTile / timeToMoveBetweenTiles;
        t = Mathf.Clamp01(t);

        float s = CustomInterpolation.Interpolate(t, interpFunction);
        moveTo = CalcPlayerPositionOverTile(tileMovingTo);

        Vector3 newPos = Vector3.Lerp(moveFrom, moveTo, s);
        transform.position = newPos;

        RotateVisualsTowardsNextTile(dt);

        if (t >= 1.0f)
            SwitchState(PlayerState.Idle, false);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialization
    /// </summary>
    private void Init()
    {
        //GameManager.Instance.ShowDescription += OnShowDescription;
        GameManager.Instance.HideDescription += OnHideDescription;

        InitStartTileAndPos();
        SwitchState(PlayerState.Idle, true);
        ShowWorldUI(false);
        FadeAvailableDirUI(FadeState.FadingIn);
        canvasFaders[0].FinishedFadingOut += OnWorldUIFinishedFadingOut;
    }

    /// <summary>
    /// Initialize the starting position
    /// </summary>
    private void InitStartTileAndPos()
    {
        TileManager tileManager = ObjRegistry.GetObj<TileManager>();

        if (tileManager != null)
        {
            Tile tile = tileManager.levelEntranceTile;
            currTile = tile;
            startPos = CalcPlayerPositionOverTile(tile);
        }

        transform.position = startPos;
        moveFrom = CalcPlayerPositionOverTile(currTile);
    }

    /// <summary>
    /// Calculate the position the player should have over a tile to not clip and not be too high
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private Vector3 CalcPlayerPositionOverTile(Tile tile)
    {
        Vector3 pos = Vector3.zero;

        pos = tile.transform.position + (tile.transform.up * (tile.distanceToSide + groundOffset));

        return pos;
    }

    /// <summary>
    /// Fade the available arrow directions
    /// </summary>
    /// <param name="newState"></param>
    private void FadeAvailableDirUI(FadeState newState)
    {
        if (CanMoveFwd)
        {
            canvasFaders[0].StartFade(newState, false);
            canvasFaders[4].StartFade(newState, false);
        }
        if (CanMoveRight)
        {
            canvasFaders[1].StartFade(newState, false);
            canvasFaders[5].StartFade(newState, false);
        }
        if (CanMoveBwd)
        {
            canvasFaders[2].StartFade(newState, false);
            canvasFaders[6].StartFade(newState, false);
        }
        if (CanMoveLeft)
        {
            canvasFaders[3].StartFade(newState, false);
            canvasFaders[7].StartFade(newState, false);
        }
    }

    /// <summary>
    /// Set the arrows visibility
    /// </summary>
    /// <param name="on"></param>
    private void ShowWorldUI(bool on)
    {
        if (on)
        {
            for (int i = 0; i < canvasFaders.Length; i++)
                canvasFaders[i].StartFade(FadeState.FadingIn, true);
        }
        else
        {
            for (int i = 0; i < canvasFaders.Length; i++)
                canvasFaders[i].StartFade(FadeState.FadingOut, true);
        }
    }

    /// <summary>
    /// Move to a tile, the player won't move if it's not in idle state (dir is a number between 0
    /// and 3, 0 fwd, 1 right, 2 bwd, 3 left)
    /// </summary>
    /// <param name="dir"></param>
    public void MoveTo(int dir)
    {
        if (currState != PlayerState.Idle)
            return;

        movingDir = dir;

        tileMovingTo = GetTileAccordingToDir(dir);
    }

    /// <summary>
    /// Get a tile according to a direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Tile GetTileAccordingToDir(int dir)
    {
        Tile tile = null;

        switch (dir)
        {
            case 0:
                tile = currTile.FwdTile;
                break;

            case 1:
                tile = currTile.RightTile;
                break;

            case 2:
                tile = currTile.BwdTile;
                break;

            case 3:
                tile = currTile.LeftTile;
                break;

            default:
                Debug.Log("This dir is not allowed, it must be between 0 and 3: " + dir);
                break;
        }

        return tile;
    }

    private bool BlockedDir(int dir) // < 0 fwd, 1 right, 2 bwd, 3 left
    {
        bool blocked = false;

        switch (dir)
        {
            case 0:
                blocked = (currTile.blockedPath.doorFwd != null);
                break;

            case 1:
                blocked = (currTile.blockedPath.doorRight != null);
                break;

            case 2:
                blocked = (currTile.blockedPath.doorBwd != null);
                break;

            case 3:
                blocked = (currTile.blockedPath.doorLeft != null);
                break;

            default:
                Debug.Log("This dir is not allowed, it must be between 0 and 3: " + dir);
                break;
        }

        return blocked;
    }

    /// <summary>
    /// Gets the door of a direction, null if none
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Door GetDirDoor(int dir)
    {
        Door door = null;

        switch (dir)
        {
            case 0:
                door = currTile.blockedPath.doorFwd;
                break;

            case 1:
                door = currTile.blockedPath.doorRight;
                break;

            case 2:
                door = currTile.blockedPath.doorBwd;
                break;

            case 3:
                door = currTile.blockedPath.doorLeft;
                break;

            default:
                Debug.Log("This dir is not allowed, it must be between 0 and 3: " + dir);
                break;
        }

        return door;
    }

    /// <summary>
    /// Face the player body to the tile we are moving to
    /// </summary>
    private void FaceVisualsToMovingTile()
    {
        rotating = true;
        posToLook = CalcPlayerPositionOverTile(tileMovingTo);
        posToLook.y = playerVisuals.position.y;

        Vector3 dirToPos = (posToLook - playerVisuals.position).normalized;
        dirToPos *= 2.0f;
        visualsPlayerRot = Quaternion.LookRotation(dirToPos);
    }

    /// <summary>
    /// Rotate towards the next tile
    /// </summary>
    /// <param name="dt"></param>
    private void RotateVisualsTowardsNextTile(float dt)
    {
        if (!rotating)
            return;

        playerVisuals.rotation = Quaternion.Slerp(playerVisuals.rotation, visualsPlayerRot, 4.0f * dt);
    }

    /// <summary>
    /// make the door to which the moving dir of the char is to be null
    /// </summary>
    /// <param name="dir"></param>
    private void NuliffyCurrTileDoor(int dir)
    {
        switch (dir)
        {
            case 0:
                currTile.blockedPath.doorFwd = null;
                break;

            case 1:
                currTile.blockedPath.doorRight = null;
                break;

            case 2:
                currTile.blockedPath.doorBwd = null;
                break;

            case 3:
                currTile.blockedPath.doorLeft = null;
                break;

            default:
                Debug.Log("This dir is not allowed, it must be between 0 and 3: " + dir);
                break;
        }
    }

    /// <summary>
    /// Checks if the player is going to clash with an enemy when both arrive the next supposed tile
    /// they should be
    /// </summary>
    private bool IsGoingToClashWithEnemy(out Enemy enemyClashedWith)
    {
        bool clash = false;
        enemyClashedWith = null;

        List<Enemy> enemies = GameManager.Instance.Enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            clash = (enemies[i].NextTile == tileMovingTo);

            if (clash)
            {
                enemyClashedWith = enemies[i];
                break;
            }
        }

        return clash;
    }

    /// <summary>
    /// Take or receive health or damage
    /// </summary>
    /// <param name="numLifes"></param>
    public void TakeLife(int numLifes)
    {
        int currLifes = GetCurrentLifes();

        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipBip, true);

        SetLife(currLifes + numLifes);
    }

    private void SetLife(int life)
    {
        if (life <= 0)
        {
            playerVisuals.gameObject.SetActive(false);
            GameManager.Instance.SwitchState(GameState.GameOverByDying, false);
            return;
        }

        for (int i = 0; i < life; i++)
        {
            lifes[i].enabled = true;
        }

        for (int i = life; i < lifes.Length; i++)
        {
            lifes[i].enabled = false;
        }
    }

    /// <summary>
    /// Gety the current lifes
    /// </summary>
    /// <returns></returns>
    public int GetCurrentLifes()
    {
        int life = 0;

        for (int i = 0; i < lifes.Length; i++)
        {
            if (lifes[i].enabled)
                life++;
        }

        return life;
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called once when the world UI finishes fading
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected virtual void OnWorldUIFinishedFadingOut(object source, System.EventArgs args)
    {
        // not strictly correct but i'm being too much perfectionist perhaps, each faded element
        // should have an id to only reset its own state
        PlayerWordWriter writer = GetComponent<PlayerWordWriter>();
        writer.ResetAvailableDirTexts();
    }

    /// <summary>
    /// Called when the player starts moving to the next tile
    /// </summary>
    protected virtual void OnPlayerStartedMoving()
    {
        if (PlayerStartedMoving != null)
            PlayerStartedMoving(this, System.EventArgs.Empty);

        FadeAvailableDirUI(FadeState.FadingOut);
        SfxManager.Instance.PlaySfx(SfxManager.Instance.clipSlide, true);

        FaceVisualsToMovingTile();
    }

    /// <summary>
    /// Called once when the player just stops on the tile he wanted to move to
    /// </summary>
    protected virtual void OnPlayerJustStopped()
    {
        if (PlayerJustStopped != null)
            PlayerJustStopped(this, System.EventArgs.Empty);

        rotating = false;
        FadeAvailableDirUI(FadeState.FadingIn);

        if (currTile == GameManager.Instance.exitTile)
        {
            GameManager.Instance.SwitchState(GameState.GameOverByWin, false);
        }
    }

    /// <summary>
    /// Called right before showing a description
    /// </summary>
    protected virtual void OnShowDescription(object source, System.EventArgs args)
    {
    }

    /// <summary>
    /// Called right before hiding the description
    /// </summary>
    protected virtual void OnHideDescription(object source, System.EventArgs args)
    {
        if (!GameManager.Instance.EnemyEvent)
        {
            // play the anim
            currDoor.TriggerDoorOpening();

            // unlock the path
            currDoor.DeactivateDoorBehaviour();
            currDoor = null;
            NuliffyCurrTileDoor(movingDir);

            //remove ui key at some point
            hasKey = false;
            keySprite.gameObject.SetActive(false);
        }
        else
        {
            enemyClashedWith.TextActivated = false;
            enemyClashedWith = null;
        }

        GameManager.Instance.EnemyEvent = false;
    }

    #endregion
}