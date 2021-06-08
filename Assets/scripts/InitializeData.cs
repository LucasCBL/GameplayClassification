using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class InitializeData : MonoBehaviour
{
    public bool is_bot = false;
    // Start is called before the first frame update
    public void initialize()
    {
        if(is_bot)
        {
            PlayerData.is_bot = true;
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "\\bots\\");
            string dir_s = Application.dataPath + "\\bots\\";
            FileInfo[] info = dir.GetFiles("*.txt");
            PlayerData.bot_id = Random.Range(0, info.Length);
            
            string bot_name = info[PlayerData.bot_id].ToString();
            //print(dir_s.Length + "   " + (bot_name.Length - dir_s.Length - 4));
            //print(bot_name.Length);
            bot_name = bot_name.Substring(dir_s.Length, (bot_name.Length - dir_s.Length - 4));
            
            PlayerData.p_name = bot_name + "_bot_" + Random.Range(0, 1000000);
            print("bot_ name = " + PlayerData.p_name);
        } else
        {
            PlayerData.is_bot = false;
            PlayerData.bot_id = 0;
            PlayerData.p_name = "player_" + Random.Range(0, 100000);
            print("player_name = " + PlayerData.p_name);
        }
    }
 

}
