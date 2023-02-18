using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : MonoBehaviour
{
    GameObject player;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.parent.gameObject.tag == "Playerke")
        {
            // stunned
            collision.gameObject.GetComponent<PlayerController>().StunPlayer();
            Destroy(gameObject);
        }
    }
}
