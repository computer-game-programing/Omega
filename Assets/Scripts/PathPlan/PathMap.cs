using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
public class PathMap : MonoBehaviour {

    public Vector3 origin_pos;
    public float cell_size;
    private ReaderWriterLockSlim read_write_lock = new ReaderWriterLockSlim();
    private Dictionary<int, bool> block_node 
               = new Dictionary<int, bool>();



   // Use this for initialization
   void Awake () {
        //collider = gameObject.GetComponent<BoxCollider>();
        for(int i = 0; i < 99; i++)
        {
            Write(i, false);
        }
        
   }

   // Update is called once per frame
   void Update () {

   }

   public bool Read(int key)
   {
       read_write_lock.EnterReadLock();
       try
       {
           return block_node[key];
       }
       finally
       {
           read_write_lock.ExitReadLock();
       }
   
   }

   public void Write(int key, bool is_blocked)
   {
       read_write_lock.EnterWriteLock();
       try
       {
           block_node[key] = is_blocked;
       }
       finally
       {
           read_write_lock.ExitWriteLock();
       }

   }

    public Vector3 GetPosByIndex(int node_index)
    {
        Vector3 pos = new Vector3();
        int z_index = node_index / 10;
        int x_index = node_index % 10;
        pos.x = origin_pos.x + x_index * cell_size;
        pos.z = origin_pos.z + z_index * cell_size;
        pos.y = origin_pos.y;
        return pos;
    }
    public int GetIndexByPos(Vector3 pos)
    {
        Vector3 diff = pos - origin_pos;
        int x_index =(int)Math.Floor(diff.x  / cell_size + 0.5f);
        int z_index = (int)Math.Floor(diff.z / cell_size + 0.5f);
        return z_index * 10 + x_index;

    }
    public void OnGUI()
    {
      
        if (GUILayout.Button("Start"))
        {
            Global.is_start = true;

        }
    }
}
