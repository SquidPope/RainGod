using UnityEngine;

public enum AttackType {Axe, Bees, Rain}

[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    // Script controling the effect/hitbox of a player's attack

    [SerializeField] GameObject axeIcon;
    [SerializeField] GameObject beesIcon;
    [SerializeField] GameObject rainIcon;

    float timer = 0f;
    bool isFired = false;
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
            

            switch(type) //ToDo: Flip required icon based on whether the player is facing left or right
            {
                case AttackType.Axe:
                damage = 4f;
                lifespan = 0.4f;
                speed = 5f;
                axeIcon.SetActive(true);
                break;

                case AttackType.Bees:
                damage = 0.25f;
                lifespan = 4f;
                speed = 0f;
                beesIcon.SetActive(true);
                break;

                case AttackType.Rain:
                damage = 0f;
                lifespan = 0.3f;
                speed = 20f;
                rainIcon.SetActive(true);
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

            collide.enabled = isFired;

            if (!isFired)
            {
                timer = 0f;
                axeIcon.SetActive(false);
                beesIcon.SetActive(false);
                rainIcon.SetActive(false);

                if (type == AttackType.Bees)
                    Chaac.Instance.LoseBees();
            }
            else
            {
                Vector2 facing = Chaac.Instance.GetFacing();

                //This gives us 8 way fire, even though the player will only have 4 way facing...
                moveDir.x = facing.x;
                moveDir.y = facing.y;

                if (facing == Vector2.zero)
                    moveDir = Vector3.up; //fire upwards by default
            }
        }
    }

    public void SetPosition(Vector3 pos) { transform.position = pos; }
    public Collider2D GetCollider2D() { return collide; }

    public void Init()
    {
        //spRenderer = gameObject.GetComponent<SpriteRenderer>();
        collide = gameObject.GetComponent<Collider2D>();

        IsFired = false;
    }

    void OnCollisionStay2D(Collision2D other) //Should fire every frame? the two collide for
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.TakeDamage(damage, type);
            Debug.Log($"Hit enemy with {type}");
        }

        if (type == AttackType.Axe)
            IsFired = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Maize")
        {
            Maize maize = other.GetComponent<Maize>();
            maize.AttackHit(type);
        }
    }

    void FixedUpdate()
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
