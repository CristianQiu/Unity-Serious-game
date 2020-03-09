using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class representing the most basic unit in our grid system, that is, a single node
/// </summary>
public class GridNode : IBinaryHeapComparable<GridNode>, IUniqueIdentifiable<GridNode>
{
    #region Constructor

    public GridNode(int uniqueId, int gridXIndex, int gridYIndex, Vector3 pos, bool walkable)
    {
        this.uniqueId = uniqueId;
        this.gridXIndex = gridXIndex;
        this.gridYIndex = gridYIndex;
        this.pos = pos;
        this.walkable = walkable;
    }

    #endregion

    #region Private Attributes

    private int uniqueId = 0;
    private int heapIndex = 0;

    private int gridXIndex = 0;
    private int gridYIndex = 0;

    private Vector3 pos = Vector3.zero;

    private bool walkable = true;

    private GridNode parent = null;

    private int gCost = 0;
    private int hCost = 0;

    private List<GridNode> neighbors = new List<GridNode>();

#if UNITY_EDITOR

    private bool hasAlignedWallAtSomeSide = false;

#endif

    #endregion

    #region Properties

    public int UniqueId { get { return uniqueId; } }
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int GridXIndex { get { return gridXIndex; } }
    public int GridYIndex { get { return gridYIndex; } }

    public Vector3 Pos { get { return pos; } }

    public bool Walkable
    {
        get { return walkable; }
        set { walkable = value; }
    }

    public GridNode Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    public int GCost
    {
        get { return gCost; }
        set { gCost = value; }
    }
    public int HCost
    {
        get { return hCost; }
        set { hCost = value; }
    }
    public int FCost { get { return gCost + hCost; } }

    public List<GridNode> Neighbors { get { return neighbors; } }

#if UNITY_EDITOR

    public bool HasAlignedWallAtSomeSide
    {
        get { return hasAlignedWallAtSomeSide; }
        set { hasAlignedWallAtSomeSide = value; }
    }

#endif

    #endregion

    #region Interfaces Methods

    /// <summary>
    /// Compare the costs of this node with another node costs
    /// </summary>
    /// <param name="otherNode"></param>
    /// <returns></returns>
    public bool LowerThan(GridNode otherNode)
    {
        bool thisIsLower = false;

        if (FCost < otherNode.FCost)
            thisIsLower = true;

        if (FCost == otherNode.FCost)
        {
            if (HCost < otherNode.HCost)
                thisIsLower = true;
        }

        return thisIsLower;
    }

    #endregion
}