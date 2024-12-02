using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedDespawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DespawnObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DespawnObject()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
