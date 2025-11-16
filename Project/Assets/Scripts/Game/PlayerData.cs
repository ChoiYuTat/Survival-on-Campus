using Microsoft.Unity.VisualStudio.Editor;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
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
}

public class PlayerSkillData
{
    public string name;
    public float damageMultiplier;
}

public class PlayerInvertory
{
    public string itemName;
    public int itemID;
    public int quantity;
    public string description;
    public string effect;
    public bool isUsable;
}
