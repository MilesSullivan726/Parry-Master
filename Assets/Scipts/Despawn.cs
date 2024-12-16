using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{

    public float despawnTime = 0.4f;
    public bool isExplosion = false;
    private AudioSource audioSource;

    // script is used for both explosion effects from deaths and hitboxes to despawn after short delay

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DespawnObject());

        audioSource = GetComponent<AudioSource>();

        if (isExplosion)
        {
            audioSource.Play();
        }
    }

    IEnumerator DespawnObject()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
