using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        //follow player
        transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
