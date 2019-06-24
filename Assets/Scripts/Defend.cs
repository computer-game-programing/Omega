using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Defend : MonoBehaviour
{
    private int MaxHP = 100;
    public Slider blood_slider;
    public int distance; // 防御距离
    public int possibility; //防御可能性 百分数
    public float percent; // 防御伤害的百分比
    public Character.AttackType type;//防御的伤害种类
    private float CurrentHP;
    private readonly object locker = new object();
    private Character character;
    private struct DefendSkiilSetting
    {
        public int minute;
        public int second;
        public float value;
    }
    private List<DefendSkiilSetting> defend_setting;
    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();
        CurrentHP = MaxHP;
        defend_setting = new List<DefendSkiilSetting>();


    }

    // Update is called once per frame
    void Update()
    {

        if (defend_setting.Count == 0)
        {
            return;
        }

        for (int i = 0; i < defend_setting.Count; i++)
        {
            DefendSkiilSetting item = defend_setting[i];
            if (item.minute == System.DateTime.Now.Minute && item.second == System.DateTime.Now.Second)
            {
                lock (locker)
                {
                    percent -= item.value;
                }
                defend_setting.Remove(item);
                i--;
            }

        }


    }

    public void TakeDamage(Character.ATK attack, Node node)
    {
        float damage_value = attack.attack_damage;

        if (attack.attack_type == type && Character.GetDistance(character.GetNode(), node) < distance)
        {
            int rand = Random.Range(0, 100);
            if (rand <= possibility)
            {
                damage_value *= Mathf.Clamp01(1 - percent);
            }

        }
        lock (locker)
        {
            CurrentHP -= damage_value;
        }
        ChangeBlood((-1) * damage_value);
    }

    public void TakeDamage(Character.ATK attack, GameObject enemy)
    {

        float damage_value = attack.attack_damage;

        if (attack.attack_type == type && Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode()) < distance)
        {
            int rand = Random.Range(0, 100);
            if (rand <= possibility)
            {
                damage_value *= Mathf.Clamp01(1 - percent);
            }

        }
        ChangeBlood((-1) * damage_value);
    }
    public void TakeDamage(int value, Character.AttackType attack_type, Node node)
    {

        float damage_value = value;

        if (attack_type == type && Character.GetDistance(character.GetNode(), node) < distance)
        {
            int rand = Random.Range(0, 100);
            if (rand <= possibility)
            {
                damage_value *= Mathf.Clamp01(1 - percent);
            }

        }
        ChangeBlood((-1) * damage_value);
    }
    public void ShowHPSlider()
    {
        blood_slider.value = CurrentHP / MaxHP;
    }

    public void ExtraDefend(float value, int time)
    {
        lock (locker)
        {
            percent += value;
        }
        int m = System.DateTime.Now.Minute; // 当前时间 (分)
        int s = System.DateTime.Now.Second;
        if (s <= 55)
        {
            s += 4;

        }
        else
        {
            m += 1;
            s = s + 4 - 60;
        }
        DefendSkiilSetting d = new DefendSkiilSetting();
        d.minute = m;
        d.second = s;
        d.value = value;
        defend_setting.Add(d);
    }

    public void AddBlood(uint value)
    {
        ChangeBlood((int)value);
    }

    private void ChangeBlood(float value)
    { //改变血量并控制血条显示，value可正可负
        lock (locker)
        {
            CurrentHP += value;

        }
        ShowHPSlider();

        if (CurrentHP > 100)
        {
            CurrentHP = 100;
        }
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Destroy(blood_slider.gameObject);
            Destroy(this.gameObject);
        }
    }


}
