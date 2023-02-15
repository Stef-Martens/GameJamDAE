using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject Ijspegel;

    public float xRange;
    public float zRange;

    public Camera StartCamera;

    int playerCount = 1;


    private float timer = 7.0f;
    private bool timerStarted = false;

    public Text TimerText;


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


        if (timerStarted)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                TimerDone(); // Call your function when the timer is done
                timerStarted = false; // Stop the timer
            }
        }

        if (GameObject.FindGameObjectsWithTag("Playerke").Length > 1)
        {
            StartTimer();
        }
        else
        {
            // wachten + movement stop
            // ....
        }
    }

    public void StartTimer()
    {
        timerStarted = true;
        TimerText.text = "Game starts in: " + (Mathf.Round(timer * 10.0f) / 10.0f).ToString() + "s";
    }

    void TimerDone()
    {
        GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInputManager>().DisableJoining();
        TimerText.enabled = false;
        // movement starten
        // ...
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
