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
    public GameObject playerSprite;
    public GameObject playerPosition;
    public GameObject LevelUP;
    public GameObject[] enemyPosition;

    [SerializeField]
    private GameObject hitEffectPrefab, criticalEffectPrefab;

    [SerializeField]
    private StressReceiver cameraReceiver;


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
    private int energyUseIndex;
    private Vector3 playerOriginalPosition;

    public BattleState state;


    private int currentEnemyIndex = 0;
    private int targetIndex = 0;
    private int skillIndex = 0;

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
        energySlider.value = 2;
        energyText.text = energySlider.value.ToString();
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
        player.GetComponent<PlayerControl>().enabled = false;
        player.GetComponent<OpenDoor>().enabled = false;
        playerSprite.SetActive(false);
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
        battleCanvas.enabled = true;
        MenuCanvas.enabled = false;

        for (int i = 0; i < currentEnemies.Count; i++) 
        {
            Debug.Log(currentEnemies[i]);
            GameObject gameObject = Instantiate(enemyPrefab, enemyPosition[i].transform.position,
                enemyPosition[i].transform.rotation, enemyPosition[i].transform);
            enemies.Add(gameObject);
            enemies[i].GetComponent<Enemy>().SetEnemyData(currentEnemies[i], i, enemyPosition[i].transform, QTEmanager);
            //enemies[i].transform.Translate(new Vector3(0, 1f));
        }

        state = BattleState.PlayerTurn;
        PlayerTurn();
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

    void UseSkill(float Multiplier, bool qteSuccess) 
    {
        state = BattleState.PlayerAction;
        battleButton.SetActive(false);
        foreach (var enemy in enemies)
        {
                int damage = (int)Mathf.Max((playerData.data.Attack * playerData.data.Skills[0].damageMultiplier 
                    * energyUseIndex * Multiplier)
                    - enemy.GetComponent<Enemy>().GetEnemyData().defense, 1);
            if (qteSuccess)
            {
                enemy.GetComponent<Enemy>().HeavyDamageEffect(criticalEffectPrefab);
                cameraReceiver.InduceStress(0.10f);
            }
            else 
            {
                cameraReceiver.InduceStress(0.04f);
            }

            enemy.GetComponent<Enemy>().TakeDamage(hitEffectPrefab, damage);
        }
        energyUseIndex = 0;

        Invoke("CheckEnemyDead", 0.5f);

        state = BattleState.CheckWinLose;
        Invoke("CheckBattleEnd", 1.3f);

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            Invoke("EnemyTurn", 1.5f);
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
        int MaxEnegryUse = 3;
        if (energySlider.value >= 3)
        {
            energyUseIndex = MaxEnegryUse;
            state = BattleState.PlayerAction;
            battleButton.SetActive(false);
            energySlider.value -= MaxEnegryUse;
            energyText.text = energySlider.value.ToString();
        }
        else if (energySlider.value >= 1)
        {
            energyUseIndex = (int)energySlider.value;
            state = BattleState.PlayerAction;
            battleButton.SetActive(false);
            energySlider.value = 0;
            energyText.text = energySlider.value.ToString();
        }
        yield return new WaitForSeconds(waitTime);
        state = BattleState.PlayerAction;
        QTEmanager.TriggerQTE("Skill");
    }

    public void FightQTEBonues(float Multiplier) 
    {
        bool success = false;
        if (Multiplier > 1.0f)
            success = true;
        ExecutePlayerAttack(enemies[targetIndex].GetComponent<Enemy>(), Multiplier, success);
    }

    public void SkillQTEBonues(float Multiplier)
    {
        bool success = false;
        if (Multiplier > 1.0f)
            success = true;
        UseSkill(Multiplier, success);
    }

    void ExecutePlayerAttack(Enemy target, float n, bool qteSuccess)
    {
        Debug.Log("��ҹ��� " + target.GetEnemyData().name);
        int damage = (int)Mathf.Max((playerData.data.Attack * n) - target.GetEnemyData().defense, 1);
        if (qteSuccess)
        {
            cameraReceiver.InduceStress(0.10f);
            target.HeavyDamageEffect(criticalEffectPrefab);
        }
        else
        {
            cameraReceiver.InduceStress(0.04f);
        }
        target.TakeDamage(hitEffectPrefab, damage);

        energySlider.value += 1;
        energyText.text = energySlider.value.ToString();

        Invoke("CheckEnemyDead", 1f);
        state = BattleState.CheckWinLose;
        Invoke("CheckBattleEnd", 1f);

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            Invoke("EnemyTurn", 1f);
        }
    }

    void CheckEnemyDead() 
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i].GetComponent<Enemy>().IsAlive())
            {
                Debug.Log(enemies[i].GetComponent<Enemy>().GetEnemyData().name + " �����ܣ�");
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
                skillIndex = enemySkillIndex[enemy];
                Debug.Log(skillIndex);
                SkillData skill = enemy.skills[skillIndex];

                enemies[currentEnemyIndex].GetComponent<Enemy>().ExecuteSkill(player.transform, skillIndex);
                enemySkillIndex[enemy] = (skillIndex + 1) % enemy.skills.Length;
            }
        }


    }

    public void EnemyActionComplete() 
    {
        currentEnemyIndex++;
        if ((currentEnemyIndex < currentEnemies.Count)) 
        {
            Invoke("EnemyTurn", 0.5f);
            //EnemyTurn();
        }
        else
        {
            currentEnemyIndex = 0;
            Invoke("EndEnemyTurn", 0.5f);
            //EndEnemyTurn();
        }
    }

    void EndEnemyTurn()
    {
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        if (state == BattleState.PlayerTurn)
        {
            battleButton.SetActive(true);
        }
    }

    public void PlayerTakeDamage()
    {
        cameraReceiver.InduceStress(0.2f);
        int damage = (int)(currentEnemies[currentEnemyIndex].attack * currentEnemies[currentEnemyIndex].skills[skillIndex].damageMultiplier
                    - player.gameObject.GetComponent<LoadPlayerData>().data.Defense);
        playerData.data.HP -= damage;
        CheckBattleEnd();
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
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
            state = BattleState.BattleOver;
            EndBattle();
        }
        else if (playerData.data.HP <= 0)
        {
            state = BattleState.Defeat;
            state = BattleState.BattleOver;
            GameOver();
        }
    }

    public void EndBattle()
    {
        resultCanvas.enabled = true;
        earnedEXP_txt.text = "+" + earnedExp.ToString();
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
        playerSprite.SetActive(true);
        player.GetComponent<PlayerControl>().enabled = true;
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
