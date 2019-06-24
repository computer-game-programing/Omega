using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkillSlots : MonoBehaviour {


    private float attackCounter; //计时器变量
    private Character character;
    public Character.ATK attact_setting;

	// Use this for initialization
	void Start () {
        attackCounter = 0;
        character = GetComponent<Character>();
    }
	
	// Update is called once per frame
	void Update () {
        attackCounter += Time.deltaTime;
        if(attackCounter >= attact_setting.attack_time)
        {
            attackCounter = 0;
            GameObject[] enemys = GameObject.FindGameObjectsWithTag(character.TargetTag);
            foreach (GameObject enemy in enemys)
            {
                double distance = Character.GetDistance(character.GetNode(), enemy.GetComponent<Character>().GetNode());
                if (distance <= attact_setting.maximum_attack_distance + 0.0001)
                {
                    enemy.GetComponent<Defend>().TakeDamage(attact_setting, character.GetNode());
                 
                }
            }
        }
    }

    public void AddSkill(int percent)
    {
        attackCounter += attact_setting.attack_time * percent / 100.0f;
    }
}
