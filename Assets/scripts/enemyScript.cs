using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class enemyScript : MonoBehaviour
{
    public bool ranged = false;
    public float perception_range = 6.0f;
    public float attack_range = 1.5f;
    public int attack_dmg = 15;
    public int health = 30;
    public int max_health = 30;
    public float x_offset = 0;
    public float y_offset = 0;
    public int selected_action = 2;
    public TMP_Text health_ui;
    private GameObject player;
    Collider2D self_collider;
    // Start is called before the first frame update
    void Start()
    {
        health_ui = GetComponentInChildren<TMP_Text>();
        player = GameObject.Find("Player");
        // used to ignore raycast without ignoring other enemy colliders
        self_collider = GetComponent<Collider2D>();
        update_health();
    }

    void update_health()
    {
        health_ui.text = health.ToString() + '/' + max_health.ToString();
    }

    public bool damage(int dmg)
    {
        health -= dmg;
        if(health <= 0)
        {
            death();
            return true;
            
        }
        update_health();
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void death()
    {
         Destroy(gameObject);
    }
    public void choose_action()
    {
        if(selected_action == 0)
        {
           ////Debug.Log("attack range");
            attack();
        } else if(selected_action == 1)
        {
            ////Debug.Log("perception range");
            move_towards_player();
        } else if(selected_action == 2)
        {
            random_move();
        }

        if(player_in_range(attack_range))
        {
            if (ranged)
            {
                float dist_x = Mathf.Abs(transform.position.x + x_offset - player.transform.position.x);
                float dist_y = Mathf.Abs(transform.position.y + y_offset - player.transform.position.y);
                //print("dist_x = " + dist_x);
                //print("dist_y = " + dist_y);
                //print("dist_y = " + dist_y);
                if (player_in_range(attack_range))
                {
                    if (dist_y < 0.5 || dist_x < 0.5 || Mathf.Abs(dist_y - dist_x) < 0.5)
                    {
                        selected_action = 0;
                    } else
                    {
                        selected_action = 1;
                    }
                }
                else
                {
                    selected_action = 1;
                }
            } else
            {
                selected_action = 0;
            }
            
        } else if(player_in_range(perception_range))
        {
            selected_action = 1;
        }
        else
        {
            selected_action = 2;
        }
    }

    void move_towards_player()
    {
        float dist_x = Mathf.Abs(transform.position.x + x_offset - player.transform.position.x);
        float dist_y = Mathf.Abs(transform.position.y + y_offset - player.transform.position.y);
        if(dist_x < dist_y)
        {
            if((transform.position.y + y_offset - player.transform.position.y) < 0)
            {
                if(can_walk(0))
                {
                    move(0);
                } else
                {
                    if (!player_in_range(1.5f))
                    {
                        random_move();
                    }
                }
            } else
            {
                if (can_walk(2))
                {
                    move(2);
                }
                else
                {
                    if (!player_in_range(1.5f))
                    {
                        random_move();
                    }
                }
            }
        } else
        {
            if((transform.position.x + x_offset - player.transform.position.x) < 0)
            {
                if (can_walk(1))
                {
                    move(1);
                }
                else
                {
                    if (!player_in_range(1.5f))
                    {
                        random_move();
                    }
                }
            } else
            {
                if(can_walk(3))
                {
                    move(3);
                } else
                {
                    if (!player_in_range(1.5f))
                    {
                        random_move();
                    }
                }
            }
        }
    }

    void random_move()
    {
        int dir = Random.Range(0, 5);
        switch(dir)
        {
            case 0:
                if (can_walk(dir))
                {
                    move(dir);
                }
                else
                {
                    random_move();
                }
                break;
            case 1:
                if (can_walk(dir))
                {
                    move(dir);
                }
                else
                {
                    random_move();
                }
                break;
            case 2:
                if (can_walk(dir))
                {
                    move(dir);
                }
                else
                {
                    random_move();
                }
                break;
            case 3:
                if (can_walk(dir))
                {
                    move(dir);
                }
                else
                {
                    random_move();
                }
                break;
            case 4:
                break;
            default:
                break;
        }
        
    }

    bool can_walk(int dir)
    {
        Vector2 direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        LayerMask mask1 = LayerMask.GetMask("wall");
        LayerMask mask2 = LayerMask.GetMask("enemy");
        LayerMask mask3= LayerMask.GetMask("player");
        LayerMask mask = mask1 | mask2 | mask3;
        self_collider.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(x_offset, y_offset, 0), direction, 1.0f, mask);
        self_collider.enabled = true;
        if (hit.collider != null)
        {
            //print("enemy: ");
            //print(hit.collider);
            if (hit.collider.tag == "wall" || hit.collider.tag == "Enemy" || hit.collider.tag == "Player")
            {
                return false;
            }
            return false;
        }
        return true;
    }

    void move(int dir)
    {
        switch (dir)
        {
            case 0:
                if (can_walk(dir))
                {
                    if(transform.parent)
                    {
                        transform.parent.position = transform.parent.position + new Vector3(0, 1, 0);
                    } else
                    {
                        transform.position = transform.position + new Vector3(0, 1, 0);
                    }
                }

                break;
            case 1:
                if (can_walk(dir))
                {
                    if (transform.parent)
                    {
                        transform.parent.position = transform.parent.position + new Vector3(1, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position + new Vector3(1, 0, 0);
                    }
                }

                break;
            case 2:
                if (can_walk(dir))
                {
                    if (transform.parent)
                    {
                        transform.parent.position = transform.parent.position + new Vector3(0, -1, 0);
                    }
                    else
                    {
                        transform.position = transform.position + new Vector3(0, -1, 0);
                    }
                }

                break;
            case 3:
                if (can_walk(dir))
                {
                    if (transform.parent)
                    {
                        transform.parent.position = transform.parent.position + new Vector3(-1, 0, 0);
                    }
                    else
                    {
                        transform.position = transform.position + new Vector3(-1, 0, 0);
                    }
                    
                }

                break;
            default:
                break;
        }
    }
    void attack()
    {
        /// ESTO NECESITA EDICION DOCUMENTACION Y PRUEBAS, PROBABLEMENTE FUNCIONE EN GENERAL, PERO ES UNA SOLUCION BURDA
        /// SI HAY TIEMPO CAMBIAR A USAR RAYCAST DE SER POSIBLE
        if(ranged)
        {
            float dist_x = Mathf.Abs(transform.position.x + x_offset - player.transform.position.x);
            float dist_y = Mathf.Abs(transform.position.y + y_offset - player.transform.position.y);
            //print(dist_y);
            //print(dist_x);
            if(player_in_range(attack_range))
            {
                if (dist_y < 0.5)
                {
                    player.GetComponent<PlayerMovement>().damage(attack_dmg, ranged);
                }
                else if (dist_x < 0.5)
                {
                    player.GetComponent<PlayerMovement>().damage(attack_dmg, ranged);
                }
                else if (Mathf.Abs(dist_y - dist_x) < 0.5)
                {
                    player.GetComponent<PlayerMovement>().damage(attack_dmg, ranged);
                }
            }
            
        } else
        {
            if (player_in_range(attack_range)) {
                player.GetComponent<PlayerMovement>().damage(attack_dmg, ranged);
            }
        }

    }
    public bool player_in_range(float range)
    {
        if (player)
        {
            float dist_sqr_x = Mathf.Pow(Mathf.Abs(transform.position.x + x_offset - player.transform.position.x), 2);
            float dist_sqr_y = Mathf.Pow(Mathf.Abs(transform.position.y + y_offset - player.transform.position.y), 2);
            float dist = Mathf.Sqrt(dist_sqr_x + dist_sqr_y);
            if(dist <= range)
            {
                return true;
            }
            return false;        
        }
        return false;
    }
}
