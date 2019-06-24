using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

    public enum AttackType
    {
        Cold,
        Hot,
        All
    };

    [System.Serializable]

    public struct ATK
    {
        public int maximum_attack_distance;
        public int attack_damage;
        public float attack_time;
        public AttackType attack_type;
    };

    //[SyncVar]

    public string TargetTag;
    // private int node_index;
    private Node node;
    private GameObject targetobj;
    //    private PathMap path_map;
    //  private PathPlanner path_planner;
    private FindPath find_path;
    private Grid grid;
    private GameObject robot;
    private float robot_angle;
    public float move_speed;
    private float attackCounter = 0; //计时器变量
    public ATK[] attack_setting;
    public AttackType attack_type;
    private int attack_num = 0;
    private bool mouse_down = false;

    // Use this for initialization
    void Awake()
    {

    }
    void Start()
    {
        robot = transform.Find("robot").gameObject;
        robot_angle = robot.transform.eulerAngles.y;
        Animation a = robot.GetComponent<Animation>();
        foreach (AnimationState s in a)
        {
            s.speed = 2.0f;
        }
        robot.GetComponent<Animation>().Stop();
        if (gameObject.tag == "Own")
        {
            robot.transform.Rotate(Vector3.up, 180);
        }
        GameObject chess_board = GameObject.Find("ChessBoard");
        // path_map = chess_board.transform.Find("PathMap").gameObject.GetComponent<PathMap>();
        // node_index = path_map.GetIndexByPos(transform.position);
        //path_map.Write(node_index, true);
        // path_planner = gameObject.GetComponent<PathPlanner>();
        find_path = chess_board.transform.Find("PathMap").gameObject.GetComponent<FindPath>();
        grid = chess_board.transform.Find("PathMap").gameObject.GetComponent<Grid>();
        node = grid.GetFromPos(transform.position);
        grid.Write(node._gridX, node._gridY, true);
        //transform.Find("Canvas").transform.Find("Slider").
        FindTarget();
        //int target_id = targetobj.GetComponent<Character>().GetNodeIndex();

        //int next_node = path_planner.FindNextNodeByAstar(GetXByIndex(node_index), GetZByIndex(node_index), GetXByIndex(target_id), GetZByIndex(target_id));

    }

    // Update is called once per frame
    void Update()
    {
        //if(targetobj == null) robot.GetComponent<Animation>().Stop();
        if (Global.is_start)
        {
            MovetoTarget();
            if (targetobj != null && attack_num < attack_setting.Length)
            {
                double distance = GetDistance2Target();
                if (distance <= attack_setting[attack_num].maximum_attack_distance + 0.1)
                {
                    if (attack_num == 0)
                    {
                        RobotLookAt(targetobj.transform.position);
                    }
                    attackCounter += Time.deltaTime;
                    if (attackCounter > attack_setting[attack_num].attack_time)
                    {
                        robot.GetComponent<Animation>().Play("attack");
                        if (targetobj.GetComponentInParent<Defend>().TakeDamage(attack_setting[attack_num], gameObject))
                        {
                            targetobj = null;
                        }
                        attackCounter = 0;
                    }
                }

            }
            else
            {
                FindTarget();

            }
            if (targetobj == null)
            {
                robot.GetComponent<Animation>().Stop();
                return;
            }
        }
    }

    public Node GetNode()
    {
        return node;
    }
    public GameObject GetTargetObj()
    {
        return targetobj;
    }
    private static int GetXByIndex(int node_index)
    {
        return node_index % 10;
    }
    private static int GetZByIndex(int node_index)
    {
        return node_index / 10;
    }

    public static double GetDistance(Node node1, Node node2)
    {
        int dx = node1._gridX - node2._gridX;
        int dz = node1._gridY - node2._gridY;
        return System.Math.Sqrt(dx * dx + dz * dz);
    }
    public double GetDistance2Target()
    {
        Node t = targetobj.GetComponent<Character>().GetNode();
        int dx = node._gridX - t._gridX;
        int dz = node._gridY - t._gridY;
        return System.Math.Sqrt(dx * dx + dz * dz);
    }

    private void FindTarget()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(TargetTag);
        double distance_min = 10000;
        foreach (GameObject enemy in enemys)
        {
            double distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < distance_min)
            {
                distance_min = distance;
                targetobj = enemy;
            }
        }
        attack_num = attack_setting.Length;
        for (int i = 0; i < attack_setting.Length; i++)
        {
            if (distance_min < attack_setting[i].maximum_attack_distance)
            {
                attack_num = i;
                break;
            }
        }

    }

    private void MovetoTarget()
    {

        if (Vector3.Distance(transform.position, node._worldPos) > 0.15)
        {
            robot.GetComponent<Animation>().Play("walk");

            //  Vector3 pos = path_map.GetPosByIndex(node_index) - transform.position;
            Vector3 pos = node._worldPos - transform.position;
            transform.Translate(Vector3.Normalize(pos) * 0.06f * move_speed);
            return;
        }
        if (attack_num == 0)
        {
            return;
        }
        if (targetobj == null)
        {
            Debug.Log("targetobj == null");
            return;
        }

        double distance = GetDistance(node, targetobj.GetComponent<Character>().GetNode());

        if (distance < attack_setting[attack_num - 1].maximum_attack_distance + 0.1)
        {
            attack_num = attack_num - 1;
            attackCounter = attack_setting[attack_num].attack_time;
            if (attack_num == 0)
            {
                return;
            }
        }
        List<Node> result = find_path.FindingPath(transform.position, targetobj.transform.position);

        Node next_node;
        if (result != null)
        {
            next_node = result[0];
        }
        else
        {
            return;
        }
        grid.Write(node._gridX, node._gridY, false);
        grid.Write(next_node._gridX, next_node._gridY, true);
        // path_map.Write(node_index, false);
        //path_map.Write(next_node, true);
        robot.GetComponent<Animation>().Play("walk");
        Vector3 temp_pos = next_node._worldPos;
        RobotLookAt(temp_pos);
        transform.Translate(Vector3.Normalize(temp_pos - transform.position) * 0.06f * move_speed);
        node = next_node;

        //int target_id = targetobj.GetComponent<Character>().GetNodeIndex();
        //int next_node = path_planner.FindNextNode(GetXByIndex(node_index), GetZByIndex(node_index), GetXByIndex(target_id), GetZByIndex(target_id));

    }

    private void RobotLookAt(Vector3 pos)
    {
        Vector3 p = pos;
        p.y = robot.transform.position.y;
        robot.gameObject.transform.LookAt(p);
        robot.gameObject.transform.Rotate(Vector3.up, robot_angle);
    }

    public void ComparetoTargetObj(GameObject attack_obj)
    {
        if (Vector3.Distance(transform.position, targetobj.transform.position) > Vector3.Distance(transform.position, attack_obj.transform.position))
        {
            targetobj = attack_obj;
        }
    }
}