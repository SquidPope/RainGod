using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthChangeEvent : UnityEvent<float> {}

[RequireComponent(typeof(Rigidbody2D))]
public class Chaac : MonoBehaviour
{
    // Script controling the player character

    [SerializeField] int attackCount;
    [SerializeField] float attackOffset; //ToDo: Make sure the attacks can't collide with the player
    [SerializeField] GameObject attackPrefab;

    new Collider2D collider;

    List<Attack> attackPool; //Object pool of attacks
    Rigidbody2D rigid;
    float speed = 7f;
    Vector2 direction = Vector2.zero;
    Vector2 lastDir = Vector2.zero;

    float healthMax = 100f;
    float health;

    float iframeTimer = 0f;
    float iframeTimerMax = 0.5f; //ToDo: tweak

    int listId = 0;

    bool isPlaying = true;
    bool hasBees = false;

    HealthChangeEvent healthChange = new HealthChangeEvent();
    public HealthChangeEvent HealthChange { get{ return healthChange; }}

    static Chaac instance;
    public static Chaac Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                instance = go.GetComponent<Chaac>();
            }

            return instance;
        }
    }

    float Health
    {
        get { return health; }
        set
        {
            health = value;
            HealthChange.Invoke(health);
        }
    }

    public Vector2 GetFacing() { return lastDir; }
    public Vector3 GetPosition() { return transform.position; }

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();

        Health = healthMax;

        collider = gameObject.GetComponent<Collider2D>();

        //Create a pool of attack ojects
        attackPool = new List<Attack>();
        GameObject attack;
        
        for (int i = 0; i < attackCount; i++)
        {
            attack = GameObject.Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
            Attack a = attack.GetComponent<Attack>();
            a.Init();
            attackPool.Add(a);
        }

        GameController.Instance.StateChange.AddListener(StateChange);

        //Project setting didn't work, presumably because it's using the 3D version for this 2D project, so setting it in code
        Physics2D.IgnoreLayerCollision(6, 6);
        
    }

    void StateChange(GameState state)
    {
        isPlaying = state == GameState.Playing;
    }

    public float GetMaxHealth() { return healthMax; }

    public void LoseBees() { hasBees = false; }

    public void TakeDamage(float amount)
    {
        if (iframeTimer > 0f) //Too soon since last hit.
            return;

        Health -= amount;

        if (health <= 0f)
        {
            GameController.Instance.State = GameState.Over; //ToDo: A death animation would be nice
        }

        iframeTimer = iframeTimerMax;
    }

    void Attack(AttackType type)
    {
        Attack current = attackPool[listId];
        //ToDo: Check that current is not active, find an inactive one if it is

        current.SetPosition(transform.position + new Vector3(lastDir.x, lastDir.y, 0f) * attackOffset); //ToDo: Is there a better way than just making a new Vector3 every time?
        current.Type = type;
        current.IsFired = true;

        if (listId >= attackPool.Count - 1)
            listId = 0;
        else
            listId++;
    }

    void FixedUpdate()
    {
        if (!isPlaying)
            return;

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        rigid.MovePosition(rigid.position + direction * speed * Time.deltaTime);

        if (direction.x != 0 || direction.y != 0)
            lastDir = direction; //Save the latest non-zero input, use for attack direction

        direction = Vector2.zero; //Prevent sliding

        
    }

    void Update()
    {
        //ToDo: Limit how often Chaahk can attack, this is frankly ridiculous
        if (Input.GetMouseButtonUp(0))
        {
            //ToDo: Move to a separate function, right click is all this but different attack type
            Attack(AttackType.Axe);
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            Attack(AttackType.Rain);
        }

        if (Input.GetKeyUp(KeyCode.B) || Input.GetKeyUp(KeyCode.Space))
        {
            if (!hasBees)
            {
                Attack(AttackType.Bees);
                hasBees = true;
            }
        }

        if (iframeTimer > 0f)
        {
            iframeTimer -= Time.deltaTime; //Are they still iframes if they're detached from the frame rate?
        }
    }
}
