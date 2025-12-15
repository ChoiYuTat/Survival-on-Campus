using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private TMP_Text damageText;

    private int number;
    private Color originalColor;
    public EnemyActionConfig[] actionConfigs;    
                                                 //public Animator animator;

    //public Animator animator;
    public Renderer enemyRenderer;
    public Collider attackCollider;              
    public Collider jumpAttackCollider;          

    private QTEManager qteManager;

    public void SetEnemyData(EnemyData data,int number,Transform position, QTEManager manager)
    {
        enemyData = data;
        enemyData.instanceID = GetInstanceID() + number;
        qteManager = manager;
        enemyData.name += " #" + number;
        this.number = number;
        originalColor = enemyRenderer.material.color;
        Debug.Log("Enemy " + enemyData.name + " initialized with HP: " + enemyData.hp);
    }

    public void TakeDamage(GameObject effect, int damage) 
    {
        GameObject eff = Instantiate(effect, transform.position, transform.rotation);
        eff.transform.localScale *= 3f;
        eff.transform.LookAt(Camera.main.transform);
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
    }

    private void ResetDamageText()
    {
        damageText.text = "";
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
                yield return StartCoroutine(ChangeColor(originalColor, Color.yellow, 0.3f));
                yield return StartCoroutine(ResetColor(Color.yellow, 0.3f));
            }
            yield return StartCoroutine(JumpAttack(skill));
        }
        else if (config.attackCount >= 2)
        {
            if (config.changeColorBeforeAttack) 
            {
                yield return StartCoroutine(ChangeColor(originalColor, Color.red, 0.3f));
                yield return StartCoroutine(ResetColor(Color.red, 0.3f));
            }

            yield return StartCoroutine(KeepAttack(skill));
        }
        else 
        {
            if (config.changeColorBeforeAttack) 
            {
                yield return StartCoroutine(ChangeColor(originalColor, Color.red, 0.3f));
                yield return StartCoroutine(ResetColor(Color.red, 0.3f));
            }

            yield return StartCoroutine(Attack(skill));
        }

        yield return 1f;
        BattleManager.Instance.EnemyActionComplete();
    }
    private IEnumerator Attack(SkillData skill)
    {
        //animator.SetTrigger("Attack");
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

    private IEnumerator ResetColor(Color fromColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            enemyRenderer.material.color = Color.Lerp(fromColor, originalColor, t);
            yield return null;
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
