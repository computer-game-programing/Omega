using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillType //技能种类
    {
        Attack,
        Defend,
        Cure,
        SkillAssist,
        AttackAssist

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
        public Character.AttackType type; //攻击的类型
    }

    [System.Serializable]
    public struct CureSkillParameter
    {
        public int distance;//涉及距离
        public int value; //治疗力度
    }

    [System.Serializable]
    public struct SkillAssistParameter //增加技能槽的辅助技能
    {
        public int distance;//涉及距离
        public int value; //技能槽增加值
    }
    [System.Serializable]
    public struct AttackAssistParameter //提升攻击力的辅助技能
    {
        public int distance;//涉及距离
        public float value; //攻击力增加比例 0-1
    }

    public SkillType type;
    public int accumulation;//技能积累速度
    public DefendSkillParameter defend;// 防御类技能的参数
    public AttackSkillParameter attack;//攻击类技能参数
    public CureSkillParameter cure;//治疗类技能参数

    private float attackCounter; //计时器变量
    private Character character;
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
        if (Global.is_start)
        {
            attackCounter += Time.deltaTime;
            if (attackCounter >= 1)
            {
                skill_value += accumulation;
                attackCounter = 0;
            }
            if (skill_value >= 100)
            {
                skill_value = 0;
                switch (type)
                {
                    case SkillType.Defend:
                        DefendSkill();
                        break;
                    case SkillType.Attack:
                        AttackSkill();
                        break;
                    case SkillType.Cure:
                        CureSkill();
                        break;
                }
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

    private void AttackSkill()
    {
        Debug.Log("AttackSkill");
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(character.TargetTag);
        foreach (GameObject enemy in enemys)
        {
            double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
            if (distance <= attack.distance + 0.0001)
            {
                enemy.GetComponent<Defend>().TakeDamage(attack.value, attack.type, character.GetNode());

            }
        }
    }
    private void CureSkill()
    {
        Debug.Log("CureSkill");
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(gameObject.tag);
        foreach (GameObject enemy in enemys)
        {
            double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
            if (distance <= cure.distance + 0.0001)
            {
                enemy.GetComponent<Defend>().AddBlood(cure.value);

            }
        }
    }
}
