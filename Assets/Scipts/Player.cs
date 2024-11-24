using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private float horizontalInput;
    private Rigidbody2D rigidBody;
    public int speed;
    public int jumpHeight;
    private SpriteRenderer parryTest;
    private bool parrySuccess = false;
    private float parryActiveTime;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        parryTest = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        rigidBody.AddForce(Vector2.right * horizontalInput * Time.deltaTime * speed, ForceMode2D.Impulse);

        if (Input.GetKeyDown(KeyCode.K))
        {
            parryActiveTime = 0;
            StartCoroutine(Parry());
            Debug.Log("parry start");
            while(parryActiveTime <= 5f)
            {
                parryActiveTime += Time.deltaTime;
                Debug.Log(parryActiveTime);
                parrySuccess = true;
            }
            Debug.Log("parry end");
            parrySuccess = false;
        }

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.AddForce(Vector2.up * Time.deltaTime * jumpHeight, ForceMode2D.Impulse);
        }
        */

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy Attack"))
        {
            if(parrySuccess == true)
            {
                Debug.Log("parried");
            }

            else
            {
                Debug.Log("hit");
            }
        }
    }

    IEnumerator Parry()
    {
        parryTest.enabled = false;
        yield return new WaitForSeconds(0.5f);
        parryTest.enabled = true;
    }
}
