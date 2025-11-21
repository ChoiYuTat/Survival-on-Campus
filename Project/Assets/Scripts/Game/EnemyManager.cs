using System.Collections.Generic;
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
    private Dictionary<int, EnemyData> enemyDict;
    [SerializeField]
    private EnemyGroupSO enemyGroup;

    void Start()
    {
        LoadEnemyData();
    }

    void LoadEnemyData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("enemies");
        enemyDatabase = JsonUtility.FromJson<EnemyDatabase>(jsonFile.text);

        enemyDict = new Dictionary<int, EnemyData>();
        foreach (var enemy in enemyDatabase.enemies)
        {
            enemyDict[enemy.id] = enemy;
        }

        for (int i = 0; i < enemyGroup.enemies.Length; i++)
        {
            if (enemyDict.ContainsKey(enemyGroup.enemies[i].id)) 
            {
                enemyGroup.enemies[i] = enemyDict[enemyGroup.enemies[i].id];
            }
        }
    }

    public List<EnemyData> getEnemyData()
    {
        bool allNull = true;
        foreach (var enemy in enemyGroup.enemies)
        {
            if (enemy != null)
            {
                allNull = false;
            }
        }

        if (!allNull)
            return new List<EnemyData>(enemyGroup.enemies);
        else return null;
    }

    public EnemyData GetEnemyById(int id)
    {
        return enemyDict.ContainsKey(id) ? enemyDict[id] : null;
    }
}
