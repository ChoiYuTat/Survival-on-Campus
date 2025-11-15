using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public int HP;
    public int Attack;
    public int Defense;
    public int Level;
    public PlayerSkillData[] Skills;
}

public class PlayerSkillData
{
    public string name;
    public float damageMultiplier;
}
