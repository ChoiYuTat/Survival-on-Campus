using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;

    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;
        Debug.Log("Enemy " + enemyData.name + " initialized with HP: " + enemyData.hp);
    }
}
