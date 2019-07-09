using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Cure : MonoBehaviour
{
    [System.Serializable]
    public struct CureSetting
    {
        public int max_distance;
        public uint value;
        public CureSetting(int d, uint v)
        {
            max_distance = d;
            value = v;
        }
    }
    public CureSetting[] cure_setting;
    private float time_counter; //计时器变量
    public int time;//治疗的速度 

    private Character character;


    // Use this for initialization
    void Start()
    {
        time_counter = time;
        character = GetComponent<Character>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!Global.is_start)
        {
            return;
        }

        time_counter += Time.deltaTime;
        if (time_counter >= time)
        {
            time_counter = 0;
            GameObject[] friends = GameObject.FindGameObjectsWithTag(gameObject.tag);
            foreach (GameObject friend in friends)
            {
                double distance = Character.GetDistance(character.GetNode(), friend.GetComponent<Character>().GetNode());
                int id = 0;
                while (id < cure_setting.Length)
                {
                    if (distance < cure_setting[id].max_distance + 0.1)
                    {
                        friend.GetComponent<Defend>().AddBlood(cure_setting[id].value);
                        break;
                    }
                    id++;
                }
            }

        }

    }

}
