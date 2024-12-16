using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    public GameObject attackHitbox;
    private Vector3 attackPos;
    public int hp = 3;
    private GameObject player;
    private bool canBeHit = false;
    private Animator animator;
    private float attackCooldown;
    private Rigidbody2D rigidBody;
    public float speed;
    private bool attacking = false;
    public GameObject stunHalo;
    public GameObject deathExp;
    private float distanceToPlayer;
    private SpriteRenderer spriteRenderer;
    private int direction;
    private AudioSource audioSource;
    private bool heightCheck;
    public float height = 5;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // determine distance to player
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // checks if enemy is within appropriate height range of player so enemy does not move or act when player far above or below
        if(Mathf.Abs(player.transform.position.y - transform.position.y) <= height)
        {
            heightCheck = true;
        }
        else
        {
            heightCheck = false;
        }

        // attack if player is close
        if (distanceToPlayer <= 2.5f && Time.time - attackCooldown >= 3 && !canBeHit && heightCheck)
        {
            animator.SetTrigger("Idle");
            attacking = true;
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            attackCooldown = Time.time;
            StartCoroutine(Attack(direction));
        }

        // approach player if within appropriate range
        else if (distanceToPlayer > 2 && distanceToPlayer < 7 && !attacking && !canBeHit && heightCheck)
        {
            animator.SetTrigger("Walk");
            if (player.transform.position.x - transform.position.x < 0)
            {
                direction = 0; // left
                spriteRenderer.flipX = false;
                rigidBody.AddForce(Vector2.left * Time.deltaTime * speed, ForceMode2D.Impulse);
            }
            else
            {
                direction = 1; // right
                spriteRenderer.flipX = true;
                rigidBody.AddForce(Vector2.right * Time.deltaTime * speed, ForceMode2D.Impulse);
            }
           
        }

        // idle if player is too far
        else
        {
            animator.SetTrigger("Idle");
        }

        // enemy dead
        if (hp <= 0)
        {
            Instantiate(deathExp, new Vector2(transform.position.x + 1, transform.position.y), transform.rotation);
            Destroy(gameObject);
        }
    }

    // become stunned momentarily, allowing for a damage window
    public IEnumerator Stunned()
    {
        animator.SetTrigger("Idle");
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        Instantiate(stunHalo, new Vector2(transform.position.x, transform.position.y + 2), transform.rotation, gameObject.transform);
        canBeHit = true;
        yield return new WaitForSeconds(2);
        canBeHit = false;
    }

    // attack in appropiate direction by playing animation and spawning hitbox
    IEnumerator Attack(int direction)
    {
        audioSource.Play();
        animator.SetTrigger("Prepare");
        yield return new WaitForSeconds(0.75f);
        animator.SetTrigger("Attack");
        audioSource.Play();
        if (direction == 0)
        {
            attackPos = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
        }
        else if (direction == 1)
        {
            attackPos = new Vector3(transform.position.x + 2.0f, transform.position.y, transform.position.z);
        }
        Instantiate(attackHitbox, attackPos, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("Done Attack");
        attacking = false;

    }

    // take damage on hit
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player Attack"))
        {
            StartCoroutine(HitFlash());
            hp -= 1;
        }
    }

    // flash red when hit
    IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
    }


}
