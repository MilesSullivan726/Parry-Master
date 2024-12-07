using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{

    private float horizontalInput;
    private Rigidbody2D rigidBody;
    public int speed;
    public int jumpHeight;
    public bool parrySuccess = false;
    public GameObject attackPrefab;
    private int direction;
    private Vector3 attackPos;
    private int hp = 3;
    public TextMeshProUGUI hpCounter;
    private bool hasJumped = false;
    private float lastParry = 0;
    private float lastAttack = 0;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking;
    private bool isDead = false;
    public GameObject spark;
    private bool iFrames = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        

        //track direction for spawning of attack hitbox
        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.flipX = true;
            
            direction = 0; //left
            if (!hasJumped)
            {
                animator.SetTrigger("Walk");
            }
        }

        else if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.flipX = false;
            
            direction = 1; //right
            if (!hasJumped)
            {
                animator.SetTrigger("Walk");
            }
        }
        else if(!hasJumped)
        {
            animator.SetTrigger("Idle");

        }

        //parry
        if (Input.GetKeyDown(KeyCode.K) && (Time.time - lastParry >= 0.8f) && !hasJumped)
        {
            lastParry = Time.time;
            StartCoroutine(Parry());
        }

        //attack
        if (Input.GetKeyDown(KeyCode.L) && Time.time - lastAttack >= 0.5f && !hasJumped)
        {
            
            lastAttack = Time.time;
            StartCoroutine(Attack(direction));
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped && !isAttacking && !isDead)
        {

            hasJumped = true;
            rigidBody.velocity = Vector2.zero;
            // rigidBody.AddForce(Vector2.up * Time.deltaTime * jumpHeight, ForceMode2D.Impulse);
            rigidBody.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
        }

        if (hasJumped)
        {
            animator.SetTrigger("Jump");
        }

        //game over
        if(hp <= 0)
        {
            animator.SetTrigger("Dead");
            isDead = true;
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
            
            
        }

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
        }

    }

    private void FixedUpdate()
    {
        rigidBody.AddForce(Vector2.right * horizontalInput * Time.deltaTime * speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // prevent infinite jumps
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetTrigger("Walk");
            hasJumped = false;
        }


    }

    IEnumerator Attack(int direction)
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        if (direction == 0) // facing left
        {
            attackPos = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
            Instantiate(attackPrefab, attackPos, transform.rotation, transform);
        }

        if (direction == 1) // facing right
        {
            attackPos = new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z);
            Instantiate(attackPrefab, attackPos, transform.rotation, transform);
        }

        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.4f);
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy Attack"))
        {
            if(parrySuccess == true)
            {
                Debug.Log("parried");
                //stun attacking enemy so they can be damaged
                Instantiate(spark, transform);
                if (collision.transform.parent.CompareTag("Basic Enemy"))
                {
                    StartCoroutine(collision.transform.parent.GetComponent<BasicEnemy>().Stunned());
                }

                else if (collision.transform.parent.CompareTag("Tough Enemy"))
                {
                    StartCoroutine(collision.transform.parent.GetComponent<ToughEnemy>().Stunned());
                }

                else if (collision.transform.parent.CompareTag("Boss"))
                {
                    StartCoroutine(collision.transform.parent.GetComponent<Boss>().Stunned());
                }
            }

            else if (hp != 0 && !iFrames)
            {
                hp -= 1;
                hpCounter.text = "HP: " + hp;
                Debug.Log("hit");
                StartCoroutine(HitFlash());
            }
        }
    }

    public void Invincible()
    {
        iFrames = true;
    }

    IEnumerator Parry()
    {
        parrySuccess = true;
        animator.SetTrigger("Parry");
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.5f);
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        parrySuccess = false;
    }

    IEnumerator HitFlash()
    {
        iFrames = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
        iFrames = false;
    }
}
