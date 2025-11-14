using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OptionData
{
    public int languageIndex = 0;
    public float musicVolume = 0f;
    public float soundVolume = 0f;
}

public class OptionSetter : MonoBehaviour
{
    [SerializeField]
    private Slider musicSlider, soundSlider;

    [SerializeField]
    private List<Toggle> langugeGroup = new List<Toggle>();

    private float musicVolume, soundVolume;
    private int languageIndex;

    private string filePath;

    OptionData optionData = new OptionData();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "setting.json");

        LoadValue();
    }

    void SetDefaltValue() {
        musicVolume = 0.5f;
        soundVolume = 0.5f;
        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
        langugeGroup[0].isOn = true;

        for (int i = 0; i < langugeGroup.Count; i++)
        {
            if (langugeGroup[i].isOn)
            {
                languageIndex = i;
            }
        }

        optionData.musicVolume = musicVolume;
        optionData.soundVolume = soundVolume;
        optionData.languageIndex = languageIndex;
    }

    public void SaveValue() 
    {
        optionData.musicVolume = musicSlider.value;
        optionData.soundVolume = soundSlider.value;
        for (int i = 0; i < langugeGroup.Count; i++)
        {
            if (langugeGroup[i].isOn)
            {
                languageIndex = i;
            }
        }
        optionData.languageIndex = languageIndex;

        string json = JsonUtility.ToJson(optionData, true);
        Debug.Log(json);
        File.WriteAllText(filePath, json);
        Debug.Log("Save success");
    }

    public void LoadValue() 
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            OptionData data = JsonUtility.FromJson<OptionData>(json);

            musicVolume = data.musicVolume;
            soundVolume = data.soundVolume;
            languageIndex = data.languageIndex;

            musicSlider.value = musicVolume;
            soundSlider.value = soundVolume;
            langugeGroup[languageIndex].isOn = true;
            Debug.Log("Load success");
        }
        else
        {
            Debug.LogWarning("Can't find Option Value");
            SetDefaltValue();
        }
    }
}
