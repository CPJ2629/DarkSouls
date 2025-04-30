using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    //A*Ѱ·�㷨
    public Transform player;
    public float gridSize = 1f;//�����С
    public LayerMask obstacleMask;

    public Stack<Vector3> path = new Stack<Vector3>();//���·��

    private Vector2Int WorldPositionToGrid(Vector3 pos) //��������->��������
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

    private List<Node> GetNeighbours(Node node) //��ȡ�˸���������ڽڵ�
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
        return !Physics.CheckSphere(worldPos, gridSize / 2, LayerMask.GetMask("Obstacle"));//������ײ������Ƿ����ϰ�
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int disY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        // �Խ����ƶ��ɱ�14��ֱ���ƶ��ɱ�10
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
                if (!isWalkable(newNode) || visited.Contains(newNode)) continue; //���ϰ����ǰ�ڵ��Ѿ���������

                int newgCost = cur.gCost + GetDistance(cur, newNode);
                if (newgCost<newNode.gCost) //δ��������ӣ��ѱ����͸��³ɱ�
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
