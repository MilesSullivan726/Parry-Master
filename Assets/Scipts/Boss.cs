using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{

    public GameObject attackHitbox;
    private Vector3 attackPos;
    public int hp = 10;
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
    private int attackChoice;
    private int stunCount = 0;
    private int numAttacks;
    private AudioSource audioSource;
    public AudioClip attackSFX;
    public AudioClip deadSFX;
    private bool isDead = false;

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
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // attack if player is close
        if (distanceToPlayer <= 2.5f && Time.time - attackCooldown >= 3.5f && !canBeHit)
        {
            numAttacks = Random.Range(2, 4);
            animator.SetTrigger("Idle");
            attacking = true;
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            attackCooldown = Time.time;
            StartCoroutine(Attack(direction, numAttacks));
        }

        else if (distanceToPlayer > 2.5f && distanceToPlayer < 7 && !attacking && !canBeHit)
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

        else
        {
            animator.SetTrigger("Idle");
        }

        // enemy dead
        if (hp <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        audioSource.PlayOneShot(deadSFX);
        player.GetComponent<Player>().Invincible();
        canBeHit = true;
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(2.5f);
        Instantiate(deathExp, new Vector2(transform.position.x + 1, transform.position.y), transform.rotation);
        StartCoroutine(player.GetComponent<Player>().Victory());
        spriteRenderer.enabled = false;
        

    }

    public IEnumerator Stunned()
    {
        stunCount += 1;
        if (stunCount == 6)
        {
            stunCount = 0;
            animator.SetTrigger("Idle");
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            Instantiate(stunHalo, new Vector2(transform.position.x, transform.position.y + 3), transform.rotation, gameObject.transform);
            canBeHit = true;
            yield return new WaitForSeconds(2);
            canBeHit = false;
        }
    }

    IEnumerator Attack(int direction,int numAttacks)
    {
        for (int i = numAttacks; numAttacks != 0; numAttacks -= 1)
        {
            if (!canBeHit)
            {
                attackChoice = Random.Range(0, 3);

                if (attackChoice == 0 && !canBeHit)
                {
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Prepare 1");
                    yield return new WaitForSeconds(0.75f);
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Attack 1");
                }

                else if (attackChoice == 1 && !canBeHit)
                {
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Prepare 2");
                    yield return new WaitForSeconds(0.75f);
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Attack 2");
                }

                else if (attackChoice == 2 && !canBeHit)
                {
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Prepare 3");
                    yield return new WaitForSeconds(0.75f);
                    audioSource.PlayOneShot(attackSFX);
                    animator.SetTrigger("Attack 3");
                }

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
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player Attack") /*&& canBeHit*/)
        {
            StartCoroutine(HitFlash());
            hp -= 1;
            Debug.Log("Enemy HP: " + hp);
        }
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
    }


}
