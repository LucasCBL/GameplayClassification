using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin_script : MonoBehaviour
{
    public int value = 1;
    // Start is called before the first frame update

    public void pick_up()
    {
        Destroy(gameObject);
    }
}
