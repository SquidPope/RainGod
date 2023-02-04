using UnityEngine;

public enum AttackType {Axe, Bees, Rain}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    // Script controling the effect/hitbox of a player's attack
    [SerializeField] Sprite axeSprite;
    [SerializeField] Sprite beesSprite; //Hive, bees themselves are a particle around this
    [SerializeField] Sprite rainSprite;

    float timer = 0f;
    bool isFired = false;
    SpriteRenderer spRenderer;
    Collider2D collide;
    
    float damage;
    float lifespan;
    float speed;

    Vector3 moveDir;

    AttackType type;
    public AttackType Type
    {
        get { return type; }
        set
        {
            type = value;
            switch(type)
            {
                case AttackType.Axe:
                damage = 4f;
                lifespan = 0.05f;
                speed = 30f;
                spRenderer.sprite = axeSprite;
                break;

                case AttackType.Bees:
                damage = 0.1f;
                lifespan = 4f;
                speed = 0f;
                spRenderer.sprite = beesSprite;
                break;

                case AttackType.Rain:
                damage = 0f;
                lifespan = 0.3f;
                speed = 20f;
                spRenderer.sprite = rainSprite;
                break;

                default:
                Debug.Log($"Unknown attack type of {type}");
                break;
            }
        }
    }

    public bool IsFired
    {
        get { return isFired; }
        set
        {
            isFired = value;

            spRenderer.enabled = isFired;
            collide.enabled = isFired;

            //Rotate based on player movement
            if (!isFired)
            {
                timer = 0f;
            }
            else
            {
                Vector2 facing = Chaahk.Instance.GetFacing();

                //This gives us 8 way fire, even though the player will only have 4 way facing...
                moveDir.x = facing.x;
                moveDir.y = facing.y;

                if (facing == Vector2.zero)
                    moveDir = Vector3.up; //fire upwards by default

                if (facing.y == 0)
                {
                    if (facing.x > 0)
                    {
                        //set euler angles to 90 0 0? (if we have side sprites for swing, set those and use sprite collision!)
                    }
                }
            }
        }
    }

    public void SetPosition(Vector3 pos) { transform.position = pos; }

    public void Init()
    {
        spRenderer = gameObject.GetComponent<SpriteRenderer>();
        collide = gameObject.GetComponent<Collider2D>();

        IsFired = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(damage);
            Debug.Log($"Hit enemy with {type}");
        }

        if (type != AttackType.Bees)
            IsFired = false;
    }

    void Update()
    {
        if (!isFired)
            return;

        timer += Time.deltaTime;
        if (timer >= lifespan)
        {
            IsFired = false;
            timer = 0f;
        }

        //move in a straight line
        transform.Translate(moveDir * speed * Time.deltaTime);
    }
}
