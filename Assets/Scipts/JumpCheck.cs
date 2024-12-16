using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    private Animator animator;

    // seperate child object with collider attached to player is used to detect ground for jumping purposes. This is
    // to prevent jumping off of walls

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // prevent infinite jumps by enabling jump after touching ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            transform.parent.GetComponent<Animator>().SetTrigger("Walk");
            transform.parent.GetComponent<Player>().JumpCheck(false);
        }
    }


    // sets jumping animation while falling, prevents actions while falling
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.parent.GetComponent<Animator>().SetTrigger("Jump");
            transform.parent.GetComponent<Player>().JumpCheck(true);
        }
    }
}
