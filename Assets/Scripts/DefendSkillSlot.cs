using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendSkillSlot : MonoBehaviour {

    private float attackCounter; //计时器变量
    private Character character;
    public int distance;//涉及距离
    public int time;//持续时间
    public float percent;//减少伤害的百分比
    public int accumulation;//技能积累速度
    private int skill_value;//当前技能槽累计值

    // Use this for initialization
    void Start()
    {
        attackCounter = 0;
        character = GetComponent<Character>();
        skill_value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        attackCounter += Time.deltaTime;
        if (attackCounter >= 1)
        {
            skill_value += accumulation;
            attackCounter = 0;
        }
        if (skill_value >= 100) { 
            skill_value = 0;
            GameObject[] enemys = GameObject.FindGameObjectsWithTag(gameObject.tag);
            foreach (GameObject enemy in enemys)
            {
                double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
                if (distance <= distance + 0.0001)
                {
                    enemy.GetComponent<Defend>().ExtraDefend(percent,time);

                }
            }
        }
    }

    public void AddSkill(int value)
    {
        skill_value += value;
    }
}
