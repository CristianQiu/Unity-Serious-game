/// <summary>
/// The base state machine to inherit from (to use with BaseStates)
/// </summary>
public abstract class BaseStateMachine
{
    #region Protected Attributes

    protected BaseState[] states;

    #endregion

    #region Private Attributes

    private int currState;
    private bool initialized;

    #endregion

    #region Properties

    public abstract int InitialState { get; }
    public abstract int NumStates { get; }

    public int CurrState
    {
        get
        {
            return currState;
        }

        set
        {
            currState = value;
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="numStates"></param>
    public BaseStateMachine()
    {
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="numStates"></param>
    protected virtual void Init()
    {
        if (initialized)
            return;

        states = new BaseState[NumStates];

        FillCustomStates();
        currState = InitialState;

        initialized = true;
    }

    #endregion

    #region BaseStateMachine Methods

    /// <summary>
    /// Update loop of the state machine
    /// </summary>
    /// <param name="dt"></param>
    public virtual void Update(float dt)
    {
        if (!initialized)
            Init();


        // update current state
        states[currState].Update(dt);

        // check if current state wants to change state
        int nextState = states[currState].MustChangeState();

        // if so then call the exit method of the current state and the enter method of the next state
        if (currState != nextState)
        {
            states[currState].ExitState();
            states[nextState].EnterState();
        }

        // and update the state index
        currState = nextState;
    }


    /// <summary>
    /// Should be filled to fill the array of BaseStates with custom states that inherit from BaseState
    /// </summary>
    protected abstract void FillCustomStates();

    #endregion
}
