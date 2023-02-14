using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject Ijspegel;

    public float xRange;
    public float zRange;

    void Start()
    {
        InvokeRepeating("SpawnObject", 0, 5);
    }


    void SpawnObject()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-xRange, xRange), 10, Random.Range(-zRange, zRange));
        Instantiate(Ijspegel, spawnPosition, Quaternion.identity);
    }
}
