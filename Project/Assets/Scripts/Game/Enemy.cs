using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    private int number;
    private int skillIndex;
    public EnemyActionConfig[] actionConfigs;    // 所有技能演出配置
                                                 //public Animator animator;

    [Header("组件引用")]
    //public Animator animator;
    public Renderer enemyRenderer;
    public Collider attackCollider;              // 普通攻击判定
    public Collider jumpAttackCollider;          // 跳跃攻击判定


    private Transform defaultPosition;
    private Transform attackTarget;

    public void SetEnemyData(EnemyData data,int number,Transform position)
    {
        defaultPosition = position;
        enemyData = data;
        enemyData.instanceID = GetInstanceID() + number;
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
            enemyRenderer.material = new Material(enemyRenderer.material); // 避免共享材质一起变色
    }

    /// <summary>
    /// 执行敌人技能
    /// </summary>
    public void ExecuteSkill(Transform target, int skillIndex)
    {
        this.skillIndex = skillIndex;
        attackTarget = target;

        if (skillIndex < 0 || skillIndex >= enemyData.skills.Length)
        {
            Debug.LogError("技能索引超出范围");
            return;
        }
        
        SkillData skill = enemyData.skills[skillIndex];
        EnemyActionConfig config = FindConfigBySkillName(skill.name);

        
        if (config == null)
        {
            Debug.LogError("未找到技能配置：" + skill.name);
            return;
        }

        StartCoroutine(ActionRoutine(skill, config));
    }

    private IEnumerator ActionRoutine(SkillData skill, EnemyActionConfig config)
    {
        // Step 1: 移动到安全距离
        yield return StartCoroutine(MoveTo(GetOffsetPosition(attackTarget, config.approachDistance)));

        // Step 2: 攻击前变色


        // Step 3: 判断攻击类型
        if (config.useJumpAttack)
        {
            if (config.changeColorBeforeAttack)
                yield return StartCoroutine(ChangeColor(Color.white, Color.yellow, 1.5f));
            yield return StartCoroutine(JumpAttack(skill, config));
        }
        else
        {
            for (int i = 0; i < config.attackCount; i++)
            {
                if (config.changeColorBeforeAttack)
                    yield return StartCoroutine(ChangeColor(Color.white, Color.red, 1.5f));

                yield return StartCoroutine(Attack(skill));
                yield return StartCoroutine(MoveTo(GetOffsetPosition(attackTarget, config.attackDistance)));
                yield return StartCoroutine(DoneAttack(skill));

                if (i < config.attackCount - 1)
                    yield return StartCoroutine(MoveTo(GetOffsetPosition(attackTarget, config.approachDistance)));
            }
        }

        // Step 4: 返回原位
        if (config.returnToOrigin)
            yield return StartCoroutine(MoveTo(defaultPosition.position));

        BattleManager.Instance.EnemyActionComplete();
        Debug.Log(enemyData.name + " 行动结束");
    }

    // 普通攻击
    private IEnumerator Attack(SkillData skill)
    {
        //animator.SetTrigger("Attack");
        ChangeColorImmediate(Color.white);
        EnableAttackCollider();
        yield return null;
    }

    private IEnumerator DoneAttack(SkillData skill)
    {
        DisableAttackCollider();
        yield return null;
    }

    // 跳跃攻击
    private IEnumerator JumpAttack(SkillData skill, EnemyActionConfig config)
    {
        //animator.SetTrigger("JumpAttack");
        ChangeColorImmediate(Color.white);
        Vector3 startPos = transform.position;
        Vector3 endPos = attackTarget.position + new Vector3(2f, 1f, 0); // 落在角色后方
        float elapsed = 0f;

        while (elapsed < config.jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / config.jumpDuration;

            float height = Mathf.Sin(t * Mathf.PI) * config.jumpHeight;
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

            if (t > 0.3f && t < 0.7f)
                EnableJumpAttackCollider();
            else
                DisableJumpAttackCollider();

            yield return null;
        }

        DisableJumpAttackCollider();
    }

    // 移动
    private IEnumerator MoveTo(Vector3 destination)
    {
        //animator.SetBool("IsMoving", true);

        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 7f * Time.deltaTime);
            yield return null;
        }

        //animator.SetBool("IsMoving", false);
    }

    // 颜色渐变
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


    // 碰撞判定
    private void OnTriggerEnter(Collider other)
    {
        if (attackCollider != null && attackCollider.enabled && other.gameObject.CompareTag("Player"))
        {
            if (attackTarget != null) 
            {
                Debug.Log("Enemy " + enemyData.name + " hits Player with skill " + enemyData.skills[skillIndex].name);
                BattleManager.Instance.PlayerTakeDamage((int)(enemyData.attack * enemyData.skills[skillIndex].damageMultiplier
                    - attackTarget.gameObject.GetComponent<LoadPlayerData>().data.Defense));
            }
        }

        if (jumpAttackCollider != null && jumpAttackCollider.enabled && other.gameObject.CompareTag("Player"))
        {
            if (attackTarget != null) 
            {
                BattleManager.Instance.PlayerTakeDamage((int)(enemyData.attack * enemyData.skills[skillIndex].damageMultiplier
                    - attackTarget.gameObject.GetComponent<LoadPlayerData>().data.Defense));
            }
        }
    }

    // 辅助方法
    private void EnableAttackCollider() { if (attackCollider != null) attackCollider.enabled = true; }
    private void DisableAttackCollider() { if (attackCollider != null) attackCollider.enabled = false; }
    private void EnableJumpAttackCollider() { if (jumpAttackCollider != null) jumpAttackCollider.enabled = true; }
    private void DisableJumpAttackCollider() { if (jumpAttackCollider != null) jumpAttackCollider.enabled = false; }

    private EnemyActionConfig FindConfigBySkillName(string skillName)
    {
        foreach (var config in actionConfigs)
        {
            if (config.skillName == skillName)
                return config;
        }
        return null;
    }

    private Vector3 GetOffsetPosition(Transform target, float distance)
    {
        // 只取水平面方向 (X,Z)，忽略 Y
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 dir = (targetPos - transform.position).normalized;

        return targetPos - dir * distance;
    }
}
