using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerData data;

    [SerializeField]
    private GameObject playerPosition;

    [SerializeField]
    private List<Button> files;

    private string filePath;

    public void SaveGame(int ID)
    {
        string pathName = "save" + ID + ".save";
        filePath = Path.Combine(Application.persistentDataPath, pathName);
        Debug.Log("Saving to: " + filePath);
        data.PlayerPosition = playerPosition.transform.position;


        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Invoke("WriteFile", 1f);
        }
        else
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Game Saved");
        }
    }

    void WriteFile() 
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        
    }
}
