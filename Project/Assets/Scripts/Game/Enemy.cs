using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;

    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;
        Debug.Log("Enemy " + enemyData.name + " initialized with HP: " + enemyData.hp);
    }

    public void TakeDamage(int damage) 
    {
        enemyData.hp -= damage;
        Debug.Log("Enemy " + enemyData.name + " took damage. Remaining HP: " + enemyData.hp);
    }

    public bool IsAlive() => enemyData.hp > 0;
}
