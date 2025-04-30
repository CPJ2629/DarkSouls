using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    //A*寻路算法
    public Transform player;
    public float gridSize = 1f;//网格大小
    public LayerMask obstacleMask;

    public Stack<Vector3> path = new Stack<Vector3>();//最短路径

    private Vector2Int WorldPositionToGrid(Vector3 pos) //世界坐标->网格坐标
    {
        int x = Mathf.RoundToInt(pos.x / gridSize);
        int y = Mathf.RoundToInt(pos.z / gridSize);
        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorldPosition(Node node)
    {
        float x = node.gridPos.x * gridSize;
        float z = node.gridPos.y * gridSize;
        return new Vector3(x, 0, z);
    }

    private List<Node> GetNeighbours(Node node) //获取八个方向的相邻节点
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(x==0 && y==0) continue;
                Vector2Int gridPos = new Vector2Int(node.gridPos.x + x, node.gridPos.y + y);
                neighbours.Add(new Node(gridPos));
            }
        }
        return neighbours;
    }

    private bool isWalkable(Node node)
    {
        Vector3 worldPos = GridToWorldPosition(node);
        return !Physics.CheckSphere(worldPos, gridSize / 2, LayerMask.GetMask("Obstacle"));//球形碰撞器检测是否有障碍
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int disY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        // 对角线移动成本14，直线移动成本10
        if (disX > disY)
            return 14 * disY + 10 * (disX - disY);
        return 14 * disX + 10 * (disY - disX);
    }

    public void FindPath(Vector3 start,Vector3 end)
    {
        Node startNode = new Node(WorldPositionToGrid(start));
        Node endNode = new Node(WorldPositionToGrid(end));

        PriorityQueue<Node> q = new PriorityQueue<Node>();
        q.Push(startNode, startNode.fCost);
        HashSet<Node> visited = new HashSet<Node>();

        while (q.Count > 0)
        {
            Node cur = q.Pop();
            visited.Add(cur);

            if (cur.gridPos == endNode.gridPos)
            {
                BackTrack(startNode, endNode);
                return;
            }

            foreach(Node newNode in GetNeighbours(cur))
            {
                if (!isWalkable(newNode) || visited.Contains(newNode)) continue; //有障碍物或当前节点已经遍历跳过

                int newgCost = cur.gCost + GetDistance(cur, newNode);
                if (newgCost<newNode.gCost) //未遍历就添加，已遍历就更新成本
                {
                    newNode.gCost = newgCost;
                    newNode.hCost = GetDistance(newNode, endNode);
                    newNode.parent = cur;
                }
                if (!visited.Contains(newNode)) visited.Add(newNode);

                if (q.Contains(newNode)) q.UpdatePriority(newNode, newNode.fCost);
                else q.Push(newNode, newNode.fCost);
            }
        }
    }

    private void BackTrack(Node startNode,Node endNode)
    {
        path.Clear();
        Node cur = endNode;
        while (cur != startNode && cur!=null)
        {
            path.Push(GridToWorldPosition(cur));
            cur = cur.parent;
        }
        path.Push(GridToWorldPosition(startNode));
    }
}
