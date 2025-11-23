using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/ActionConfig", fileName = "NewEnemyActionConfig")]
public class EnemyActionConfig : ScriptableObject
{
    [Header("技能信息")]
    public string skillName;                // 与 SkillData.name 对应

    [Header("普通攻击演出参数")]
    public int attackCount = 1;             // 普通攻击次数
    public float approachDistance = 2f;     // 安全距离
    public float attackDistance = 0.5f;     // 攻击距离
    public bool changeColorBeforeAttack = true; // 是否攻击前变色
    public bool returnToOrigin = true;      // 攻击后是否返回原位

    [Header("跳跃攻击演出参数")]
    public bool useJumpAttack = false;      // 是否启用跳跃攻击
    public float jumpHeight = 2f;           // 跳跃高度
    public float jumpDuration = 1f;         // 跳跃总时长
}
