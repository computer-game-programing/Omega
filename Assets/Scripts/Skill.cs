using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {
    public enum SkillType
    {
        Attack,
        Defend,
        Cure
   
    };

    [System.Serializable]
    public struct DefendSkillParameter
    {
        public int distance;//涉及距离
        public int time;//持续时间
        public float percent;//减少伤害的百分比
    }
    [System.Serializable]
    public struct AttackSkillParameter
    {
        public int distance;//涉及距离
        public int value; //攻击力的值
    }


    public SkillType type;
    public int accumulation;//技能积累速度
    public DefendSkillParameter defend;// 防御类技能的参数

    private float attackCounter; //计时器变量
    private Character character;
    private int skill_value;//当前技能槽累计值

    // Use this for initialization
    void Start () {
        attackCounter = 0;
        character = GetComponent<Character>();
        skill_value = 0;
    }
	
	// Update is called once per frame
	void Update () {
        attackCounter += Time.deltaTime;
        if (attackCounter >= 1)
        {
            skill_value += accumulation;
            attackCounter = 0;
        }
        if (skill_value >= 100)
        {
            switch (type)
            {
                case SkillType.Defend:

                    break;
            }
        }
     }

    private void DefendSkill()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(gameObject.tag);
        foreach (GameObject enemy in enemys)
        {
            double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
            if (distance <= distance + 0.0001)
            {
                enemy.GetComponent<Defend>().ExtraDefend(defend.percent, defend.time);

            }
        }
    }
}
