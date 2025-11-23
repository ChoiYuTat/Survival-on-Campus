using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;

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
    public GameObject LevelUP;
    public GameObject[] enemyPosition;


    public MenuManager menuManager;
    public QTEManager QTEmanager;

    public Transform content;
    public LoadPlayerData playerData;
    public Canvas battleCanvas, MenuCanvas, itemCanvas, resultCanvas, gameOverCanvas;

    public Slider energySlider;
    public Text energyText, playerHP, earnedEXP_txt;

    private List<EnemyData> currentEnemies = new List<EnemyData>();
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private Dictionary<EnemyData, int> enemySkillIndex = new Dictionary<EnemyData, int>();
    private int earnedExp = 0;
    private Vector3 playerOriginalPosition;

    public BattleState state;


    private int currentEnemyIndex = 0;
    private int targetIndex = 0;

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
        playerOriginalPosition = player.transform.position;
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
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
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
        if (state == BattleState.PlayerTurn)
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
            //UseSkill();
            battleButton.SetActive(false);
            StartCoroutine(WaitAndTriggerSkillQTE(0.5f));
        }
    }

    void UseSkill(float Multiplier) 
    {
        int MaxEnegryUse = 3;
        if (energySlider.value >= 3)
        {
            state = BattleState.PlayerAction;
            battleButton.SetActive(false);
            foreach (var enemy in enemies)
            {
                int damage = (int)Mathf.Max((playerData.data.Attack * playerData.data.Skills[0].damageMultiplier 
                    * MaxEnegryUse * Multiplier)
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
                int damage = (int)Mathf.Max((playerData.data.Attack * playerData.data.Skills[0].damageMultiplier 
                    * energySlider.value * Multiplier)
                    - enemy.GetComponent<Enemy>().GetEnemyData().defense, 1);
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
            energySlider.value = 0;
            energyText.text = energySlider.value.ToString();
        }

        CheckEnemyDead();

        state = BattleState.CheckWinLose;
        CheckBattleEnd();

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            EnemyTurn();
        }
    }

    public void UseItem() 
    {
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
        itemCanvas.enabled = false;
        state = BattleState.EnemyTurn;
        EnemyTurn();
    }

    public void OnTargetSelected(int target)
    {

        targetPanel.SetActive(false);
        battleButton.SetActive(false);
        StartCoroutine(WaitAndTriggerFightQTE(0.5f));
        targetIndex = target;
    }

    IEnumerator WaitAndTriggerFightQTE(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        state = BattleState.PlayerAction;
        QTEmanager.TriggerQTE("Fight");
    }

    IEnumerator WaitAndTriggerSkillQTE(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        state = BattleState.PlayerAction;
        QTEmanager.TriggerQTE("Skill");
    }

    public void FightQTEBonues(float Multiplier) 
    {
        ExecutePlayerAttack(enemies[targetIndex].GetComponent<Enemy>(), Multiplier);
    }

    public void SkillQTEBonues(float Multiplier)
    {
        UseSkill(Multiplier);
    }

    void ExecutePlayerAttack(Enemy target, float n)
    {
        Debug.Log("玩家攻击 " + target.GetEnemyData().name);
        int damage = (int)Mathf.Max((playerData.data.Attack * n) - target.GetEnemyData().defense, 1);
        target.TakeDamage(damage);
        energySlider.value += 1;
        energyText.text = energySlider.value.ToString();

        CheckEnemyDead();
        state = BattleState.CheckWinLose;
        CheckBattleEnd();

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            EnemyTurn();
        }
    }

    void CheckEnemyDead() 
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i].GetComponent<Enemy>().IsAlive())
            {
                Debug.Log(enemies[i].GetComponent<Enemy>().GetEnemyData().name + " 被击败！");
                earnedExp += enemies[i].GetComponent<Enemy>().GetEnemyData().exp;
                Destroy(enemies[i]);
                enemies.RemoveAt(i);
                currentEnemies.RemoveAt(i);
            }
        }
    }

    void EnemyTurn()
    {
        if ((currentEnemyIndex < currentEnemies.Count) && (state == BattleState.EnemyTurn))
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
            EndBattle();
        }
        else if (playerData.data.HP <= 0)
        {
            state = BattleState.Defeat;
            Debug.Log("玩家被击败！");
            state = BattleState.BattleOver;
            GameOver();
        }
    }

    public void EndBattle()
    {
        resultCanvas.enabled = true;
        earnedEXP_txt.text = "+" + earnedExp.ToString();
        Debug.Log("获得经验值: " + earnedExp);
        if (earnedExp >= playerData.data.RequiredExp)
        {
            LevelUP.SetActive(true);
        }
        playerData.data.AddExperience(earnedExp);
        earnedExp = 0;
    }
    public void CloseResult()
    {
        resultCanvas.enabled = false;
        player.transform.position = playerOriginalPosition;
        player.GetComponent<luna>().enabled = true;
        player.GetComponent<OpenDoor>().enabled = true;
        battleCanvas.enabled = false;
        MenuCanvas.enabled = true;
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        currentEnemies.Clear();
        enemySkillIndex.Clear();
        menuManager.ResetEnemy();
        LevelUP.SetActive(false);
        battleScene.SetActive(false);
    }

    void GameOver() 
    {
        gameOverCanvas.enabled = true;
    }
}
