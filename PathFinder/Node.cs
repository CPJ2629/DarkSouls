using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//3D空间里网格的节点
public class Node
{
    public Vector2Int gridPos;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(Vector2Int gridPos)
    {
        this.gridPos = gridPos;
    }
}
