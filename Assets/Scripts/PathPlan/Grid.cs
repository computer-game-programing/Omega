using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class Node
{
    /// <summary>
    /// 是否可以通过此路径
    /// </summary>
    public bool _canWalk;
    /// <summary>
    /// 保存节点位置
    /// </summary>
    public Vector3 _worldPos;
    /// <summary>
    /// 整个网格的索引
    /// </summary>
    public int _gridX, _gridY;


    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node parent;

    public Node(bool _canWalk, Vector3 _worldPos, int _gridX, int _gridY)
    {
        this._canWalk = _canWalk;
        this._worldPos = _worldPos;
        this._gridX = _gridX;
        this._gridY = _gridY;
    }
}

public class Grid : MonoBehaviour
{

    Node[,] grid;
    /// <summary>
    /// 保存网格大小
    /// </summary>
    private Vector3 gridSize;
    /// <summary>
    /// 节点半径
    /// </summary>
    private float nodeRadius;
    /// <summary>
    /// 节点直径
    /// </summary>
    float nodeDiameter;
    /// <summary>
    /// 射线图层
    /// </summary>
    public LayerMask WhatLayer;

    /// <summary>
    /// 每个方向网格数的个数
    /// </summary>
    public int gridCntX, gridCntY;
    //读写锁
    private ReaderWriterLockSlim read_write_lock = new ReaderWriterLockSlim();
    // 保存障碍物信息
    private bool[ ,] block_node;
    /// <summary>
    /// 保存路径列表
    /// </summary>
    public List<Node> path = new List<Node>();
    // Use this for initialization
    // 棋盘位置
    private Vector3 origin_pos;

   

    void Awake()
    {
        gridCntX = 10;
        gridCntY = 10;
        gridSize = GetComponent<BoxCollider>().size;
        nodeDiameter = gridSize.x / gridCntX;
        nodeRadius = nodeDiameter / 2;
        block_node = new bool[gridCntX, gridCntY];
        origin_pos.x = transform.position.x + GetComponent<BoxCollider>().center.x - gridSize.x * 0.5f;
        origin_pos.z = transform.position.z + GetComponent<BoxCollider>().center.z - gridSize.z * 0.5f;
        origin_pos.y = transform.position.y + GetComponent<BoxCollider>().center.y;
        for (int i = 0; i < gridCntX; i++)
        {
            for(int j = 0; j < gridCntY; j++)
            {
                Write(i,j, false);
            }
           
        }
        grid = new Node[gridCntX, gridCntY];
        CreateGrid();
    }
    public bool Read(int x,int y)
    {
        read_write_lock.EnterReadLock();
        try
        {
            return block_node[x,y];
        }
        finally
        {
            read_write_lock.ExitReadLock();
        }

    }

    public void Write(int x,int y, bool is_blocked)
    {
        read_write_lock.EnterWriteLock();
        try
        {
            block_node[x,y] = is_blocked;
        }
        finally
        {
            read_write_lock.ExitWriteLock();
        }

    }
    private void CreateGrid()
    {
        Vector3 startPoint = transform.position - gridSize.x * 0.5f * Vector3.right
            - gridSize.y * 0.5f * Vector3.forward;
        for (int i = 0; i < gridCntX; i++)
        {
            for (int j = 0; j < gridCntY; j++)
            {
                Vector3 pos;
                pos.x = origin_pos.x + i * nodeDiameter + nodeRadius;
                pos.z = origin_pos.z + j * nodeDiameter + nodeRadius;
                pos.y = origin_pos.y;
                Vector3 worldPoint = startPoint + Vector3.right * (i * nodeDiameter + nodeRadius)
                    + Vector3.forward * (j * nodeDiameter + nodeRadius);
                //此节点是否可走
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, WhatLayer);
                //i，j是二维数组的索引
                grid[i, j] = new Node(walkable, pos, i, j);
            }
        }
    }

    public Node GetFromPos(Vector3 pos)
    {
        float percentX = (pos.x - origin_pos.x) / gridSize.x;
        float percentY = (pos.z - origin_pos.z) / gridSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridCntX - 1) * percentX);
        int y = Mathf.RoundToInt((gridCntY - 1) * percentY);
        return grid[x, y];

    }

   /* void OnDrawGizmos()
    {
        //画出网格边缘
        Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, new Vector3(gridSize.x, 1, gridSize.z));
        //画不可走网格
        if (grid == null)
            return;
       // Node playerNode = GetFromPos(player.position);
        foreach (var item in grid)
        {
            Gizmos.color = item._canWalk ? Color.white : Color.red;
            Gizmos.DrawCube(item._worldPos, Vector3.one * (nodeDiameter - 0.1f));
        }
        //画路径
        if (path != null)
        {
            foreach (var item in path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(item._worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
        //画玩家
       // if (playerNode != null && playerNode._canWalk)
       // {
       //     Gizmos.color = Color.cyan;
       //     Gizmos.DrawCube(playerNode._worldPos, Vector3.one * (nodeDiameter - 0.1f));
       // }
    }*/

    public List<Node> GetNeibourhood(Node node, Node endnode)
    {
        List<Node> neibourhood = new List<Node>();
        //相邻上下左右格子
        
        int tempX = node._gridX + 1;
        int tempY = node._gridY;
        if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY &&( ! block_node[tempX,tempY] || (tempX == endnode._gridX && tempY == endnode._gridY)) )
        {
            neibourhood.Add(grid[tempX, tempY]);
        }

        tempX = node._gridX - 1;
        tempY = node._gridY;
        if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY && (!block_node[tempX, tempY] || (tempX == endnode._gridX && tempY == endnode._gridY)))
        {
            neibourhood.Add(grid[tempX, tempY]);
        }

        tempX = node._gridX;
        tempY = node._gridY+1;
        if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY && (!block_node[tempX, tempY] || (tempX == endnode._gridX && tempY == endnode._gridY)))
        {
            neibourhood.Add(grid[tempX, tempY]);
        }

        tempX = node._gridX;
        tempY = node._gridY-1;
        if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY && (!block_node[tempX, tempY] || (tempX == endnode._gridX && tempY == endnode._gridY)))
        {
            neibourhood.Add(grid[tempX, tempY]);
        }

        return neibourhood;
    }
}