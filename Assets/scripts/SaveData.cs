using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveData : MonoBehaviour
{
    // Start is called before the first frame update
    static public void SaveToFile(string player_name, int level, string filename, DataStruct data)
    {

        print("saving data for: " + player_name);
        // The target file path e.g.
#if UNITY_EDITOR
        string folder = "Assets";
        string json = JsonUtility.ToJson(data);
        folder = folder + "/OUTPUT DATA/" + player_name + "/level_" + level;
        Directory.CreateDirectory(folder);
        string file_path = folder + "/" + filename + ".json";
        File.WriteAllText(file_path, json);
        AssetDatabase.Refresh();
#else
        string folder = Application.persistentDataPath;
        string json = JsonUtility.ToJson(data);
        folder = folder + "/OUTPUT DATA/" + player_name + "/level_" + level ;
        Directory.CreateDirectory(folder);
        string file_path = folder + "/" + filename;
        File.Create(file_path);

        StreamWriter writer = new StreamWriter(file_path, true);
        writer.Write(json);
#endif
    }
}
