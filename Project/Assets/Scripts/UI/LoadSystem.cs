using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TimeStamp
{
    public string currentTime;
}

public class LoadSystem : MonoBehaviour
{
    private string filePath;
    private const int fileCount = 8;

    [SerializeField]
    private TMP_Text[] FileTime;

    [SerializeField]
    private Button[] LoadButtons;

    [SerializeField]
    private LoadPlayerData player;

    [SerializeField]
    private GameObject saveIDPrefab;

    void Start()
    {        
        for (int i = 0; i < fileCount; i++)
        {
            filePath = Path.Combine(Application.persistentDataPath, "save" + i + ".save");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                TimeStamp timeList = JsonUtility.FromJson<TimeStamp>(json);
                FileTime[i].text = timeList.currentTime;
                Debug.Log("Loaded Time for file " + i + ": " + timeList.currentTime);
            }
            else if (LoadButtons != null)
            {
                LoadButtons[i].interactable = false;
                FileTime[i].text = "No Data";
            }
            else 
            {
                FileTime[i].text = "No Data";
            }
        }

        LoadData();
    }

    public void LoadGame(int ID) 
    {
        GameObject saveID = Instantiate(saveIDPrefab);
        saveID.GetComponent<SaveID>().SetSaveID(ID);
    }

    private void LoadData()
    {
        if (GameObject.FindGameObjectWithTag("SaveID") && (player != null))
        {
            GameObject SaveObject = GameObject.FindGameObjectWithTag("SaveID");
            int saveID = GameObject.FindGameObjectWithTag("SaveID").GetComponent<SaveID>().GetSaveID();
            filePath = Path.Combine(Application.persistentDataPath, "save" + saveID + ".save");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                player.data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log("Game Loaded from file " + saveID);
            }
            Destroy(SaveObject);
        }
        else if (player != null)
        {
            player.data.SetDefaultValue();
        }
    }
}
