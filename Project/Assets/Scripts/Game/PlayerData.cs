using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : ScriptableObject
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
}

[System.Serializable]
public class PlayerSkillData
{
    public string name;
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
