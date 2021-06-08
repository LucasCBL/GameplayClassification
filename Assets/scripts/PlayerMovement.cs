using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class PlayerMovement : MonoBehaviour
{
    private bool manualMovementOnCooldown = false;
    private bool botMovementOnCooldown = false;
    public int next_level;
    public float cooldown = 1;
    public float bot_cooldown = 0.05f;
    public int point_dir = 1;
    public int health = 100;
    public int max_health = 100;
    public int regen = 1;
    private GameObject pointer_prefab;
    private GameObject pointer;
    public bool is_bot = false;
    public TMP_Text health_ui;

    private int death_toll = 0;
    // stats conseguidos a lo largo de una vida, si el jugador no muere
    // coincidirá con los totales
    public int current_damage_dealt = 0;
    public int current_health_lost = 0;
    public int current_kills = 0;
    public int current_movements = 0;
    public int current_health_regen = 0;
    public int current_coins = 0;
    public int current_points = 0;
    public int ranged_kills; // enemigos matados usando ataques de rango
    public int ranged_enem_kills; // enemigos que atacan a rango matados
    public int damage_dealt_range;
    public int health_lost_range;
    public int close_kills; // enemigos matados cuerpo a cuerpo
    public int close_enem_kills; // enemigos que atacan cuerpo a cuerpo matados
    public int damage_dealt_close;
    public int health_lost_close;

    private bool player_dead = false;

    public string p_name = "player";
    public AStar a_star;
    public BotBehaviours bot;

    
    // Start is called before the first frame update
    void Start()
    {
        p_name = PlayerData.p_name;
        is_bot = is_bot || PlayerData.is_bot;
        pointer_prefab = Resources.Load("selected") as GameObject;
        health_ui = GetComponentInChildren<TMP_Text>();
        create_pointer();
        manualCooldown();
        a_star = new AStar(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "coin")
        {
            coin_script coin = col.gameObject.GetComponent<coin_script>();
            current_coins += coin.value;
            current_points += coin.value * 100;
            coin.pick_up();
        }
    }

    void update_health()
    {
        health_ui.text = health.ToString() + '/' + max_health.ToString();
    }
    private void movement()
    {
        current_movements++;
        if(health < max_health)
        {
            current_points -= 2;
        }
        health += regen;
        if(health > max_health)
        {
            int max_regen = health - max_health;
            health = max_health;
            current_health_regen += max_regen;
        } else
        {
            current_health_regen += regen;
        }
        update_health();
        
        
    }


    public void create_pointer()
    {
        if (pointer)
        {
            pointer.transform.parent = null;
            Destroy(pointer);
        }
        pointer = Instantiate(pointer_prefab);
        Vector3 direction;
        if (point_dir < 4)
        {
            direction = new Vector3(point_dir == 1 ? 1 : (point_dir == 3 ? -1 : 0), point_dir == 0 ? 1 : (point_dir == 2 ? -1 : 0), 0);
        }
        else
        {
            direction = new Vector3((point_dir == 4 || point_dir == 7) ? 1 : -1, (point_dir == 4 || point_dir == 5) ? 1 : -1, 0);
        }
        pointer.transform.position = transform.position;
        pointer.transform.parent = transform;
        pointer.transform.position += direction;

    }

    public bool move(int dir)
    {

        switch (dir)
        {
            case 0:
                if(GameManager.instance.playerCanWalk(0))
                {

                    movement();
                    transform.position = transform.position + new Vector3(0, 1, 0);
                    GameManager.instance.playerTurnEnd();
                    return true;
                }
                return false;
            case 1:
                if (GameManager.instance.playerCanWalk(1))
                {
                    movement();
                    transform.position = transform.position + new Vector3(1, 0, 0);
                    GameManager.instance.playerTurnEnd();
                    return true;
                }
                return false;
            case 2:
                if (GameManager.instance.playerCanWalk(2))
                {
                    movement();
                    transform.position = transform.position + new Vector3(0, -1, 0);
                    GameManager.instance.playerTurnEnd();
                    return true;
                }
                return false;
            case 3:
                if (GameManager.instance.playerCanWalk(3))
                {
                    movement();
                    transform.position = transform.position + new Vector3(-1, 0, 0);
                    GameManager.instance.playerTurnEnd();
                    return true;
                }
                return false;
            default:
                return false;
        }
    }
    public void damage(int dmg, bool ranged)
    {
        if(!player_dead)
        {
                health -= dmg;
                current_health_lost += dmg;
                health_lost_close += ranged ? 0 : dmg;
                health_lost_range += ranged ? dmg : 0;
                if (health <= 0)
                {
                    this.enabled = false;
                    current_points -= 1000;
                    save_data("_death");
                    if(!player_dead)
                    {
                        PlayerData.death_toll++;
                    }
                    player_dead = true;
                    
                    if (PlayerData.death_toll >= 10) {
                        print("death number 10 toll:" + PlayerData.death_toll);
                        PlayerData.death_toll = 0;
                        SceneManager.LoadScene(next_level);
                    }else
                    {
                        print(PlayerData.death_toll);
                        GameManager.instance.playerDeath();
                    }

                }
                update_health();
        }

    }
    // FUNCION PARA GUARDAR LOS LOGS
    public void save_data(string save_reason)
    {
        DataStruct data = new DataStruct();
        data.turns = GameManager.instance.total_turns;
        data.damage_dealt = current_damage_dealt;
        data.health_lost = current_health_lost;
        data.health_regen = current_health_regen;
        data.kills = current_kills;
        data.movements = current_movements;
        data.coins = current_coins;
        data.points = current_points;
        data.health_lost_close = health_lost_close;
        data.health_lost_range = health_lost_range;
        data.close_enem_killed = close_enem_kills;
        data.ranged_enem_killed = ranged_enem_kills;
        data.close_kills = close_kills;
        data.ranged_kills = ranged_kills;
        data.damage_dealt_close = damage_dealt_close;
        data.damage_dealt_range = damage_dealt_range;
        data.prior_deaths = PlayerData.death_toll;
        data.death = (save_reason == "_death");
        if(!data.death)
        {
            print("Completed level, death toll = " + PlayerData.death_toll);
            PlayerData.death_toll = 0;
            print("reset death toll = " + PlayerData.death_toll);
        }
        string date = (System.DateTime.Now.ToString().Replace('/', '_'));
        date = (date.Replace(' ', '_'));
        date = (date.Replace(':', '_'));
        SaveData.SaveToFile(p_name, GameManager.instance.level,date + save_reason, data);
    }
    public void attack_close(int dir)
    {
        switch (dir)
        {
            case 0:
                if (GameManager.instance.closeUpEnemy(dir))
                {
                    GameObject target = GameManager.instance.getCloseUpEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 1:
                if (GameManager.instance.closeUpEnemy(dir))
                {
                    GameObject target = GameManager.instance.getCloseUpEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 2:
                if (GameManager.instance.closeUpEnemy(dir))
                {
                    GameObject target = GameManager.instance.getCloseUpEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if(kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 3:
                if (GameManager.instance.closeUpEnemy(dir))
                {
                    GameObject target = GameManager.instance.getCloseUpEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 4:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 5:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 6:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }

                break;
            case 7:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(10);
                    if (kill)
                    {
                        current_kills++;
                        close_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 10;
                    damage_dealt_close += 10;
                }
                break; 
            default:
                break;
        }
        GameManager.instance.playerTurnEnd();
    }

    public void attack_range(int dir)
    {
        switch (dir)
        {
            case 0:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 1:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 2:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 3:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 4:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 5:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 6:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            case 7:
                if (GameManager.instance.rangeEnemy(dir))
                {
                    GameObject target = GameManager.instance.getRangeEnemy(dir);
                    bool kill = target.GetComponent<enemyScript>().damage(3);
                    if (kill)
                    {
                        current_kills++;
                        ranged_kills++;
                        ranged_enem_kills += target.GetComponent<enemyScript>().ranged ? 1 : 0;
                        close_enem_kills += target.GetComponent<enemyScript>().ranged ? 0 : 1;
                        current_points += 500;
                    }
                    current_damage_dealt += 3;
                    damage_dealt_range += 3;
                }

                break;
            default:
                break;
        }
        GameManager.instance.playerTurnEnd();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            is_bot = !is_bot;
        } else if (!manualMovementOnCooldown)
        {
            if (Input.GetKey("w"))
            {
                move(0);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                point_dir = 0;
                create_pointer();
                StartCoroutine("manualCooldown");
            } else if (Input.GetKey("d"))
            {
                move(1);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                point_dir = 1;
                create_pointer();
                StartCoroutine("manualCooldown");
            } else if (Input.GetKey("s"))
            {
                move(2);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                point_dir = 2;
                create_pointer();
                StartCoroutine("manualCooldown");
            } else if (Input.GetKey("a"))
            {
                move(3);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                point_dir = 3;
                create_pointer();
                StartCoroutine("manualCooldown");
            }else if(Input.GetKey("space"))
            {
                GameObject exit = GameObject.FindGameObjectWithTag("Exit");
                int move_dir = 0;
                Vector2 goal_pos = new Vector2(exit.transform.position.x, exit.transform.position.y);
                move_dir = a_star.get_next_move(goal_pos);
                //----------------------------------------
                move(move_dir);
                manualMovementOnCooldown = true;
                point_dir = move_dir;
                create_pointer();
                StartCoroutine("manualCooldown");
            }else if (Input.GetKey("b"))
            {
                bot.get_decision();
                manualMovementOnCooldown = true;
                StartCoroutine("manualCooldown");
            }else if (Input.GetKey("e"))
            {
                attack_close(point_dir);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                StartCoroutine("manualCooldown");
            }
            else if (Input.GetKey("q"))
            {
                attack_range(point_dir);
                //Debug.Log("cooldown BEGIN");
                manualMovementOnCooldown = true;
                StartCoroutine("manualCooldown");
            }
        }

        if(is_bot && !botMovementOnCooldown)
        {
            bot.get_decision();
            botMovementOnCooldown = true;
            StartCoroutine("botCooldown");
        }


        /// POINTERS, NECESITA MEJORA MUCHO
        if (Input.GetKey("up") && Input.GetKey("right"))
        {
            point_dir = 4;
            create_pointer();
        }
        else if (Input.GetKey("right") && Input.GetKey("down"))
        {
            point_dir = 7;
            create_pointer();
        }
        else if (Input.GetKey("down") && Input.GetKey("left"))
        {
            point_dir = 6;
            create_pointer();
        }
        else if (Input.GetKey("left") && Input.GetKey("up"))
        {
            point_dir = 5;
            create_pointer();
        }
        else if (Input.GetKey("up"))
        {
            point_dir = 0;
            create_pointer();
        }
        else if (Input.GetKey("right"))
        {
            point_dir = 1;
            create_pointer();
        }
        else if (Input.GetKey("down"))
        {
            point_dir = 2;
            create_pointer();
        }
        else if (Input.GetKey("left"))
        {
            point_dir = 3;
            create_pointer();
        }

    }

    private IEnumerator manualCooldown()
    {
        //Debug.Log("cooldown BEGIN");
        yield return new WaitForSeconds(cooldown);
        manualMovementOnCooldown = false;
        //Debug.Log("cooldown over");
    }

    private IEnumerator botCooldown()
    {
        //Debug.Log("cooldown BEGIN");
        yield return new WaitForSeconds(bot_cooldown);
        botMovementOnCooldown = false;
        //Debug.Log("cooldown over");
    }
}
