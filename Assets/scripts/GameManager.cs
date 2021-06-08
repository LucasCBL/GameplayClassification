using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject player;
    public bool playerTurn = true;
    public int total_turns = 0;
    public int level = 0;
    private List<GameObject> enemy_list;

    // Start is called before the first frame update
    void Start()
    {
        enemy_list = new List<GameObject>( GameObject.FindGameObjectsWithTag("Enemy"));
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool playerCanWalk(int dir)
    {
        Vector2 direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        LayerMask mask1 = LayerMask.GetMask("wall");
        LayerMask mask2 = LayerMask.GetMask("enemy");
        LayerMask mask = mask1 | mask2;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 1.0f, mask);
        if(hit.collider != null)
        {
            if(hit.collider.tag == "wall" || hit.collider.tag == "enemy")
            {
                return false;
            }
            return false;
        }
        return true;
    }

    public bool closeUpEnemy(int dir)
    {
        Vector2 direction;
        if (dir < 4)
        {
            direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        }
        else
        {
            direction = new Vector2((dir == 4 || dir == 7) ? 0.5f : -0.5f, (dir == 4 || dir == 5) ? 0.5f : -0.5f);
        }
        LayerMask mask = LayerMask.GetMask("enemy");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 1.0f, mask);
        if (hit.collider != null)
        {
            // //print(hit.collider);
            if (hit.collider.tag == "Enemy")
            {
                return true;
            }
            return false;
        }
        // //print(direction);
        return false;
    }
    public GameObject getCloseUpEnemy(int dir)
    {
        Vector2 direction;
        if (dir < 4)
        {
            direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        }
        else
        {
            direction = new Vector2((dir == 4 || dir == 7) ? 0.5f : -0.5f, (dir == 4 || dir == 5) ? 0.5f : -0.5f);
        }
        LayerMask mask = LayerMask.GetMask("enemy");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 1.0f, mask);
        if (hit.collider != null)
        {
            // //print(hit.collider);
            if (hit.collider.tag == "Enemy")
            {
                 //print("returning " + hit.transform.gameObject);
                return hit.transform.gameObject;
            }
            return null;
        }
        // //print(direction);
        return null;
    }

    public GameObject getRangeEnemy(int dir)
    {
        Vector2 direction;
        if(dir < 4)
        {
            direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        } else
        {
            direction = new Vector2((dir == 4 || dir == 7) ? 0.5f : -0.5f, (dir == 4 || dir == 5) ? 0.5f : -0.5f);
        }
        LayerMask mask = LayerMask.GetMask("enemy");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 6.0f, mask);
        if (hit.collider != null)
        {
             //print(hit.collider);
            if (hit.collider.tag == "Enemy")
            {
                return hit.transform.gameObject;
            }
            return null;
        }
         //print(direction);
        return null;
    }
    public bool rangeEnemy(int dir)
    {
        Vector2 direction;
        if (dir < 4)
        {
            direction = new Vector2(dir == 1 ? 1 : (dir == 3 ? -1 : 0), dir == 0 ? 1 : (dir == 2 ? -1 : 0));
        }
        else
        {
            direction = new Vector2((dir == 4 || dir == 7) ? 0.5f : -0.5f, (dir == 4 || dir == 5) ? 0.5f : -0.5f);
        }
        LayerMask mask = LayerMask.GetMask("enemy");
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, 5.0f, mask);
        if (hit.collider != null)
        {
             //print(hit.collider);
            if (hit.collider.tag == "Enemy")
            {
                return true;
            }
            return false;
        }
         //print(direction);
        return false;
    }

    public void playerTurnEnd()
    {
        total_turns++;
        playerTurn = false;
        enemy_actions();
    }

    void enemy_actions()
    {
        int i = 0;
        if(enemy_list.Count > 0)
        {
            foreach (GameObject enemy in enemy_list.GetRange(0, enemy_list.Count))
            {
                if (enemy)
                {
                    enemyScript script = enemy.GetComponent<enemyScript>();
                    if (script)
                    {
                        script.choose_action();
                    }
                }
                else
                {
                    enemy_list.RemoveAt(i);
                }
                i++;
            }
        }
      
    }

    public void playerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}
