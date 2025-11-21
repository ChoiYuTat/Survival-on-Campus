using UnityEngine;
using System.Collections.Generic;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    BattleOver
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameObject battleScene;
    public GameObject enemyPrefab;
    public GameObject player;
    public GameObject playerPosition;
    public GameObject[] enemyPosition;
    public LoadPlayerData playerData;

    private List<EnemyData> currentEnemies = new List<EnemyData>();
    private List<GameObject> enemies = new List<GameObject>();
    private Dictionary<EnemyData, int> enemySkillIndex = new Dictionary<EnemyData, int>();
    private BattleState state;


    private int currentEnemyIndex = 0;

    //void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    public void StartBattle(List<EnemyData> enemies)
    {
        battleScene.SetActive(true);
        currentEnemies = enemies;
        state = BattleState.Start;

        foreach (var enemy in currentEnemies)
        {
            enemySkillIndex[enemy] = 0;
        }

        Debug.Log("战斗开始！");
        BeginBattle();
    }

    void BeginBattle()
    {
        player.transform.position = playerPosition.transform.position;
        player.GetComponent<luna>().enabled = false;
        player.GetComponent<OpenDoor>().enabled = false;
        for (int i = 0; i < currentEnemies.Count; i++) 
        {
            enemies.Add(Instantiate(enemyPrefab, enemyPosition[i].transform.position, 
                enemyPosition[i].transform.rotation, enemyPosition[i].transform));
            enemies[i].GetComponent<Enemy>().SetEnemyData(currentEnemies[i]);
            enemies[i].transform.Translate(new Vector3(0, 1f));
        }

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        Debug.Log("玩家回合开始！");
        // 玩家操作完成后调用 EndPlayerTurn()
    }

    public void EndPlayerTurn()
    {
        Debug.Log("玩家回合结束！");
        state = BattleState.EnemyTurn;
        currentEnemyIndex = 0; // 从第一个敌人开始
        EnemyTurn();
    }

    void EnemyTurn()
    {
        if (currentEnemyIndex < currentEnemies.Count)
        {
            EnemyData enemy = currentEnemies[currentEnemyIndex];
            if (enemy.hp > 0 && enemy.skills.Length > 0)
            {
                int index = enemySkillIndex[enemy];
                SkillData skill = enemy.skills[index];

                int damage = Mathf.Max((int)(enemy.attack * skill.damageMultiplier) - playerData.data.Defense, 1);

                Debug.Log($"{enemy.name} 使用技能 {skill.name}，造成 {damage} 点伤害！玩家剩余 HP: {playerData.data.HP}");

                enemySkillIndex[enemy] = (index + 1) % enemy.skills.Length;

                if (playerData.data.HP <= 0)
                {
                    Debug.Log("玩家被击败！游戏结束！");
                    state = BattleState.BattleOver;
                    return;
                }
            }

            // 下一个敌人
            currentEnemyIndex++;
            Invoke("EnemyTurn", 1f); // 延迟调用，模拟逐个行动
        }
        else
        {
            EndEnemyTurn();
        }
    }

    void EndEnemyTurn()
    {
        Debug.Log("所有敌人行动完毕，切换到玩家回合！");
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }
}
