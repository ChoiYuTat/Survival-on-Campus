using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[System.Serializable]
public class SkillData
{
    public string name;
    public float damageMultiplier;
    public float duration;
}

[System.Serializable]
public class EnemyData
{
    public int id;
    public string name;
    public int hp;
    public int attack;
    public int defense;
    public int exp;
    public SkillData[] skills;
}

[System.Serializable]
public class EnemyDatabase
{
    public EnemyData[] enemies;
}

public class EnemyManager : MonoBehaviour
{
    private EnemyDatabase enemyDatabase;

    void Start()
    {
        LoadEnemyData();
        PrintEnemyInfo(1); 
    }

    void LoadEnemyData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("enemies");
        enemyDatabase = JsonUtility.FromJson<EnemyDatabase>(jsonFile.text);
    }

    public EnemyData GetEnemyById(int id)
    {
        foreach (var enemy in enemyDatabase.enemies)
        {
            if (enemy.id == id)
                return enemy;
        }
        return null;
    }

    void PrintEnemyInfo(int id)
    {
        EnemyData enemy = GetEnemyById(id);
        if (enemy != null)
        {
            Debug.Log($"敌人: {enemy.name}, HP: {enemy.hp}, 攻击: {enemy.attack}, 经验: {enemy.exp}");
            foreach (var skill in enemy.skills)
            {
                Debug.Log($"技能: {skill.name}, 持续时间: {skill.duration}");
            }
        }
    }
}
