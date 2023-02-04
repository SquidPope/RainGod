using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Base script for an enemy
    protected float speed = 50f;
    protected float damage = 5f;
    protected float health = 10;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    protected void Die()
    {
        //return to object pool, let enemyManager know
        Debug.Log("ACK");
    }
}
