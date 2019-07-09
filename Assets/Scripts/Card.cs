using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//攻击详情
[System.Serializable]
public class ATK
{
    public int max_distance;
    public float value;
    public float time;
}
[System.Serializable]
public class DFS
{
    public int distance;
    public float possibility;
    public float percent;
}
[System.Serializable]
public struct CureSetting
{
    public int max_distance;
    public float value;
    public float time;
}

//card基类，不被绑定的脚本
//CardIndex为卡的唯一属性
[System.Serializable]
public class SimpleCard
{
    public int buy;                     //购买价格
    public int sell;                    //出售价格
    public int CardIndex;               //一共90种，index确定，固有属性确定
    public int type;                 //类型
    public string name;     //名称
    public string intro;    //简介(MP)


    public int atk_type;    //攻击类型
    public ATK[] atks;      //攻击区间
    public int dfs_type;    //防御类型（和攻击类型相对）
    public DFS dfs;         //防御
    public CureSetting[] cures;    //治疗区间

    //后来新加的部分
    public int skill_type;
    public int distance;
    public float accumulatation;
    public int attack_value;
    public int attack_type;
    public int attack_blood_value;
    public int defend_time;
    public float defend_percent;
    public float cure_value;
    public int cure_blood_value;
    public int skill_assists_value;
    public float attack_assist_value;
    public float perish_together_percent;


    //初始化,仅有index决定
    public SimpleCard()
    {
        atks = new ATK[2];
        dfs = new DFS();
        cures = new CureSetting[2];
        name = intro = "";
    }
    /*public SimpleCard(int CardIndex)
    {
        this.CardIndex = CardIndex;
        switch (CardIndex)
        {   //1-1to1-3
            case 0:
                buy = 5;
                sell = 5;
                break;
            case 1:
                buy = 5;
                sell = 7;
                break;
            case 2:
                buy = 5;
                sell = 9;
                break;
            //3-1to3-3
            case 6:
                buy = 4;
                sell = 4;
                break;
            case 7:
                buy = 4;
                sell = 6;
                break;
            case 8:
                buy = 4;
                sell = 8;
                break;
            //6-1to6-3
            case 15:
                buy = 4;
                sell = 4;
                break;
            case 16:
                buy = 4;
                sell = 6;
                break;
            case 17:
                buy = 4;
                sell = 8;
                break;
        }
    }*/

}
