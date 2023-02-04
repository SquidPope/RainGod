using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour
{
    // Base script for an enemy
    protected float speed = 0.001f; //ToDo: change based on enemy type
    protected float damage = 5f;
    protected float health = 10;

    SpriteRenderer spRenderer;
    Collider2D collide;

    public EnemyManager manager;

    bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            
            spRenderer.enabled = isActive;
            collide.enabled = isActive;
        }
    }

    void Start()
    {
        spRenderer = gameObject.GetComponent<SpriteRenderer>();
        collide = gameObject.GetComponent<Collider2D>();

        IsActive = false;
    }

    public void SetPosition(Vector3 pos) { transform.position = pos; }

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
        IsActive = false;
        manager.EnemyDied();
    }

    void Update()
    {
        //ToDo: Make a state machine?
        transform.position = Vector3.MoveTowards(transform.position, Chaahk.Instance.GetPosition(), speed);
    }
}
