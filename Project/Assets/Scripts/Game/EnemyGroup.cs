using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "RPG/Enemy Group", order = 2)]
public class EnemyGroupSO : ScriptableObject
{
    [Header("队伍名称")]
    public string groupName;
    public int groupID;

    [Header("敌人组合 (最多3个)")]
    public EnemyData[] enemies = new EnemyData[3];
}
