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

    bool isWet = false;
    float wetTimer = 0f;
    float wetTimerMax = 5f;
    float wetDamageMultiplier = 2f; //ToDo: Make this global?

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

    public void TakeDamage(float amount, AttackType type)
    {
        if (isWet && type == AttackType.Axe)
            amount *= wetDamageMultiplier;

        health -= amount;

        if (type == AttackType.Rain)
            isWet = true;

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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Chaac.Instance.TakeDamage(damage);
        }
    }

    void Update()
    {
        //ToDo: Make a state machine?
        //ToDo: Only move if playing!
        transform.position = Vector3.MoveTowards(transform.position, Chaac.Instance.GetPosition(), speed);

        if (isWet)
        {
            wetTimer += Time.deltaTime;
            if (wetTimer >= wetTimerMax)
            {
                wetTimer = 0f;
                isWet = false;
            }
        }
    }
}
