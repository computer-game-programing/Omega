using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Xml;
using System.IO;
public class EditPrefab : Editor
{

    [ExecuteInEditMode]
    [MenuItem("Tools/Set Prefab Data")]
    public static void SetData()
    {
        if (EditorUtility.DisplayDialog("功能确认",
          "此功能会重新读取Card信息",
          "确定"))
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefab/CardInfo.prefab", typeof(GameObject)) as GameObject;
            GameObject card_info = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            for (int i = 8; i <= 8; i++)
            {
                for (int j = 1; j <= 1; j++)
                {
                    GameObject robot = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefab/" + i + "_" + j + ".prefab", typeof(GameObject)) as GameObject;

                    // GameObject blood_obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefab/Blood.prefab", typeof(GameObject)) as GameObject;
                    // GameObject new_obj = PrefabUtility.InstantiatePrefab(obj) as GameObject; ;
                    GameObject ts = PrefabUtility.InstantiatePrefab(robot) as GameObject;
                    Debug.Log(ts);
                    // GameObject blood = PrefabUtility.InstantiatePrefab(blood_obj) as GameObject;
                    Debug.Log(ts.transform.Find("Canvas"));
                    Slider blood = robot.transform.Find("Canvas").Find("Slider").gameObject.GetComponent<Slider>();
                    SimpleCard card = card_info.GetComponent<CardsInfo>().SimpleCards[3 * (i - 1) + j - 1];

                    if (ts.GetComponent<PathPlanner>() == null)
                    {
                        ts.AddComponent<PathPlanner>();
                    }

                    if (ts.GetComponent<Character>() == null)
                    {
                        ts.AddComponent<Character>();
                    }

                    Character c = ts.GetComponent<Character>();
                    c.move_speed = 1.5f;
                    c.attack_type = Convert2AttackType(card.atk_type);
                    if (card.atks[0].max_distance > card.atks[1].max_distance)
                    {
                        c.attack_setting = new Character.ATK[2];
                        c.attack_setting[0] = Convert2AttackSetting(card.atks[0], c.attack_type);
                        c.attack_setting[1] = Convert2AttackSetting(card.atks[1], c.attack_type);
                    }
                    else if (card.atks[0].max_distance < card.atks[1].max_distance)
                    {
                        c.attack_setting = new Character.ATK[2];
                        c.attack_setting[0] = Convert2AttackSetting(card.atks[1], c.attack_type);
                        c.attack_setting[1] = Convert2AttackSetting(card.atks[0], c.attack_type);

                    }
                    else
                    {
                        c.attack_setting = new Character.ATK[1];
                        c.attack_setting[0] = Convert2AttackSetting(card.atks[0], c.attack_type);

                    }


                    if (ts.GetComponent<Defend>() == null)
                    {
                        ts.AddComponent<Defend>();
                    }

                    Defend defend = ts.GetComponent<Defend>();
                    defend.blood_slider = blood;
                    defend.distance = card.dfs.distance;
                    defend.possibility = (int)card.dfs.possibility * 100;
                    defend.percent = card.dfs.percent;
                    defend.type = Convert2AttackType(card.dfs_type);
                    defend.skill_type = Convert2SkillType(card.skill_type);

                    if (ts.GetComponent<Skill>() == null)
                    {
                        ts.AddComponent<Skill>();
                    }

                    Skill skill = ts.GetComponent<Skill>();
                    skill.type = Convert2SkillType(card.skill_type);
                    skill.accumulation = (int)card.accumulatation;
                    skill.skill_distance = card.distance;
                    if (skill.type == Skill.SkillType.Attack || skill.type == Skill.SkillType.Attack2)
                    {
                        skill.attack.value = card.attack_value;
                        skill.attack.type = Convert2AttackType(card.attack_type);
                        if (skill.type == Skill.SkillType.Attack2)
                        {
                            skill.attack.blood_value = card.attack_blood_value;
                        }
                    }
                    else if (skill.type == Skill.SkillType.Attack)
                    {
                        skill.defend.percent = card.defend_percent;
                        skill.defend.time = card.defend_time;
                    }
                    else if (skill.type == Skill.SkillType.PerishTogether)
                    {
                        skill.perish_together.percent = (int)card.perish_together_percent;
                    }
                    else if (skill.type == Skill.SkillType.AttackAssist)
                    {
                        skill.attack_assist.value = card.attack_assist_value;
                    }
                    else if (skill.type == Skill.SkillType.SkillAssist)
                    {
                        skill.skill_assist.value = card.skill_assists_value;
                    }
                    else if (skill.type == Skill.SkillType.Cure || skill.type == Skill.SkillType.Cure2)
                    {
                        skill.cure.value = (uint)card.cure_value;
                        if (skill.type == Skill.SkillType.Cure2)
                        {
                            skill.attack.blood_value = card.cure_blood_value;
                        }
                    }

                    if (card.type == 3)
                    {
                        if (ts.GetComponent<Cure>() == null)
                        {
                            ts.AddComponent<Cure>();
                        }
                        Cure cure = ts.GetComponent<Cure>();
                        cure.time = (int)card.cures[0].time;
                        if (card.cures[0].max_distance > card.cures[1].max_distance)
                        {
                            cure.cure_setting = new Cure.CureSetting[2];
                            cure.cure_setting[0] = Convert2CureSetting(card.cures[0]);
                            cure.cure_setting[1] = Convert2CureSetting(card.cures[1]);
                        }
                        else if (card.cures[0].max_distance < card.cures[1].max_distance)
                        {
                            cure.cure_setting = new Cure.CureSetting[2];
                            cure.cure_setting[0] = Convert2CureSetting(card.cures[1]);
                            cure.cure_setting[1] = Convert2CureSetting(card.cures[0]);
                        }
                        else
                        {
                            cure.cure_setting = new Cure.CureSetting[1];
                            cure.cure_setting[0] = Convert2CureSetting(card.cures[0]);
                        }

                    }
                    PrefabUtility.ReplacePrefab(ts, robot, ReplacePrefabOptions.Default);

                    MonoBehaviour.DestroyImmediate(ts);
                }
            }
            MonoBehaviour.DestroyImmediate(card_info);


            Debug.Log("Done");
            AssetDatabase.SaveAssets();
        }
    }

    private static Character.ATK Convert2AttackSetting(ATK origin, Character.AttackType type)
    {
        Character.ATK a = new Character.ATK(origin.max_distance, origin.time, origin.value, type);
        return a;

    }
    private static Cure.CureSetting Convert2CureSetting(CureSetting origin)
    {
        Cure.CureSetting a = new Cure.CureSetting((int)origin.max_distance, (uint)origin.value);
        return a;

    }


    private static Character.AttackType Convert2AttackType(int n)
    {
        Character.AttackType a = Character.AttackType.All;

        switch (n)
        {
            case 0:
                a = Character.AttackType.Cold;
                break;
            case 1:
                a = Character.AttackType.Hot;
                break;
            case 2:
                a = Character.AttackType.All;
                break;

        }
        return a;
    }

    private static Skill.SkillType Convert2SkillType(int n)
    {
        Skill.SkillType a = Skill.SkillType.Attack;

        switch (n)
        {
            case 1:
                a = Skill.SkillType.Attack;
                break;
            case 2:
                a = Skill.SkillType.Defend;
                break;
            case 3:
                a = Skill.SkillType.Cure;
                break;
            case 4:
                a = Skill.SkillType.SkillAssist;
                break;
            case 5:
                a = Skill.SkillType.AttackAssist;
                break;
            case 6:
                a = Skill.SkillType.PerishTogether;
                break;
            case 7:
                a = Skill.SkillType.Attack2;
                break;
            case 8:
                a = Skill.SkillType.Cure2;
                break;

        }
        return a;
    }
    [ExecuteInEditMode]
    [MenuItem("Tools/Read Data")]
    public static void LoadData()
    {
        // if (EditorUtility.DisplayDialog("功能确认",
        //     "此功能会重新读取Card信息",
        //     "确定"))
        // {
        //     GameObject prefab_ = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/CardInfo.prefab", typeof(GameObject)) as GameObject;
        //     GameObject cards = PrefabUtility.InstantiatePrefab(prefab_) as GameObject;
        //     SimpleCard[] SimpleCards = cards.GetComponent<CardsInfo>().SimpleCards;

        //     XmlDocument xml = new XmlDocument();

        //     xml.Load(Application.dataPath + "/Resources/total.xml");
        //     XmlNodeList xmlNodeList = xml.SelectSingleNode("gamedata").ChildNodes;

        //     foreach (XmlElement ele in xmlNodeList)
        //     {
        //         int index = int.Parse(ele.SelectSingleNode("id").InnerText);

        //         SimpleCards[index] = new SimpleCard();
        //         SimpleCards[index].CardIndex = index;
        //         SimpleCards[index].name = ele.SelectSingleNode("name").InnerText;
        //         SimpleCards[index].type = int.Parse(ele.SelectSingleNode("type").InnerText);
        //         SimpleCards[index].buy = int.Parse(ele.SelectSingleNode("buy").InnerText);
        //         SimpleCards[index].sell = int.Parse(ele.SelectSingleNode("sell").InnerText);
        //         SimpleCards[index].intro = ele.SelectSingleNode("intro").InnerText;

        //         SimpleCards[index].atk_type = int.Parse(ele.SelectSingleNode("atk_type").InnerText);
        //         SimpleCards[index].atks[0] = new ATK();
        //         SimpleCards[index].atks[0].max_distance = int.Parse(ele.SelectSingleNode("a_max_dst1").InnerText);
        //         SimpleCards[index].atks[0].value = float.Parse(ele.SelectSingleNode("a_val1").InnerText);
        //         SimpleCards[index].atks[0].time = float.Parse(ele.SelectSingleNode("a_t1").InnerText);
        //         SimpleCards[index].atks[1] = new ATK();
        //         SimpleCards[index].atks[1].max_distance = int.Parse(ele.SelectSingleNode("a_max_dst2").InnerText);
        //         SimpleCards[index].atks[1].value = float.Parse(ele.SelectSingleNode("a_val2").InnerText);
        //         SimpleCards[index].atks[1].time = float.Parse(ele.SelectSingleNode("a_t2").InnerText);
        //         SimpleCards[index].dfs_type = int.Parse(ele.SelectSingleNode("dfs_type").InnerText);
        //         SimpleCards[index].dfs.distance = int.Parse(ele.SelectSingleNode("d_dst").InnerText);
        //         SimpleCards[index].dfs.possibility = float.Parse(ele.SelectSingleNode("d_possibility").InnerText);
        //         SimpleCards[index].dfs.percent = float.Parse(ele.SelectSingleNode("d_per").InnerText);
        //         SimpleCards[index].cures[0] = new Cure();
        //         SimpleCards[index].cures[0].max_distance = int.Parse(ele.SelectSingleNode("c_max_dst1").InnerText);
        //         SimpleCards[index].cures[0].value = float.Parse(ele.SelectSingleNode("c_val1").InnerText);
        //         SimpleCards[index].cures[0].time = float.Parse(ele.SelectSingleNode("c_t1").InnerText);
        //         SimpleCards[index].cures[1] = new Cure();
        //         SimpleCards[index].cures[1].max_distance = int.Parse(ele.SelectSingleNode("c_max_dst2").InnerText);
        //         SimpleCards[index].cures[1].value = float.Parse(ele.SelectSingleNode("c_val2").InnerText);
        //         SimpleCards[index].cures[1].time = float.Parse(ele.SelectSingleNode("c_t2").InnerText);

        //         //新增部分
        //         SimpleCards[index].skill_type = int.Parse(ele.SelectSingleNode("skill_type").InnerText);
        //         SimpleCards[index].distance = int.Parse(ele.SelectSingleNode("distance").InnerText);
        //         SimpleCards[index].accumulatation = float.Parse(ele.SelectSingleNode("accumulatation").InnerText);
        //         SimpleCards[index].attack_value = int.Parse(ele.SelectSingleNode("attack_value").InnerText);
        //         SimpleCards[index].attack_type = int.Parse(ele.SelectSingleNode("attack_type").InnerText);
        //         SimpleCards[index].attack_blood_value = int.Parse(ele.SelectSingleNode("attack_blood_value").InnerText);
        //         SimpleCards[index].defend_time = int.Parse(ele.SelectSingleNode("defend_time").InnerText);
        //         SimpleCards[index].defend_percent = float.Parse(ele.SelectSingleNode("defend_percent").InnerText);
        //         SimpleCards[index].cure_value = float.Parse(ele.SelectSingleNode("cure_value").InnerText);
        //         SimpleCards[index].cure_blood_value = int.Parse(ele.SelectSingleNode("cure_blood_value").InnerText);
        //         SimpleCards[index].skill_assists_value = int.Parse(ele.SelectSingleNode("skill_assists_value").InnerText);
        //         SimpleCards[index].attack_assist_value = float.Parse(ele.SelectSingleNode("attack_assist_value").InnerText);
        //         SimpleCards[index].perish_together_percent = float.Parse(ele.SelectSingleNode("perish_together_percent").InnerText);

        //     }
        //     PrefabUtility.ReplacePrefab(cards, prefab_, ReplacePrefabOptions.Default);
        //     MonoBehaviour.DestroyImmediate(cards);
        //     Debug.Log("Done");
        // }

    }
    [ExecuteInEditMode]
    [MenuItem("Tools/Edit Prefab")]

    public static void AddHitData()
    {

        if (EditorUtility.DisplayDialog("功能确认", "设置Prefab",
            "确定"))
        {
            Vector3 pos = new Vector3(29, 1, 0);


            //LoadData();
            for (int i = 1; i <= 1; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    GameObject robot = AssetDatabase.LoadAssetAtPath("Assets/Resources/robots/" + i + "_" + j + ".fbx", typeof(GameObject)) as GameObject;
                    GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefab/Empty.prefab", typeof(GameObject)) as GameObject;
                    GameObject blood_obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefab/Blood.prefab", typeof(GameObject)) as GameObject;
                    GameObject new_obj = PrefabUtility.InstantiatePrefab(obj) as GameObject; ;
                    GameObject ts = PrefabUtility.InstantiatePrefab(robot) as GameObject;
                    GameObject blood = PrefabUtility.InstantiatePrefab(blood_obj) as GameObject;
                    if (new_obj.transform.Find("robot") == null)
                    {
                        ts.name = "robot";
                        ts.transform.SetParent(new_obj.transform);
                        blood.transform.SetParent(new_obj.transform);
                    }
                    new_obj.GetComponent<Transform>().position = pos;
                    var newprefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Prefab/" + i + "_" + j + ".prefab");
                    PrefabUtility.ReplacePrefab(new_obj, newprefab, ReplacePrefabOptions.Default);
                    MonoBehaviour.DestroyImmediate(new_obj);
                    MonoBehaviour.DestroyImmediate(ts);
                    MonoBehaviour.DestroyImmediate(blood);
                }
            }



            Debug.Log("Done");
            AssetDatabase.SaveAssets();
        }
    }
}