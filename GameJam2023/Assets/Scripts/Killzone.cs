using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(other.transform.parent.gameObject);
            Destroy(other.gameObject);
        }
    }
}
