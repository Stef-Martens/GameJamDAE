using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject Ijspegel;

    public float xRange;
    public float zRange;

    public Camera StartCamera;

    int playerCount = 1;

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Playerke").Length != 0)
        {
            StartCamera.enabled = false;
        }

        // player namen
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Playerke");
        foreach (var player in Players)
        {
            if (player.gameObject.name == "Player(Clone)")
            {
                player.gameObject.name = "Player" + playerCount.ToString();
                playerCount++;
            }


        }
    }

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
