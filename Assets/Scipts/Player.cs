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
    private bool onGround = true;

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
        rigidBody.AddForce(Vector2.right * horizontalInput * Time.deltaTime * speed, ForceMode2D.Impulse);

        //track direction for spawning of attack hitbox
        if (Input.GetKey(KeyCode.A))
        {
            spriteRenderer.flipX = true;
            //animator.SetBool("Idle", false);
            //animator.SetBool("Walk", true);
            animator.SetTrigger("Walk");
            direction = 0; //left
        }

        else if (Input.GetKey(KeyCode.D))
        {
            spriteRenderer.flipX = false;
            //animator.SetBool("Idle", false);
            //animator.SetBool("Walk", true);
            animator.SetTrigger("Walk");
            direction = 1; //right
        }
        else
        {
            animator.SetTrigger("Idle");
            //animator.SetBool("Walk", false);
            //animator.SetBool("Idle", true);

        }

        //parry
        if (Input.GetKeyDown(KeyCode.K) && (Time.time - lastParry >= 1f))
        {
            lastParry = Time.time;
            StartCoroutine(Parry());
        }

        //attack
        if (Input.GetKeyDown(KeyCode.L) && Time.time - lastAttack >= 0.5f)
        {
            lastAttack = Time.time;
            Attack(direction);
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped)
        {
            onGround = false;
            hasJumped = true;
            
            rigidBody.AddForce(Vector2.up * Time.deltaTime * jumpHeight, ForceMode2D.Impulse);
        }

        if (!onGround)
        {
            animator.SetTrigger("Jump");
        }

        //game over
        if(hp <= 0)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // prevent infinite jumps
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasJumped = false;
            onGround = true;
        }


    }

    private void Attack(int direction)
    {
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy Attack"))
        {
            if(parrySuccess == true)
            {
                Debug.Log("parried");
                //stun attacking enemy so they can be damaged
                StartCoroutine(collision.transform.parent.GetComponent<BasicEnemy>().Stunned());
            }

            else
            {
                hp -= 1;
                hpCounter.text = "HP: " + hp;
                Debug.Log("hit");
            }
        }
    }

    IEnumerator Parry()
    {
        parrySuccess = true;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.enabled = true;
        parrySuccess = false;
    }
}
