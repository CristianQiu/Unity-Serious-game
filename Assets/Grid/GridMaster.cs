using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class that stores the grid nodes and manages them to build a grid. It can also handle their destroying, to recreate new grids,
/// as well as the obstacles baking so that nodes are set to be walkable or not
/// </summary>
public class GridMaster : MonoBehaviour
{
    #region Public Attributes

    [HideInInspector]
    public float nodeDiameter = 1.0f;

    [HideInInspector]
    public int numRows = 5;
    [HideInInspector]
    public int numCols = 5;

    [HideInInspector]
    public string obstacleLayerName = "GridObstacle";
    [HideInInspector]
    public string thinWallLayerName = "GridThinWall";

#if UNITY_EDITOR

    [HideInInspector]
    public Color walkableColor = new Color(0.0f, 1.0f, 0.0f, 0.2f);
    [HideInInspector]
    public Color notWalkableColor = new Color(1.0f, 0.0f, 0.0f, 0.2f);
    [HideInInspector]
    public Color hasAlignedWallAtSomeSideColor = new Color(0.0f, 0.0f, 1.0f, 0.2f);

#endif

    #endregion

    #region Private Attributes

    private GridNode[][] nodes = null;

    private bool hasGridBeenBuilt = false;

    private LayerMask obstacleMask;
    private LayerMask thinWallMask;

    #endregion

    #region Properties

    public GridNode[][] Nodes
    {
        get { return nodes; }
        set { nodes = value; }
    }

