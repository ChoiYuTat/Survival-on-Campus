using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    private int number;
    public void SetEnemyData(EnemyData data,int number)
    {
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
}
