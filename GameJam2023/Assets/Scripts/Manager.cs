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


    private float timer = 5.0f;
    private bool timerStarted = false;

    public Text TimerText;

    bool startGame = true;

    public static GameObject[] Players;


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

        if (timer > 0)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Playerke"))
            {
                player.transform.GetChild(0).GetComponent<PlayerController>().canMove = false;
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
        else if (timer > 0)
            TimerText.text = "Waiting for more players!";

        if (GameObject.FindGameObjectsWithTag("Playerke").Length > 1)
        {
            StartTimer();
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

        if (startGame)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Playerke"))
            {
                player.transform.GetChild(0).GetComponent<PlayerController>().canMove = true;
            }
            InvokeRepeating("SpawnObject", 0, 0.5f);

            Players = GameObject.FindGameObjectsWithTag("Playerke");

            startGame = false;
        }
    }

    void SpawnObject()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-xRange, xRange), 30, Random.Range(-zRange, zRange));
        Instantiate(Ijspegel, spawnPosition, Quaternion.identity);
    }
}
