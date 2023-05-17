using System;
using UnityEngine;

public class Node: IComparable<Node>
{
    public Vector2Int GridPosition;
    public int GCost = 0;
    public int HCost = 0;
    public bool IsObstacle = false;
    public int MovementPenalty;
    public Node ParentNode;

    public int FCost
    {
        get => GCost + HCost;
    }

    public Node(Vector2Int gridPosition)
    {
        this.GridPosition = gridPosition;
        ParentNode = null;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return compare;
    }
}
