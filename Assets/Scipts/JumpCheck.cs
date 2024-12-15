using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // prevent infinite jumps
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            transform.parent.GetComponent<Animator>().SetTrigger("Walk");
            transform.parent.GetComponent<Player>().JumpCheck(false);
        }

      
      


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.parent.GetComponent<Animator>().SetTrigger("Jump");
            transform.parent.GetComponent<Player>().JumpCheck(true);
        }
    }
}
