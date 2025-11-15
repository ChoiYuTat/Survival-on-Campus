using UnityEngine;

public class ExpTableGenerator : MonoBehaviour
{
    [SerializeField]
    private int maxLevel = 20;
    [SerializeField]
    private int baseExp = 100;   
    [SerializeField]
    private float growthRate = 1.5f;

    [SerializeField]
    private int hp = 20;
    [SerializeField]
    private int atk = 3;
    [SerializeField]
    private int def = 2;

    [SerializeField]
    private int hpGrowth = 3;
    [SerializeField]
    private int attackGrowth = 2;
    [SerializeField]
    private int defenseGrowth = 2;

    [SerializeField]
    private float grouthMultiplier = 1.1f;


    void Start()
    {
        GenerateExpTable();
    }

    void GenerateExpTable()
    {
        Debug.Log("===== 经验表生成器 =====");
        for (int level = 1; level <= maxLevel; level++)
        {
            Debug.Log($"现数值：HP: {hp} 攻击力: {atk} 防御力: {def}");
            int requiredExp = Mathf.RoundToInt(baseExp * Mathf.Pow(level, growthRate));
            int currentHP = (int)((hp + hpGrowth) * grouthMultiplier);
            hp = currentHP;
            int currentATK = (int)((atk + attackGrowth) * grouthMultiplier);
            atk = currentATK;
            int currentDEF = (int)((def + defenseGrowth) * grouthMultiplier);
            def = currentDEF;
            Debug.Log($"等级 {level} → 升级所需经验: {requiredExp}");
            Debug.Log($"升级后数值：HP: {currentHP} 攻击力: {currentATK} 防御力: {currentDEF}");
        }
    }
}

