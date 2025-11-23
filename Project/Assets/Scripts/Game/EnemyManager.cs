using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
[CreateAssetMenu(menuName = "Enemy/ActionConfig", fileName = "NewEnemyActionConfig")]
public class EnemyActionConfig : ScriptableObject
{
    [Header("技能信息")]
    public string skillName;                // 与 SkillData.name 对应

    [Header("普通攻击演出参数")]
    public int attackCount = 1;             // 普通攻击次数
    public float approachDistance = 2f;     // 安全距离
    public float attackDistance = 0.5f;     // 攻击距离
    public bool changeColorBeforeAttack = true; // 是否攻击前变色
    public bool returnToOrigin = true;      // 攻击后是否返回原位

    [Header("跳跃攻击演出参数")]
    public bool useJumpAttack = false;      // 是否启用跳跃攻击
    public float jumpHeight = 2f;           // 跳跃高度
    public float jumpDuration = 1f;         // 跳跃总时长
}

[System.Serializable]
public class SkillData
{
    public string name;
    public float damageMultiplier;
    public float duration;
}

[System.Serializable]
public struct EnemyData
{
    public int id;
    public string name;
    public int hp;
    public int maxHp;
    public int attack;
    public int defense;
    public int exp;
    public int instanceID;
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

    public EnemyGroupSO enemyGroup;

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

        for (int i = 0; i < enemyGroup.enemies.Count; i++)
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
            if (IsValid(enemy))
            {
                allNull = false;
            }
        }

        if (!allNull)
            return new List<EnemyData>(enemyGroup.enemies);
        else return null;
    }
    public EnemyData? GetEnemyById(int id)
    {
        if (enemyDict.TryGetValue(id, out EnemyData enemy))
        {
            return enemy; 
        }
        return null; 
    }

    public bool IsValid(EnemyData enemy)
    {
        return enemy.id > 0 && !string.IsNullOrEmpty(name);
    }
}
