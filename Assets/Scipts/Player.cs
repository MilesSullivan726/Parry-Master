using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public int hp = 5;
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
    public Canvas gameOverScreen;
    private AudioSource audioSource;
    public AudioClip hurtSFX;
    public AudioClip attackSFX;
    public AudioClip parryStartSFX;
    public AudioClip parrySuccessSFX;
    public AudioClip jumpSFX;
    public AudioClip footStep1;
    public AudioClip footStep2;
    public AudioClip dead;
    private float lastStep;
    public GameObject fadeOut;
    public GameObject music;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

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

            // play footstep sfx if enough time has passed between last step
            if (Time.time - lastStep > 0.6f && !hasJumped)
            {
                lastStep = Time.time;
                StartCoroutine(FootSteps());
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

            // play footstep sfx if enough time has passed between last step
            if (Time.time - lastStep > 0.6f && !hasJumped)
            {
                lastStep = Time.time;
                StartCoroutine(FootSteps());
            }
        }

        // idle if not jumping or moving
        else if(!hasJumped)
        {
            animator.SetTrigger("Idle");

        }

        //parry
        if (Input.GetKeyDown(KeyCode.K) && (Time.time - lastParry >= 0.8f) && !hasJumped && !isDead)
        {
            lastParry = Time.time;
            StartCoroutine(Parry());
        }

        //attack
        if (Input.GetKeyDown(KeyCode.L) && Time.time - lastAttack >= 0.5f && !hasJumped && !isDead)
        {
            
            lastAttack = Time.time;
            StartCoroutine(Attack(direction));
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped && !isAttacking && !isDead)
        {
            audioSource.PlayOneShot(jumpSFX);
            hasJumped = true;
            rigidBody.velocity = Vector2.zero;
            rigidBody.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
        }

        // ensures jump animation plays when falling
        if (hasJumped)
        {
            animator.SetTrigger("Jump");
        }

        //game over
        if(hp <= 0 && !isDead)
        {
            audioSource.PlayOneShot(dead);
            Destroy(music);
            gameOverScreen.gameObject.SetActive(true);
            animator.SetTrigger("Dead");
            isDead = true;
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
            
            
        }

        // halts all player movement when A or D is released to prevent floatiness and make controls snappier
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
        }

    }

    // move player based on input
    private void FixedUpdate()
    {
        rigidBody.AddForce(Vector2.right * horizontalInput * Time.deltaTime * speed, ForceMode2D.Impulse);
    }

    // play footstep sfx on certain loop. Prevent footstep sfx when jumping
    IEnumerator FootSteps()
    {
        if (!hasJumped && !isAttacking && !parrySuccess && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        {
            audioSource.PlayOneShot(footStep1);
        }
        yield return new WaitForSeconds(0.3f);

        if (!hasJumped && !isAttacking && !parrySuccess && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        {
            audioSource.PlayOneShot(footStep2);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // victory fanfare once boss is defeated
    public IEnumerator Victory()
    {
        music.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Win Screen");       
    }
    
    // used in conjunction with child object to detect when the player can jump again
    public void JumpCheck(bool jumped)
    {
        hasJumped = jumped;     
    }

    // attack in current direction by playing animation and spawning hitbox
    IEnumerator Attack(int direction)
    {
        audioSource.PlayOneShot(attackSFX);
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

        // prevent player movement while attacking
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.4f);
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy Attack") && !iFrames)
        {

            // if the player is parrying and is hit, increments stun counter of attacking enemy
            if(parrySuccess == true)
            {
                audioSource.PlayOneShot(parrySuccessSFX);
                StartCoroutine(ParryIFrame());

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

            // take damage if hit when not parrying or otherwise invulnerable
            else if (hp != 0 && !iFrames)
            {
                audioSource.PlayOneShot(hurtSFX);
                hp -= 1;
                hpCounter.text = "HP: " + hp;
                StartCoroutine(HitFlash());
            }
        }

        if(collision.gameObject.CompareTag("Next Level"))
        {
            SceneManager.LoadScene("Level Transition", LoadSceneMode.Single);
        }
    }

    // iframes prevent extra damage from a single attack
    IEnumerator ParryIFrame()
    {
        iFrames = true;
        yield return new WaitForSeconds(0.4f);
        iFrames = false;
    }

    // triggered when boss dies to prevent player death from lingering boss attack hitbox
    public void Invincible()
    {
        iFrames = true;
    }

    // activates and closes the parry window which will stun enemies when attacked during duration
    IEnumerator Parry()
    {
        audioSource.PlayOneShot(parryStartSFX);
        parrySuccess = true;
        animator.SetTrigger("Parry");
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.5f);
        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        parrySuccess = false;
    }

    // flash red when damaged
    IEnumerator HitFlash()
    {
        iFrames = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
        iFrames = false;
    }
}