    public bool HasGridBeenBuilt { get { return hasGridBeenBuilt; } }

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        //ObjectTracker.RegisterObject(this);
    }

    private void Start()
    {
        CreateGrid(nodeDiameter, numRows, numCols);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float t = 0.0f;
            Vector3 pos = Vector3.zero; // Shared.Maths.MathUtils.RayPlaneIntersection(ray, Vector3.up, transform.position, out t);

            UnityEngine.Profiling.Profiler.BeginSample("Pathfinding");

            currPath = GridPathFinder.FindPath(Vector3.zero, pos);

            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    private void OnDestroy()
    {
        //ObjectTracker.UnregisterObject(this);
    }

    private List<Vector3> currPath = new List<Vector3>();

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || currPath == null || currPath.Count <= 0)
            return;

        float r = nodeDiameter * 0.05f;

        for (int i = 0; i < currPath.Count; i++)
        {
            if (i == currPath.Count - 1)
                return;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(currPath[i], currPath[i + 1]);
            Gizmos.DrawSphere(currPath[i], r);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create and initialize the whole grid given the needed parameters, note that it initializes each node neighbors
    /// and does the obstacle baking for each node on the fly, at the same time it's created
    /// </summary>
    /// <param name="nodeDiameter"></param>
    /// <param name="numRows"></param>
    /// <param name="numCols"></param>
    public void CreateGrid(float nodeDiameter, int numRows, int numCols)
    {
        if (hasGridBeenBuilt)
            DestroyGrid();

        BuildMasks();
        GenerateGrid(numRows, numCols, nodeDiameter);
        InitNeighbors();
    }

    /// <summary>
    /// Initialize the masks to build the grid features such as the obstacles or thin walls
    /// </summary>
    public void BuildMasks()
    {
        obstacleMask = LayerMask.GetMask(obstacleLayerName);
        thinWallMask = LayerMask.GetMask(thinWallLayerName);
    }

    /// <summary>
    /// Function in charge of baking the obstacles in case we need to update an existing grid,
    /// which means setting each tile to be walkable or not
    /// </summary>
    public void BakeObstacles()
    {
        float nodeRadius = nodeDiameter * 0.5f;
        // maybe we want different height...

        Vector3 halfExtents = new Vector3(nodeRadius * 0.85f, nodeRadius * 0.85f, nodeRadius * 0.85f);

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                GridNode n = nodes[i][j];
                RaycastHit[] hitInfos = new RaycastHit[1];

                // cast a box from each tile
                if (Physics.BoxCastNonAlloc(n.Pos, halfExtents, Vector3.up, hitInfos, Quaternion.identity, nodeRadius, obstacleMask, QueryTriggerInteraction.Collide) > 0)
                    n.Walkable = false;
                else
                    n.Walkable = true;
            }
        }
    }

    /// <summary>
    /// Calculate the node corresponding to a position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GridNode PosToGridNode(Vector3 pos)
    {
        int i = Mathf.FloorToInt(pos.z / nodeDiameter);
        int j = Mathf.FloorToInt(pos.x / nodeDiameter);

        GridNode n = null;

        if (i >= 0 && j >= 0 && i < numRows && j < numCols)
            n = nodes[i][j];

        return n;
    }

    /// <summary>
    /// Generates the grid with the given parameters, note that it also bakes obstacles
    /// </summary>
    /// <param name="numRows"></param>
    /// <param name="numCols"></param>
    /// <param name="nodeDiameter"></param>
    private void GenerateGrid(int numRows, int numCols, float nodeDiameter)
    {
        // intialize sizes
        nodes = new GridNode[numRows][];

        for (int i = 0; i < numRows; i++)
            nodes[i] = new GridNode[numCols];

        // precalculate the radius used inside the for loop
        float nodeRadius = nodeDiameter * 0.5f;

        // and prepare variables so we can bake obstacles at the same time we create nodes
        Vector3 halfExtents = new Vector3(nodeRadius, nodeRadius, nodeRadius);

        // we're going to set a unique id for each node
        int nodeId = 0;

        // now populate the grid
        for (int i = 0; i < numRows; i++)
        {
            // snap the left bottom corner of the first node with the world 0.0.0. The Zpos is actually shared among all the columns of the same row
            float Zpos = nodeRadius + (i * nodeDiameter);

            for (int j = 0; j < numCols; j++)
            {
                float Xpos = nodeRadius + (j * nodeDiameter);

                Vector3 pos = new Vector3(Xpos, 0.0f, Zpos);
                nodes[i][j] = new GridNode(nodeId, i, j, pos, true);

                // now bake the obstacles
                RaycastHit[] hitInfos = new RaycastHit[1];

                if (Physics.BoxCastNonAlloc(pos, halfExtents, Vector3.up, hitInfos, Quaternion.identity, nodeRadius, obstacleMask, QueryTriggerInteraction.Collide) > 0)
                    nodes[i][j].Walkable = false;
                else
                    nodes[i][j].Walkable = true;

                // increase the id
                nodeId++;
            }
        }

        hasGridBeenBuilt = true;
    }

    /// <summary>
    /// Initialize the neighbors of each node
    /// </summary>
    private void InitNeighbors()
    {
        // initialize useful variables used inside the loop
        float nodeRadius = nodeDiameter * 0.5f;

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                GridNode currNode = nodes[i][j];

                // we'll skip not walkable nodes, as they don't really need neighbors, this may be somehow changed in case we had "dynamic" obstacles
                if (!currNode.Walkable)
                    continue;

                int aboveRow = i + 1;
                int belowRow = i - 1;

                int leftCol = j - 1;
                int rightCol = j + 1;

                GridNode neighbor = null;

                // check for the next row same col neighbor
                if (aboveRow < numRows)
                {
                    neighbor = nodes[aboveRow][j];

                    // if it's walkable and no thin wall is in between add it to the neighbors list
                    if (neighbor.Walkable && !HasAlignedWallInBetween(currNode, neighbor, nodeRadius, thinWallMask))
                        currNode.Neighbors.Add(neighbor);
                }

                // do the same for the rest of the potential neighbors

                if (belowRow >= 0)
                {
                    neighbor = nodes[belowRow][j];

                    if (neighbor.Walkable && !HasAlignedWallInBetween(currNode, neighbor, nodeRadius, thinWallMask))
                        currNode.Neighbors.Add(neighbor);
                }

                if (leftCol >= 0)
                {
                    neighbor = nodes[i][leftCol];

                    if (neighbor.Walkable && !HasAlignedWallInBetween(currNode, neighbor, nodeRadius, thinWallMask))
                        currNode.Neighbors.Add(neighbor);
                }

                if (rightCol < numCols)
                {
                    neighbor = nodes[i][rightCol];

                    if (neighbor.Walkable && !HasAlignedWallInBetween(currNode, neighbor, nodeRadius, thinWallMask))
                        currNode.Neighbors.Add(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Checks if two nodes that "are neighbors" are separated by a thin wall
    /// </summary>
    /// <param name="currNode"></param>
    /// <param name="neighborNode"></param>
    /// <param name="nodeRadius"></param>
    /// <param name="thinWallMask"></param>
    /// <returns></returns>
    private bool HasAlignedWallInBetween(GridNode currNode, GridNode neighborNode, float nodeRadius, LayerMask thinWallMask)
    {
        bool hasAlignedWallInBetween = false;

        // build the needed info to raycast
        Vector3 dir = (neighborNode.Pos - currNode.Pos).normalized;
        Ray ray = new Ray(currNode.Pos, dir);
        RaycastHit[] hitInfos = new RaycastHit[1];

        // now check if there's a thin wall in between
        if (Physics.RaycastNonAlloc(ray, hitInfos, nodeRadius, thinWallMask, QueryTriggerInteraction.Collide) > 0)
        {
#if UNITY_EDITOR
            hasAlignedWallInBetween = true;
#endif
            currNode.HasAlignedWallAtSomeSide = true;
        }

        return hasAlignedWallInBetween;
    }

    /// <summary>
    /// Destroy the grid
    /// </summary>
    private void DestroyGrid()
    {
        nodes = null;
        hasGridBeenBuilt = false;
    }

    #endregion
}