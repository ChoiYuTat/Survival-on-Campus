using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    private int number;
    private int skillIndex;
    public EnemyActionConfig[] actionConfigs;    
                                                 //public Animator animator;

    //public Animator animator;
    public Renderer enemyRenderer;
    public Collider attackCollider;              
    public Collider jumpAttackCollider;          


    private Transform defaultPosition;
    private Transform attackTarget;

    private QTEManager qteManager;

    public void SetEnemyData(EnemyData data,int number,Transform position, QTEManager manager)
    {
        defaultPosition = position;
        enemyData = data;
        enemyData.instanceID = GetInstanceID() + number;
        qteManager = manager;
        enemyData.name += " #" + number;
        this.number = number;
        Debug.Log("Enemy " + enemyData.name + " initialized with HP: " + enemyData.hp);
    }

    public void TakeDamage(int damage) 
    {
        enemyData.hp -= damage;
        Debug.Log("Enemy " + enemyData.name + " took damage. Remaining HP: " + enemyData.hp);
    }

    public bool IsAlive() => enemyData.hp > 0;

    public EnemyData GetEnemyData() => enemyData;

    public int GetNumber() => number;

    private void Start()
    {
        if (attackCollider != null) attackCollider.enabled = false;
        if (jumpAttackCollider != null) jumpAttackCollider.enabled = false;

        if (enemyRenderer != null)
            enemyRenderer.material = new Material(enemyRenderer.material); // ���⹲������һ���ɫ
    }
    public void ExecuteSkill(Transform target, int skillIndex)
    {
        this.skillIndex = skillIndex;
        attackTarget = target;

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
                yield return StartCoroutine(ChangeColor(Color.white, Color.yellow, 0.5f));
            yield return StartCoroutine(JumpAttack(skill));
        }
        else if (config.attackCount >= 2)
        {
            if (config.changeColorBeforeAttack)
                yield return StartCoroutine(ChangeColor(Color.white, Color.red, 0.5f));
            yield return StartCoroutine(KeepAttack(skill));
        }
        else 
        {
            if (config.changeColorBeforeAttack)
                yield return StartCoroutine(ChangeColor(Color.white, Color.red, 0.5f));
            yield return StartCoroutine(Attack(skill));
        }

            BattleManager.Instance.EnemyActionComplete();
    }
    private IEnumerator Attack(SkillData skill)
    {
        //animator.SetTrigger("Attack");
        ChangeColorImmediate(Color.white);
        bool qteFinished = false;
        bool qteSuccess = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; qteSuccess = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true; qteSuccess = false; };

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
        bool qteSuccess = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; qteSuccess = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true; qteSuccess = false; };

        // 找到对应的QTE事件并绑定回调
        var qte = qteManager.qteEvents.Find(e => e.eventName == "EnemyJumpAttack");
        if (qte != null)
        {
            qte.onSuccess.AddListener(successAction);
            qte.onFailure.AddListener(failureAction);
        }
        ChangeColorImmediate(Color.white);
        qteManager.TriggerQTE("EnemyJumpAttack");
        // 等待QTE完成
        yield return new WaitUntil(() => qteFinished);

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
        bool qteSuccess = false;

        // 注册临时回调
        UnityEngine.Events.UnityAction successAction = () => { qteFinished = true; qteSuccess = true; };
        UnityEngine.Events.UnityAction failureAction = () => { qteFinished = true; qteSuccess = false; };

        // 找到对应的QTE事件并绑定回调
        var qte = qteManager.qteEvents.Find(e => e.eventName == "EnemyKeepAttack");
        if (qte != null)
        {
            qte.onSuccess.AddListener(successAction);
            qte.onFailure.AddListener(failureAction);
        }
        ChangeColorImmediate(Color.white);
        qteManager.TriggerQTE("EnemyKeepAttack");
        // 等待QTE完成
        yield return new WaitUntil(() => qteFinished);

        // 移除回调，避免重复绑定
        if (qte != null)
        {
            qte.onSuccess.RemoveListener(successAction);
            qte.onFailure.RemoveListener(failureAction);
        }
    }

    private IEnumerator ChangeColor(Color fromColor, Color toColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            enemyRenderer.material.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }
    }

    private void ChangeColorImmediate(Color toColor) 
    {
        enemyRenderer.material.color = toColor;
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
