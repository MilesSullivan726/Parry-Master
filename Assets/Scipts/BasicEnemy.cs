using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    public GameObject attackHitbox;
    private Vector3 attackPos;
    private int hp = 3;
    public GameObject player;
    private bool canBeHit = false;
    private Animator animator;
    private float attackCooldown;
    private Rigidbody2D rigidBody;
    public float speed;
    private bool attacking = false;
    public GameObject stunHalo;
    public GameObject deathExp;
    private float distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // attack if player is close
        if (distanceToPlayer <= 2 && Time.time - attackCooldown >= 3 && !canBeHit)
        {
            animator.SetTrigger("Idle");
            attacking = true;
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            attackCooldown = Time.time;
            StartCoroutine(Attack());
        }

        else if (distanceToPlayer > 2 && distanceToPlayer < 7 && !attacking && !canBeHit)
        {
            animator.SetTrigger("Walk");
           rigidBody.AddForce(Vector2.left * Time.deltaTime * speed, ForceMode2D.Impulse);
           
        }

        // enemy dead
        if (hp <= 0)
        {
            Instantiate(deathExp, new Vector2(transform.position.x + 1, transform.position.y), transform.rotation);
            Destroy(gameObject);
        }
    }

    public IEnumerator Stunned()
    {
        animator.SetTrigger("Idle");
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        Instantiate(stunHalo, new Vector2(transform.position.x + 1, transform.position.y + 2), transform.rotation, gameObject.transform);
        canBeHit = true;
        yield return new WaitForSeconds(2);
        canBeHit = false;
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("Prepare");
        yield return new WaitForSeconds(0.75f);
        animator.SetTrigger("Attack");
        attackPos = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
        Instantiate(attackHitbox, attackPos, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("Done Attack");
        attacking = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player Attack") && canBeHit)
        {
            hp -= 1;
            Debug.Log("Enemy HP: " + hp);
        }
    }

 
}
