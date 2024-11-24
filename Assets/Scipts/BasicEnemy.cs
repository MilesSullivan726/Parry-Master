using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    public GameObject attackHitbox;
    private Vector3 attackPos;

    // Start is called before the first frame update
    void Start()
    {
        
        InvokeRepeating("Attack", Time.deltaTime, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Attack()
    {
        attackPos = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
        Instantiate(attackHitbox, attackPos, transform.rotation);
    }
}
