using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using DG.Tweening;

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
    public GameObject LevelUPTXT;
    public GameObject[] enemyPosition;
    public GameObject battleCameraPosition;

    [SerializeField]
    private GameObject hitEffectPrefab, criticalEffectPrefab;

    [SerializeField]
    private StressReceiver cameraReceiver;


    public MenuManager menuManager;
    public QTEManager QTEmanager;

    public Transform targetContent;
    public LoadPlayerData playerData;
    public Canvas battleCanvas, MenuCanvas, itemCanvas, resultCanvas, gameOverCanvas;

    public Slider energySlider;
    public Text energyText, playerHP, earnedEXP_txt;
    public AudioClip enemyHit, playerHit, enemyDead, playerDead, criticalSound, dodge, heal;
    public Image hitImage, healImage;
    public Button fight, skill, closeResult;

    private List<EnemyData> currentEnemies = new List<EnemyData>();
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> targets = new List<GameObject>();
    private Dictionary<EnemyData, int> enemySkillIndex = new Dictionary<EnemyData, int>();
    private int earnedExp = 0;
    private int energyUseIndex;
    private Vector3 playerOriginalPosition;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();

        fight.onClick.AddListener(() => { 
            PlayerAction(0);
            targetPanel.SetActive(true);
        });
        skill.onClick.AddListener(() => PlayerAction(1));
        closeResult.onClick.AddListener(() => CloseResult());
    }

    public void Teleport() 
    {
        playerOriginalPosition = player.transform.position;
        player.GetComponent<PlayerControl>().enabled = false;
        player.GetComponent<OpenDoor>().enabled = false;
        player.GetComponent<AudioSource>().mute = true;
        playerSprite.SetActive(false);
    }

    public void StartBattle(List<EnemyData> enemies)
    {
        player.transform.position = playerPosition.transform.position;
        battleCameraPosition.SetActive(true);
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
        if (battleScene == null)
            return;

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
                targets.Add(Instantiate(enemyTargetManager, targetContent));
                targets[i].GetComponent<EnemyTargetManager>().SetTarget(enemies[i], i);
            }
        }
        else if (actionType == 1)
        {
            //UseSkill();
            battleButton.SetActive(false);
            QTEmanager.ShowQTETips();
            StartCoroutine(WaitAndTriggerSkillQTE(1f));
        }
    }

    public void SkipBattle() 
    {
        clearEnemy();
    }

    void clearEnemy() 
    {
        battleButton.SetActive(false);
        foreach (var enemy in enemies)
        {
            int damage = 114514;
            enemy.GetComponent<Enemy>().TakeDamage(hitEffectPrefab, damage);
        }
        EndTrun();
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
                audioSource.PlayOneShot(enemyHit);
                Invoke("PlayCriticalSound", 0.13f);
                enemy.GetComponent<Enemy>().HeavyDamageEffect(criticalEffectPrefab);
                cameraReceiver.InduceStress(0.10f);
            }
            else 
            {
                audioSource.PlayOneShot(enemyHit);
                cameraReceiver.InduceStress(0.04f);
            }

            enemy.GetComponent<Enemy>().TakeDamage(hitEffectPrefab, damage);
        }

        energyUseIndex = 0;

        EndTrun();
    }

    void PlayCriticalSound() 
    {
        audioSource.PlayOneShot(criticalSound, 0.2f);
    }

    public void EndTrun() 
    {
        Invoke("CheckEnemyDead", 0.4f);

        state = BattleState.CheckWinLose;
        Invoke("CheckBattleEnd", 1.5f);

        if (state != BattleState.BattleOver)
        {
            state = BattleState.EnemyTurn;
            Invoke("EnemyTurn", 1.5f);
        }
    }

    public void UseItem() 
    {
        StartCoroutine(HealAnimation());
        audioSource.PlayOneShot(heal);
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
        itemCanvas.enabled = false;
        state = BattleState.EnemyTurn;
        Invoke("EnemyTurn", 1.5f);
    }

    public void OnTargetSelected(int target)
    {
        QTEmanager.ShowQTETips();
        targetPanel.SetActive(false);
        battleButton.SetActive(false);
        StartCoroutine(WaitAndTriggerFightQTE(1f));
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

    public void PlayerDodge() 
    {
        audioSource.PlayOneShot(dodge);
    }

    void ExecutePlayerAttack(Enemy target, float n, bool qteSuccess)
    {
        Debug.Log("��ҹ��� " + target.GetEnemyData().name);
        int damage = (int)Mathf.Max((playerData.data.Attack * n) - target.GetEnemyData().defense, 1);
        if (qteSuccess)
        {
            audioSource.PlayOneShot(enemyHit);
            Invoke("PlayCriticalSound", 0.13f);
            cameraReceiver.InduceStress(0.10f);
            target.HeavyDamageEffect(criticalEffectPrefab);
        }
        else
        {
            audioSource.PlayOneShot(enemyHit);
            cameraReceiver.InduceStress(0.04f);
        }
        target.TakeDamage(hitEffectPrefab, damage);

        energySlider.value += 1;
        energyText.text = energySlider.value.ToString();

        EndTrun();
    }

    void CheckEnemyDead() 
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i].GetComponent<Enemy>().IsAlive())
            {
                audioSource.PlayOneShot(enemyDead, 1.2f);
                earnedExp += enemies[i].GetComponent<Enemy>().GetEnemyData().exp;
                enemies[i].GetComponent<Enemy>().DeadAnimation();
                Destroy(enemies[i], 3f);
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
                QTEmanager.ShowAttentionTips();
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
        }
        else
        {
            currentEnemyIndex = 0;
            Invoke("EndEnemyTurn", 0.5f);
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
        StartCoroutine(HitAnimation());
        audioSource.PlayOneShot(playerHit);
        cameraReceiver.InduceStress(0.2f);
        int damage = (int)(currentEnemies[currentEnemyIndex].attack * currentEnemies[currentEnemyIndex].skills[skillIndex].damageMultiplier
                    - player.gameObject.GetComponent<LoadPlayerData>().data.Defense);
        playerData.data.HP -= damage;
        CheckBattleEnd();
        playerHP.text = playerData.data.HP.ToString() + "/" + playerData.data.MaxHP.ToString();
    }

    IEnumerator HitAnimation() 
    {
        hitImage.transform.gameObject.SetActive(true);
        hitImage.DOFade(0.2f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        hitImage.DOFade(0, 0.25f);
        yield return new WaitForSeconds(0.25f);
        hitImage.transform.gameObject.SetActive(false);
    }

    IEnumerator HealAnimation()
    {
        healImage.transform.gameObject.SetActive(true);
        healImage.DOFade(0.2f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        healImage.DOFade(0, 0.25f);
        yield return new WaitForSeconds(0.25f);
        healImage.transform.gameObject.SetActive(false);
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
            audioSource.PlayOneShot(playerDead);
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
            LevelUPTXT.SetActive(true);
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
        player.GetComponent<AudioSource>().mute = false;
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
        LevelUPTXT.SetActive(false);
        battleScene.SetActive(false);
    }

    void GameOver() 
    {
        gameOverCanvas.enabled = true;
    }
}
