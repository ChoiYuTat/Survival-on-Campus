using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    PlayerTurn,
    PlayerAction,
    SelectTarget,
    EnemyTurn,
    Victory,
    Defeat,
    CheckWinLose,
    BattleOver
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public GameObject battleScene;
    public GameObject enemyPrefab;
    public GameObject enemyTargetManager;
    public GameObject targetPanel;
    public GameObject battleButton;
    public GameObject player;
    public GameObject playerPosition;
    public GameObject[] enemyPosition;


    public Transform content;
    public LoadPlayerData playerData;
    public Canvas battleCanvas, MenuCanvas;

    public Slider energySlider;
    public Text energyText, playerHP;

    private List<EnemyData> currentEnemies = new List<EnemyData>();
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private Dictionary<EnemyData, int> enemySkillIndex = new Dictionary<EnemyData, int>();
    public BattleState state;


    private int currentEnemyIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        energyText.text = energySlider.value.ToString();
    }

    public void StartBattle(List<EnemyData> enemies)
    {
        battleScene.SetActive(true);
        currentEnemies = enemies;
        state = BattleState.Start;

        foreach (var enemy in currentEnemies)
        {
            enemySkillIndex[enemy] = 0;
        }

        BeginBattle();
    }

    void BeginBattle()
    {
        player.transform.position = playerPosition.transform.position;
        player.GetComponent<luna>().enabled = false;
        player.GetComponent<OpenDoor>().enabled = false;
        battleCanvas.enabled = true;
        MenuCanvas.enabled = false;

        for (int i = 0; i < currentEnemies.Count; i++) 
        {
            Debug.Log(currentEnemies[i]);
            GameObject gameObject = Instantiate(enemyPrefab, enemyPosition[i].transform.position,
                enemyPosition[i].transform.rotation, enemyPosition[i].transform);
            enemies.Add(gameObject);
            enemies[i].GetComponent<Enemy>().SetEnemyData(currentEnemies[i], i);
            enemies[i].transform.Translate(new Vector3(0, 1f));
        }

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        battleButton.SetActive(true);
    }

    public void PlayerAction(int actionType)
    {
        state = BattleState.SelectTarget;
        for (int i = 0; i < targets.Count; i++)
        {
            Destroy(targets[i]);
        }
        targets.Clear();

        if (actionType == 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                targets.Add(Instantiate(enemyTargetManager, content));
                targets[i].GetComponent<EnemyTargetManager>().SetTarget(enemies[i], i);
            }
        }
        else if (actionType == 1) 
        {
            UseSkill();

            state = BattleState.CheckWinLose;
            CheckBattleEnd();

            if (state != BattleState.BattleOver)
            {
                state = BattleState.EnemyTurn;
                EnemyTurn();
            }
        }
    }

    public void UseSkill() 
    {
        int MaxEnegryUse = 3;
        if (energySlider.value >= 3)
        {
            state = BattleState.PlayerAction;
            battleButton.SetActive(false);
            foreach (var enemy in enemies)
            {
                int damage = (int)Mathf.Max((playerData.data.Attack * playerData.data.Skills[0].damageMultiplier * MaxEnegryUse)
                    - enemy.GetComponent<Enemy>().GetEnemyData().defense, 1);
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }

            energySlider.value -= MaxEnegryUse;
            energyText.text = energySlider.value.ToString();
        }
        else if (energySlider.value >= 1)
        {
            state = BattleState.PlayerAction;
            battleButton.SetActive(false);
            foreach (var enemy in enemies)
            {
                int damage = (int)Mathf.Max((playerData.data.Attack * playerData.data.Skills[0].damageMultiplier * energySlider.value)
                    - enemy.GetComponent<Enemy>().GetEnemyData().defense, 1);
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }

            energySlider.value = 0;
            energyText.text = energySlider.value.ToString();
        }
    }

    public void OnTargetSelected(int target)
    {
        state = BattleState.PlayerAction;
        targetPanel.SetActive(false);
        battleButton.SetActive(false);
        ExecutePlayerAttack(enemies[target].GetComponent<Enemy>());
    }

    void ExecutePlayerAttack(Enemy target)
    {
        Debug.Log("玩家攻击 " + target.GetEnemyData().name);
        int damage = Mathf.Max(playerData.data.Attack - target.GetEnemyData().defense, 1);
        target.TakeDamage(damage);
        energySlider.value += 1;
        energyText.text = energySlider.value.ToString();

        state = BattleState.CheckWinLose;
        CheckBattleEnd();

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            EnemyTurn();
        }
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
                playerData.data.HP -= damage;
                playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();

                Debug.Log($"{enemy.name} 使用技能 {skill.name}，造成 {damage} 点伤害！玩家剩余 HP: {playerData.data.HP}");
                CheckBattleEnd();
                enemySkillIndex[enemy] = (index + 1) % enemy.skills.Length;
            }

            currentEnemyIndex++;
            Invoke("EnemyTurn", 1f); 
        }
        else
        {
            currentEnemyIndex = 0;
            EndEnemyTurn();
        }
    }

    void EndEnemyTurn()
    {
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void CheckBattleEnd()
    {
        bool allEnemiesDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>().IsAlive()) 
            {
                allEnemiesDead = false; 
            }
        }

        if (allEnemiesDead)
        {
            state = BattleState.Victory;
            Debug.Log("战斗胜利！");
            state = BattleState.BattleOver;
        }
        else if (playerData.data.HP <= 0)
        {
            state = BattleState.Defeat;
            Debug.Log("玩家被击败！");
            state = BattleState.BattleOver;
        }
    }
}
