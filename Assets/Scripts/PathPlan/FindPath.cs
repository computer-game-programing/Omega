using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath : MonoBehaviour
{
    public Transform player, EndPoint;
    Grid grid;
    // Use this for initialization
    void Start()
    {
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
       // FindingPath(player.position, EndPoint.position);
    }

    public List<Node> FindingPath(Vector3 StarPos, Vector3 EndPos)
    {
        Node startNode = grid.GetFromPos(StarPos);
        Node endNode = grid.GetFromPos(EndPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return GeneratePath(startNode, endNode);
            }
            //判断周围最优节点
            foreach (var item in grid.GetNeibourhood(currentNode,endNode))
            {
                    
                if (!item._canWalk || closeSet.Contains(item))
                    continue;
                int newCost = currentNode.gCost + GetDistanceNodes(currentNode, item);
                if (newCost < item.gCost || !openSet.Contains(item))
                {
                    item.gCost = newCost;
                    item.hCost = GetDistanceNodes(item, endNode);
                    item.parent = currentNode;
                    if (!openSet.Contains(item))
                    {
                        openSet.Add(item);
                    }
                }

            }
        }
        return null;
    }

    private List<Node> GeneratePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node temp = endNode;
        while (temp != startNode)
        {
            path.Add(temp);
            temp = temp.parent;
        }
        //列表反转
        path.Reverse();
        grid.path = path;
        return path;
    }


    int GetDistanceNodes(Node a, Node b)
    {
        //估算权值,对角线算法 看在X轴还是Y轴格子数多  可计算斜移动
        int cntX = Mathf.Abs(a._gridX - b._gridX);
        int cntY = Mathf.Abs(a._gridY - b._gridY);
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }

        //曼哈顿算法
        //return Mathf.Abs(a._gridX - b._gridX) * 10 + Mathf.Abs(a._gridY - b._gridY) * 10;
    }
}
