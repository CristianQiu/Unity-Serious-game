using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class used to do pathfinding tasks in our grid system. It will use the standard A * algorithm, that can be found
/// all over the internet
/// </summary>
public static class GridPathFinder
{
    #region Methods

    /// <summary>
    /// The function that calculates the path between two positions. It works with a certain number of nodes that are
    /// put onto a grid, meaning that the positions passed will be snapped to the center of their corresponding nodes
    /// and so will be each position of the path returned
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public static List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        GridMaster gm = null;
        //ObjectTracker.GetObject<GridMaster>();

        // convert the positions to nodes
        GridNode startNode = gm.PosToGridNode(startPos);
        GridNode endNode = gm.PosToGridNode(endPos);

        // initialize the sets
        HeapDictionary<GridNode> openSet = new HeapDictionary<GridNode>(256);
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        // and add the start node to start finding the path
        startNode.GCost = 0;
        startNode.HCost = GetDist(startNode, endNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // get the lowest fCost node from the open set
            GridNode currNode = openSet.ExtractMin();

            // add it to the closed set
            closedSet.Add(currNode);

            // we actually finished if it's the end node
            if (currNode == endNode)
                return GetPath(startNode, currNode);

            // otherwise iterate over its neighbors
            int numNeighbors = currNode.Neighbors.Count;

            for (int i = 0; i < numNeighbors; i++)
            {
                GridNode neighbor = currNode.Neighbors[i];

                // if we can't pass through the neighbor or it's in the closed set skip the iteration
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    continue;

                // calculate the new gCost to that neighbor
                int newGCostToNeighbor = currNode.GCost + GetDist(currNode, neighbor);

                // if it's lower than the actual one or the neighbor is not in the openset
                if (newGCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    // assign it costs
                    neighbor.GCost = newGCostToNeighbor;
                    neighbor.HCost = GetDist(neighbor, endNode);

                    // and set its parent
                    neighbor.Parent = currNode;

                    // if the openset does not contain this neighbor, add it to it
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    // otherwise tell the heap to reorder the element
                    else
                        openSet.UpdateElementWithDecreasedValue(neighbor);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Get the distance between two nodes considering we can only move diagonally, horizontally or vertically
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDist(GridNode nodeA, GridNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridXIndex - nodeB.GridXIndex);
        int distY = Mathf.Abs(nodeA.GridYIndex - nodeB.GridYIndex);

        int min = Mathf.Min(distX, distY);
        int max = Mathf.Max(distX, distY);

        int offset = max - min;

        // we are rounding diagonal neighbor cost to 14 and horizontal or vertical neighbor to 10
        int dist = (min * 14) + (offset * 10);

        return dist;
    }

    /// <summary>
    /// Reconstruct the path from the latest node
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private static List<Vector3> GetPath(GridNode startNode, GridNode endNode)
    {
        // fill the list from the end position to the start
        List<Vector3> points = new List<Vector3>();

        GridNode currNode = endNode;

        // iterate from the latest to the first one
        while (currNode != startNode)
        {
            points.Add(currNode.Pos);
            currNode = currNode.Parent;
        }

        // and reverse it
        points.Reverse();

        return points;
    }

    #endregion
}