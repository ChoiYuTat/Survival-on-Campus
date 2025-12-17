using System.Collections;
using UnityEngine;

public class EnemyActionController : MonoBehaviour
{
    public EnemyData enemyData;                  // 当前敌人数据
    public EnemyActionConfig[] actionConfigs;    // 所有技能演出配置
    //public Animator animator;

    private Transform attackTarget;

    public void ExecuteSkill(Transform target, int skillIndex)
    {
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
        // 移动到安全距离
        yield return StartCoroutine(MoveTo(attackTarget.position - (attackTarget.forward * config.approachDistance)));

        // 攻击前变色
        if (config.changeColorBeforeAttack)
            yield return new WaitForSeconds(0.5f);

        // 判断是否跳跃攻击
        if (config.useJumpAttack)
        {
            yield return StartCoroutine(JumpAttack(skill));
        }
        else
        {
            for (int i = 0; i < config.attackCount; i++)
            {
                yield return StartCoroutine(Attack(skill));
            }
        }
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 3f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Attack(SkillData skill)
    {
        //animator.SetTrigger("Attack");
        yield return new WaitForSeconds(skill.duration);

        // 伤害计算：敌人攻击力 * 技能伤害倍率
        int damage = Mathf.RoundToInt(enemyData.attack * skill.damageMultiplier);
        Debug.Log(enemyData.name + " 使用技能 " + skill.name + " 对玩家造成伤害：" + damage);
    }

    private IEnumerator JumpAttack(SkillData skill)
    {
        //animator.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(skill.duration);

        int damage = Mathf.RoundToInt(enemyData.attack * skill.damageMultiplier);
        Debug.Log(enemyData.name + " 使用跳跃技能 " + skill.name + " 对玩家造成伤害：" + damage);
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
