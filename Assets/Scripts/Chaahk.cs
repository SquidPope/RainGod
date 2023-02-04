using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Chaahk : MonoBehaviour
{
    // Script controling the player character

    [SerializeField] int attackCount;
    [SerializeField] float attackOffset; //ToDo: Make sure the attacks can't collide with the player
    [SerializeField] GameObject attackPrefab;

    List<Attack> attackPool; //Object pool of attacks
    Rigidbody2D rigid;
    float speed = 70f;
    Vector2 direction = Vector2.zero;
    Vector2 lastDir = Vector2.zero;

    int listId = 0;

    static Chaahk instance;
    public static Chaahk Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                instance = go.GetComponent<Chaahk>();
            }

            return instance;
        }
    }

    public Vector2 GetFacing() { return lastDir; }

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();

        //Create a pool of attack ojects
        attackPool = new List<Attack>();
        GameObject attack;
        for (int i = 0; i < attackCount; i++)
        {
            attack = GameObject.Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
            attackPool.Add(attack.GetComponent<Attack>());
        }
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

    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        rigid.MovePosition(rigid.position + direction * speed * Time.deltaTime);

        if (direction.x != 0 || direction.y != 0)
            lastDir = direction; //Save the latest non-zero input, use for attack direction

        direction = Vector2.zero; //Prevent sliding

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
    }
}
