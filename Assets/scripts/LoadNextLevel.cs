using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //So you can use SceneManager

public class LoadNextLevel : MonoBehaviour
{
    public bool first_level = false;
    public int next_scene = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print(collision);
        if(collision.gameObject.tag == "Player")
        {
            PlayerMovement script = collision.gameObject.GetComponent<PlayerMovement>();
            if (first_level)
            {
                gameObject.GetComponent<InitializeData>().initialize();
            } else
            {
                script.save_data("_completed");
            }
            SceneManager.LoadScene(next_scene);
        }
    }
}
