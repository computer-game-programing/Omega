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
        public int time;
    }
    public CureSetting[] cure_setting;
    private float time_counter; //计时器变量
    private int sid;

    private Character character;


    // Use this for initialization
    void Start()
    {

        sid = cure_setting.Length - 1;
        time_counter = cure_setting[sid].time;
        character = GetComponent<Character>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!Global.is_start)
        {
            return;
        }
        double distance = character.GetDistance2Target();
        if (distance <= cure_setting[sid].max_distance + 0.1)
        {
            time_counter += Time.deltaTime;
            if (time_counter >= cure_setting[sid].time)
            {
                time_counter = 0;
                character.GetTargetObj().GetComponent<Defend>().AddBlood(cure_setting[sid].value);
            }
            if (sid > 0)
            {
                if (distance <= cure_setting[sid - 1].max_distance + 0.1)
                {
                    sid -= 1;
                }
            }

        }

    }

}
