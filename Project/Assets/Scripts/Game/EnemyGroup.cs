using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "RPG/Enemy Group", order = 2)]
public class EnemyGroupSO : ScriptableObject
{
    public string groupName;
    public int groupID;

    public List<EnemyData> enemies = new List<EnemyData>();
}
