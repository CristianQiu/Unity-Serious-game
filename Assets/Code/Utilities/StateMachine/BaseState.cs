/// <summary>
/// The base state to inherit from
/// </summary>
public abstract class BaseState
{
    #region Protected Attributes

    // the time the state has been active
    protected float timer;

    // the state machine that owns this state
    protected BaseStateMachine stateMachine;

    #endregion

    #region Properties

    // the identifier of the state. It should be associated to one state in the stateMachine states
    // enumeration !
    public abstract int TypeIdentifier { get; }

    #endregion

    #region Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="stateMachine"></param>
    public BaseState(BaseStateMachine stateMachine)
    {
        Init(stateMachine);
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="stateMachine"></param>
    protected virtual void Init(BaseStateMachine stateMachine)
    {
        timer = 0.0f;
        this.stateMachine = stateMachine;
    }

    #endregion

    #region BaseState Methods

    /// <summary>
    /// Called once when entering this state
    /// </summary>
    public virtual void EnterState()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// The update loop
    /// </summary>
    /// <param name="dt"></param>
    public virtual void Update(float dt)
    {
        timer += dt;
    }

    /// <summary>
    /// Called once when exiting this state
    /// </summary>
    public virtual void ExitState()
    {
        timer = 0.0f;
    }

    /// <summary>
    /// Checks if we should change the state and if so it should return the next state identifier,
    /// otherwise it returns its own state identifier
    /// </summary>
    /// <returns></returns>
    public abstract int MustChangeState();

    #endregion
}