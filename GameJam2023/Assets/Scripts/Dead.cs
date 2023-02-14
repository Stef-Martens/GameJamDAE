using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public static List<string> leaderboard;

    void Start()
    {
        leaderboard = new List<string>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.parent.gameObject.tag == "Playerke")
        {
            leaderboard.Insert(0, collision.gameObject.transform.parent.name);
            Destroy(collision.gameObject.transform.parent.gameObject);

            if (GameObject.FindGameObjectsWithTag("Playerke").Length == leaderboard.Count + 1)
            {
                leaderboard.Insert(0, GameObject.FindGameObjectWithTag("Playerke").gameObject.name);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        else
        {
            Destroy(collision.gameObject);
        }
    }
}
