using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class EnemyBase : MonoBehaviour
{
    // Base script for an enemy
    protected float speed = 0.01f; //ToDo: change based on enemy type
    protected float damage = 5f;
    protected float health = 10;

    SpriteRenderer spRenderer;
    Collider2D collide;
    new ParticleSystem particleSystem;

    public EnemyManager manager;

    bool isWet = false;
    float wetTimer = 0f;
    float wetTimerMax = 5f;
    float wetDamageMultiplier = 2f; //ToDo: Make this global?

    bool isMoving = false;

    bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            
            spRenderer.enabled = isActive;
            collide.enabled = isActive;
            isMoving = isActive;
            
            if (!isActive)
            {
                particleSystem.Stop(); //Make sure particles are removed
            }
        }
    }

    void Start()
    {
        spRenderer = gameObject.GetComponent<SpriteRenderer>();
        collide = gameObject.GetComponent<Collider2D>();
        particleSystem = gameObject.GetComponent<ParticleSystem>();

        IsActive = false;
        particleSystem.Stop();

        GameController.Instance.StateChange.AddListener(StateChange);
    }

    void StateChange(GameState state)
    {
        if (isActive)
            isMoving = state == GameState.Playing;
    }

    public void SetPosition(Vector3 pos) { transform.position = pos; }

    public void TakeDamage(float amount, AttackType type)
    {
        if (isWet && type == AttackType.Axe)
            amount *= wetDamageMultiplier;

        health -= amount;
        Debug.Log($"Took {amount} damage");

        if (type == AttackType.Rain) //ToDo: Slight blue tint or particle effect?
        {
            isWet = true;
            particleSystem.Play();
        }

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
        if (!isMoving || !isActive)
            return;

        if (isWet)
        {
            wetTimer += Time.deltaTime;
            if (wetTimer >= wetTimerMax)
            {
                wetTimer = 0f;
                isWet = false;
                particleSystem.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving || !isActive)
            return;

        transform.position = Vector3.MoveTowards(transform.position, Chaac.Instance.GetPosition(), speed);
    }
}
