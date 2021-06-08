using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class BotBehaviours : MonoBehaviour
{
    private List<ActionBranch> actions; // order of actions indicate priority in behaviour
    private GameObject player;
    private PlayerMovement player_script;
    public int bot_id = 0;
    // Start is called before the first frame update
    void Start()
    {
        bot_id = PlayerData.bot_id;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player_script = player.GetComponent<PlayerMovement>();
        actions = new List<ActionBranch>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "\\bots\\");
        FileInfo[] info = dir.GetFiles("*.txt");
        load_actions(info[bot_id].ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool load_actions(string filename)
    {
        //string file_path = Application.dataPath + "\\bots\\" + filename;
        string[] lines = System.IO.File.ReadAllLines(filename);
        int valid_pairs = 0;
        if(lines.Length == 0)
        {
            return false;
        }
        int line = 0;
        while(line < lines.Length)
        {
            string[] probabilities_string = lines[line].Split(' ');
            if (probabilities_string[0] == "#" || probabilities_string.Length != 6)
            {
                while (probabilities_string[0] == "#" || probabilities_string.Length != 6)
                {
                    line++;
                    if(line >= lines.Length)
                    {
                        break;
                    }
                    probabilities_string = lines[line].Split(' ');
                }
            }
            line++;
            if (line >= lines.Length)
            {
                break;
            }
            int[] probabilities = Array.ConvertAll(probabilities_string, int.Parse);
            ActionBranch action = new ActionBranch();
            action.random_move_prob = probabilities[0];
            action.move_towards_enemy_prob = probabilities[1];
            action.move_towards_goal_prob = probabilities[2];
            action.run_away_prob = probabilities[3];
            action.mov_coin_prob = probabilities[4];
            action.range_attack_prob = probabilities[5];


            string[] restrictions_string = lines[line].Split(' ');
            
            if (restrictions_string[0] == "#" && restrictions_string.Length != 9)
            {
                while (restrictions_string[0] == "#" || restrictions_string.Length != 9)
                {
                    line++;
                    if (line >= lines.Length)
                    {
                        break;
                    }
                    restrictions_string = lines[line].Split(' ');
                }
            }
            if (line >= lines.Length)
            {
                break;
            }
            int[] restrictions = Array.ConvertAll(restrictions_string, int.Parse);
            action.health_min = restrictions[0];
            action.health_max = restrictions[1];
            action.enemy_min = restrictions[2];
            action.enemy_max = restrictions[3];
            action.goal_max = restrictions[4];
            action.max_enemy_health = restrictions[5];
            action.coin_range = restrictions[6];
            action.coins_detect = restrictions[7];
            action.enemy_detect = restrictions[8];
            actions.Add(action);
        }

        return true;
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    public GameObject FindClosestCoin()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("coin");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }


    public int get_decision()
    {
        foreach(ActionBranch action in actions)
        {
            bool is_enemy = true;
            float enemy_distance;
            GameObject enemy = FindClosestEnemy();
            int enemy_health;
            if (enemy)
            {
                enemy_distance = Mathf.Abs(player.transform.position.x - enemy.transform.position.x);
                enemy_distance += Mathf.Abs(player.transform.position.y - enemy.transform.position.y);
                enemy_health = enemy.GetComponent<enemyScript>().health;
            }
            else {
                is_enemy = false;
                enemy_distance = 1000;
                enemy_health = 0;
            }

            GameObject coin = FindClosestCoin();
            bool is_coin = true;
            float coin_distance;
            if (coin)
            {
                coin_distance = Mathf.Abs(player.transform.position.x - coin.transform.position.x);
                coin_distance += Mathf.Abs(player.transform.position.y - coin.transform.position.y);

            } else
            {
                is_coin = false;
                coin_distance = 1000;
            }
            GameObject exit = GameObject.FindGameObjectWithTag("Exit");
            float goal_distance;
            goal_distance = Mathf.Abs(player.transform.position.x - exit.transform.position.x);
            goal_distance += Mathf.Abs(player.transform.position.y - exit.transform.position.y);
            bool possible;
            possible = action.check_conditions(player_script.health, enemy_distance, enemy_health, goal_distance, coin_distance, is_coin, is_enemy);
            
            if (possible)
            {
                int decision = action.get_action();
                if((decision == 1 || decision == 3 || decision == 4 || decision == 5) && !enemy)
                {
                    //randomizar esto un poco
                    decision = 2;
                }
                bool moved = false;
                int move_tries = 0;
                System.Random r = new System.Random();
                switch (decision)
                {
                    case 0:
                        //Debug.Log("random move");
                        
                        
                        while(!moved && move_tries < 10)
                        {
                            move_tries++;
                            moved = player_script.move(r.Next(0, 4));
                        }

                        break;
                    case 1:
                        //Debug.Log("move to enemy");
                        int enemy_dir = 0;
                        Vector2 enemy_pos;
                        if(enemy.transform.parent)
                        {
                            enemy_pos = new Vector2(enemy.transform.parent.position.x, enemy.transform.parent.position.y);
                        } else
                        {
                            enemy_pos = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                        }
                        float enemy_dist_x = Mathf.Abs(enemy_pos.x - player.transform.position.x);
                        float enemy_dist_y = Mathf.Abs(enemy_pos.y - player.transform.position.y);
                        //print(enemy_dist_x);
                        //print(enemy_dist_y);
                        if (enemy_dist_x < 1.5 && enemy_dist_y < 1.5)
                        {
                            //Debug.Log("attack enemy not move");
                            player_script.point_dir = get_enemy_dir(enemy);
                            //print("enemy dir: " + player_script.point_dir);
                            player_script.create_pointer();
                            player_script.attack_close(player_script.point_dir);
                        } else
                        {
                            enemy_dir = player_script.a_star.get_next_move(enemy_pos);
                            //----------------------------------------
                            player_script.move(enemy_dir);
                        }
                        
                       
                        break;
                    case 2:
                        //Debug.Log("move to goal");
                        int goal_dir = 0;
                        Vector2 goal_pos = new Vector2(exit.transform.position.x, exit.transform.position.y);
                        goal_dir = player_script.a_star.get_next_move(goal_pos);
                        //----------------------------------------
                        moved = player_script.move(goal_dir);
                        while (!moved && move_tries < 10)
                        {
                            move_tries++;
                            moved = player_script.move(r.Next(0, 4));
                        }
                        break;
                    case 3:
                        //Debug.Log("run from enemy");
                        int opposite_to_enemy_dir = 0;
                        int enemy_dir2 = 0;
                        Vector2 enemy_pos2 = new Vector2(exit.transform.position.x, exit.transform.position.y);
                        enemy_dir2 = player_script.a_star.get_next_move(enemy_pos2);
                        switch(enemy_dir2)
                        {
                            case 0:
                                opposite_to_enemy_dir = 2;
                                break;
                            case 1:
                                opposite_to_enemy_dir = 3;
                                break;
                            case 2:
                                opposite_to_enemy_dir = 0;
                                break;
                            case 3:
                                opposite_to_enemy_dir = 1;
                                break;

                        }
                        //----------------------------------------
                        moved = player_script.move(opposite_to_enemy_dir);
                        while (!moved && move_tries < 10)
                        {
                            move_tries++;
                            moved = player_script.move(r.Next(0, 4));
                        }
                        break;
                    case 4:
                        
                        //Debug.Log("move to goal");
                        int coin_dir = 0;
                        Vector2 coin_pos = new Vector2(coin.transform.position.x, coin.transform.position.y);
                        goal_dir = player_script.a_star.get_next_move(coin_pos);
                        //----------------------------------------
                        moved = player_script.move(goal_dir);
                        while (!moved && move_tries < 10)
                        {
                            move_tries++;
                            moved = player_script.move(r.Next(0, 4));
                        }
                        break;
                    case 5:
                        //Debug.Log("attack enemy range");
                        player_script.point_dir = get_enemy_dir(enemy);
                        //print("enemy dir: " + player_script.point_dir);
                        player_script.create_pointer();
                        player_script.attack_range(player_script.point_dir);

                        break;
                    default:
                        break;
                }
            }
        
        }

        return 0;
    }
    int get_enemy_dir(GameObject enemy)
    {
        float dist_x = enemy.transform.position.x - player.transform.position.x;
        float dist_y = enemy.transform.position.y - player.transform.position.y;
        //print(dist_y);
        //print(dist_x);
        if (dist_y < 0.9 && dist_y > -0.9)
        {
            if(dist_x < 0)
            {
                return 3;
            } else
            {
                return 1;
            }
        }
        else if (dist_x < 0.9 && dist_x > -0.9)
        {
            if (dist_y < 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        else if (dist_x < 0 && dist_y < 0)
        {
            return 6;
        }
        else if (dist_x > 0 && dist_y < 0)
        {
            return 7;
        }
        else if (dist_x < 0 && dist_y > 0)
        {
            return 5;
        }
        else if (dist_x > 0 && dist_y > 0)
        {
            return 4;
        }
        return 0;
    }
}
