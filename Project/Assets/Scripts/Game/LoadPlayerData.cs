using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int HP;
    public int MaxHP;
    public int Attack;
    public int Defense;
    public int Level;
    public int CurrentExp;
    public int RequiredExp;
    public List<PlayerSkillData> Skills = new List<PlayerSkillData>();
    public List<PlayerInvertory> Inventory = new List<PlayerInvertory>();
    public Vector3 PlayerPosition;
    public List<int> interactiveItemsID = new List<int>();
    public string currentTime;

    public void AddExperience(int exp)
    {
        CurrentExp += exp;
        while (CurrentExp >= RequiredExp)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        Level++;
        CurrentExp -= RequiredExp;
        RequiredExp = Mathf.RoundToInt(20 * Mathf.Pow(Level, 1.5f));
        MaxHP += (int)(3 * 1.1f);
        HP = MaxHP;
        Attack += (int)(2 * 1.1f);
        Defense += (int)(1 * 1.1f);
        Debug.Log("Leveled Up to Level " + Level);
    }

    public void SetDefaultValue()
    {
        HP = 20;
        MaxHP = 20;
        Attack = 5;
        Defense = 3;
        Level = 1;
        CurrentExp = 0;
        RequiredExp = 20;
        PlayerPosition = new Vector3(-3.62f,1,3.34f);
        Skills.Add(new PlayerSkillData { name = "God's Slap", type = "group", damageMultiplier = 1.0f });
    }
}

[System.Serializable]
public class PlayerSkillData
{
    public string name;
    public string type;
    public float damageMultiplier;
}

[System.Serializable]
public class PlayerInvertory
{
    public string itemName;
    public int itemID;
    public int quantity;
    public string description;
    public string effect;
    public bool isUsable;
}

public class LoadPlayerData : MonoBehaviour
{
    public PlayerData data = new PlayerData();

    private void Start()
    {
        Invoke("UpdatePosition", 0.01f);
    }

    void UpdatePosition() 
    {
        gameObject.transform.position = data.PlayerPosition;
    }

    public void HealPlayer(int healAmount)
    {
        data.HP += healAmount;
        if (data.HP > data.MaxHP)
        {
            data.HP = data.MaxHP;
        }
        Debug.Log("Player Healed by " + healAmount + ". Current HP: " + data.HP);
    }

    public void takeDamage(int damage)
    {
        data.HP -= damage;
        if (data.HP < 0)
        {
            data.HP = 0;
        }
        Debug.Log($"Player took {damage} damage. Current HP: " + data.HP);
    }
}
