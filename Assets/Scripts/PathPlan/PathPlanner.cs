using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlanner : MonoBehaviour {

    private GameObject path_map;

    // Use this for initialization
    void Start()
    {
        GameObject chess_board = GameObject.Find("ChessBoard");
        path_map = chess_board.transform.Find("PathMap").gameObject;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int FindNextNode(int start_x, int start_y, int target_x, int target_y)
    {
       /* Point start = new Point(start_x, start_y);
        Point end = new Point(target_x, target_y);
        Point temp =  astar.FindPath(start, end);
        Debug.Log(temp);
        while(temp.parent.x != start_x && temp.parent.y != start_y)
        {
            temp = temp.parent;
        }
        return temp.x + temp.y * 10;*/
        int diff_x = target_x - start_x;
        int flag_x;
        if (diff_x == 0)
        {
            flag_x = 1;
        }
        else
        {
            flag_x = diff_x / System.Math.Abs(diff_x);
        }
         
        int diff_y = target_y - start_y;
        int flag_y;
        if (diff_x == 0)
        {
            flag_y = 1;
        }
        else
        {
            if(diff_y !=0)
            flag_y = diff_y / System.Math.Abs(diff_y);
            else
            {
                flag_y = -1;
            }
        }
        double ratio = System.Math.Abs(diff_y * 1.0 / diff_x);
        int[] sorted_choose = new int[4];
        if (ratio > 1)
        {
            sorted_choose[0] = flag_y * 10;
            sorted_choose[1] = flag_x * 1;
            sorted_choose[2] = -flag_x * 1;
            sorted_choose[3] = -flag_y * 10;
        }
        else
        {
            sorted_choose[0] = flag_x * 1;
            sorted_choose[1] = flag_y * 10;
            sorted_choose[2] = -flag_y * 10;
            sorted_choose[3] = -flag_x * 1;
        }
        int id = GetIndexFromXY(start_x, start_y);
        for(int i = 0; i< 4; i++)
        {
       
            if(!path_map.GetComponent<PathMap>().Read(id + sorted_choose[i]))
            {
                return id + sorted_choose[i];
            }
        }
        return -1;
    }

    private int GetIndexFromXY(int x_index,int y_index)
    {
        return y_index * 10 + x_index;
    }


}
