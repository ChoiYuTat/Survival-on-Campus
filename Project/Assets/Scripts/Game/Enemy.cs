using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private Text damageText;
    [SerializeField]
    private GameObject[] enemyModelPrefab;

    private GameObject model;
    private Animator animator;

    private int number;
    public EnemyActionConfig[] actionConfigs;    

    public Renderer enemyRenderer;        

    private QTEManager qteManager;

    private BattleManager battleManager;

    public void SetEnemyData(EnemyData data,int number,Transform position, QTEManager manager, BattleManager ins)
    {
        battleManager = ins;
        enemyData = data;
        enemyData.instanceID = GetInstanceID() + number;
        qteManager = manager;
        enemyData.name += " #" + number;
        this.number = number;
        model = Instantiate(enemyModelPrefab[enemyData.id - 1], position.position, position.rotation, transform);
        model.transform.rotation = Quaternion.Euler(0, 180, 0);
        model.transform.localScale = new Vector3(1f, 1.5f, 1f);
        model.transform.position += new Vector3(0, -0.5f, 0);
        animator = model.GetComponent<Animator>();

        Debug.Log("Enemy " + enemyData.name + " initialized with HP: " + enemyData.hp);
    }

    public void TakeDamage(GameObject effect, int damage) 
    {
        GameObject eff = Instantiate(effect, transform.position, transform.rotation);
        eff.transform.localScale *= 3f;
        eff.transform.LookAt(Camera.main.transform);
        Destroy(eff, 3f);
        animator.SetTrigger("DamageWeak");
        enemyData.hp -= damage;
        damageText.text += damage.ToString();
        Invoke("ResetDamageText", 1f);
        Debug.Log("Enemy " + enemyData.name + " took damage. Remaining HP: " + enemyData.hp);
    }

    public void HeavyDamageEffect(GameObject effect) 
    {
        damageText.text += "<color=yellow>CRITICAL!</color> \n";
        GameObject crEff = Instantiate(effect, transform.position, transform.rotation);
        crEff.transform.localScale *= 4f;
        crEff.transform.LookAt(Camera.main.transform);
        Destroy(crEff, 3f);
    }

    private void ResetDamageText()
    {
        damageText.text = "";
    }

    public void DeadAnimation() 
    {
        animator.SetBool("isDead", true);
    }

    public bool IsAlive() => enemyData.hp > 0;

    public EnemyData GetEnemyData() => enemyData;

    public int GetNumber() => number;

    public void ExecuteSkill(Transform target, int skillIndex)
    {

        if (skillIndex < 0 || skillIndex >= enemyData.skills.Length)
        {
            Debug.LogError("Skill Out bound");
            return;
        }
        
        SkillData skill = enemyData.skills[skillIndex];
        EnemyActionConfig config = FindConfigBySkillName(skill.name);

        
        if (config == null)
        {
            Debug.LogError("Can't not find" + skill.name);
            return;
        }

        StartCoroutine(ActionRoutine(skill, config));
    }

    private IEnumerator ActionRoutine(SkillData skill, EnemyActionConfig config)
    {
        if (config.useJumpAttack)
        {
            if (config.changeColorBeforeAttack) 
            {

            }
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(JumpAttack(skill));
        }
        else if (config.attackCount >= 2)
        {
            if (config.changeColorBeforeAttack) 
            {

            }
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(KeepAttack(skill));
        }
        else 
        {
            if (config.changeColorBeforeAttack) 
            {

            }
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Attack(skill));
        }

        yield return 1f;
        battleManager.EnemyActionComplete();
    }
    private IEnumerator Attack(SkillData skill)
    {
        //animator.SetTrigger("Attack");
        bool qteFinished = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true;  };

        // 找到对应的QTE事件并绑定回调
        var qte = qteManager.qteEvents.Find(e => e.eventName == "EnemyAttack");
        if (qte != null)
        {
            qte.onSuccess.AddListener(successAction);
            qte.onFailure.AddListener(failureAction);
        }

        qteManager.TriggerQTE("EnemyAttack");

        // 等待QTE完成
        yield return new WaitUntil(() => qteFinished);
        animator.SetTrigger("Attack");

        // 移除回调，避免重复绑定
        if (qte != null)
        {
            qte.onSuccess.RemoveListener(successAction);
            qte.onFailure.RemoveListener(failureAction);
        }
    }

    private IEnumerator JumpAttack(SkillData skill)
    {
        //animator.SetTrigger("JumpAttack");
        bool qteFinished = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true;};

        // 找到对应的QTE事件并绑定回调
        var qte = qteManager.qteEvents.Find(e => e.eventName == "EnemyJumpAttack");
        if (qte != null)
        {
            qte.onSuccess.AddListener(successAction);
            qte.onFailure.AddListener(failureAction);
        }
        qteManager.TriggerQTE("EnemyJumpAttack");
        // 等待QTE完成
        yield return new WaitUntil(() => qteFinished);
        animator.SetTrigger("Attack");

        // 移除回调，避免重复绑定
        if (qte != null)
        {
            qte.onSuccess.RemoveListener(successAction);
            qte.onFailure.RemoveListener(failureAction);
        }
    }

    private IEnumerator KeepAttack(SkillData skill)
    {
        //animator.SetTrigger("JumpAttack");
        bool qteFinished = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true; };

        // 找到对应的QTE事件并绑定回调
        var qte = qteManager.qteEvents.Find(e => e.eventName == "EnemyKeepAttack");
        if (qte != null)
        {
            qte.onSuccess.AddListener(successAction);
            qte.onFailure.AddListener(failureAction);
        }
        qteManager.TriggerQTE("EnemyKeepAttack");
        // 等待QTE完成
        yield return new WaitUntil(() => qteFinished);
        animator.SetTrigger("Attack");

        // 移除回调，避免重复绑定
        if (qte != null)
        {
            qte.onSuccess.RemoveListener(successAction);
            qte.onFailure.RemoveListener(failureAction);
        }
    }

    private EnemyActionConfig FindConfigBySkillName(string skillName)
    {
        foreach (var config in actionConfigs)
        {
            if (config.skillName == skillName)
                return config;
        }
        return null;
    }

}
