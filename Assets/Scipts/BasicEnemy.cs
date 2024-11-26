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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        // InvokeRepeating("Attack", Time.deltaTime, 3);
        
    }

    // Update is called once per frame
    void Update()
    {

        // attack if player is close
        if(Mathf.Abs(player.transform.position.x - transform.position.x) <= 2 && Time.time - attackCooldown >= 3 && !canBeHit)
        {
            attackCooldown = Time.time;
            StartCoroutine(Attack());
        }

        // enemy dead
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator Stunned()
    {
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
