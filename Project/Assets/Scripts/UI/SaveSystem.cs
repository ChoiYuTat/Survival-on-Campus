using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    [SerializeField]
    private LoadPlayerData player;

    [SerializeField]
    private List<Button> files;

    [SerializeField]
    private List<TMP_Text> fileTime;

    private string filePath;

    public void SaveGame(int ID)
    {
        string pathName = "save" + ID + ".save";
        filePath = Path.Combine(Application.persistentDataPath, pathName);
        Debug.Log("Saving to: " + filePath);
        player.data.PlayerPosition = player.transform.position;
        fileTime[ID].text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log("Updated Save Time: " + fileTime[ID].text);
        player.data.currentTime = fileTime[ID].text;


        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Invoke("WriteFile", 0.01f);
        }
        else
        {
            string json = JsonUtility.ToJson(player.data, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Game Saved");
        }
    }

    void WriteFile() 
    {
        string json = JsonUtility.ToJson(player.data, true);
        File.WriteAllText(filePath, json);
    }
}
