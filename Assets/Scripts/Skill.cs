using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public enum SkillType //技能种类
    {
        Attack = 1,
        Defend = 2,
        Cure = 3,
        SkillAssist = 4,
        AttackAssist = 5,
        PerishTogether = 6,
        Attack2 = 7,//血量特定变化时会产生的攻击
        Cure2 = 8 //血量特定变化时会产生的治疗

    };

    [System.Serializable]
    public struct DefendSkillParameter
    {
        public int time;//持续时间
        public float percent;//减少伤害的百分比
    }
    [System.Serializable]
    public struct AttackSkillParameter
    {

        public int value; //攻击力的值
        public Character.AttackType type; //攻击的类型
        public int blood_value; //引发攻击的血量临界值(当为Attack2技能类型时，有用)
    }

    [System.Serializable]
    public struct CureSkillParameter
    {
        public uint value; //治疗力度
    }

    [System.Serializable]
    public struct SkillAssistParameter //增加技能槽的辅助技能
    {
        public int value; //技能槽增加值
    }
    [System.Serializable]
    public struct AttackAssistParameter //提升攻击力的辅助技能
    {
        public float value; //攻击力增加比例 0-1
    }
    [System.Serializable]
    public struct PerishTogetherSkillParameter //同归于尽技能
    {
        public int percent; //同归于尽的可能性(0-100)
    }

    public SkillType type;
    public int accumulation;//技能积累速度
    public int skill_distance;//技能波及距离
    public DefendSkillParameter defend;// 防御类技能的参数
    public AttackSkillParameter attack;//攻击类技能参数
    public CureSkillParameter cure;//治疗类技能参数
    public SkillAssistParameter skill_assist;//技能槽增加
    public AttackAssistParameter attack_assist;//攻击力增加
    public PerishTogetherSkillParameter perish_together;//同归于尽技能
    private readonly object locker = new object();//读写锁


    private float attackCounter; //计时器变量
    private Character character;
    private int skill_value;//当前技能槽累计值

    // Use this for initialization
    void Start()
    {
        attackCounter = 0;
        skill_value = 0;
        character = GetComponent<Character>();
        GetComponent<Defend>().skill_type = type;

    }

    // Update is called once per frame
    void Update()
    {
        if (Global.is_start)
        {
            attackCounter += Time.deltaTime;
            if (attackCounter >= 1)
            {
                AddSkill(accumulation);
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
                    case SkillType.AttackAssist:
                        AttackAssist();
                        break;
                    case SkillType.SkillAssist:
                        SkillAssist();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void AddSkill(int value)
    {
        lock (locker)
        {
            skill_value += value;
        }

    }
    private void DefendSkill()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (friends.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/Cure"), this.gameObject.transform.position, Quaternion.identity);
        }
        foreach (GameObject friend in friends)
        {
            double distance = Character.GetDistance(character.GetNode(), friend.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                friend.GetComponent<Defend>().ExtraDefend(defend.percent, defend.time);

            }
        }
    }

    public void WhetherTrigger(float blood_value0, float blood_value1) //判断是否为引起攻击/治疗的临界血量
    {
        if (blood_value0 > attack.blood_value && blood_value1 <= attack.blood_value)
        {
            switch (type)
            {
                case SkillType.Attack2:
                    AttackSkill();
                    break;
                case SkillType.Cure2:
                    CureSkill();
                    break;
                default:
                    break;
            }

        }
        return;
    }
    private void AttackSkill()
    {
        Debug.Log("AttackSkill");
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(character.TargetTag);
        if (enemys.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/AttackSkill"), this.gameObject.transform.position, Quaternion.identity);
        }
        foreach (GameObject enemy in enemys)
        {
            double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                enemy.GetComponent<Defend>().TakeDamage(attack.value, attack.type, character.GetNode());

            }
        }
    }
    private void CureSkill()
    {
        Debug.Log("CureSkill");
        GameObject[] friends = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (friends.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/Cure"), this.gameObject.transform.position, Quaternion.identity);
        }

        foreach (GameObject friend in friends)
        {
            double distance = Character.GetDistance(character.GetNode(), friend.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                friend.GetComponent<Defend>().AddBlood(cure.value);

            }
        }
    }
    private void SkillAssist()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (friends.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/SkillAssist"), this.gameObject.transform.position, Quaternion.identity);
        }
        foreach (GameObject friend in friends)

        {
            double distance = Character.GetDistance(character.GetNode(), friend.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                friend.GetComponent<Skill>().AddSkill(skill_assist.value);
            }
        }
    }

    private void AttackAssist()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag(gameObject.tag);
        Debug.Log("AttackAssist");
        if (friends.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/AttackAssist"), this.gameObject.transform.position, Quaternion.identity);
        }

        foreach (GameObject friend in friends)
        {
            double distance = Character.GetDistance(character.GetNode(), friend.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                Debug.Log("PerishTogetherSkill");
                friend.GetComponent<Character>().ImprovAttackValue(attack_assist.value);
            }
        }
    }

    public void PerishTogetherSkill()
    {
        Debug.Log("PerishTogetherSkill");
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(character.TargetTag);
        if (enemys.Length > 0)
        {
            Instantiate(Resources.Load("SelectedEffect/PerishTogether"), this.gameObject.transform.position, Quaternion.identity);
        }
        foreach (GameObject enemy in enemys)
        {
            double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
            if (distance <= skill_distance + 0.0001)
            {
                int rand = Random.Range(0, 100);
                if (rand <= perish_together.percent)
                {
                    enemy.GetComponent<Defend>().KillObject();

                }

            }
        }

    }
}
