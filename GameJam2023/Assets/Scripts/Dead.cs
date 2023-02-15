using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public static List<string> leaderboard;

    int deadcount = 0;

    void Start()
    {
        leaderboard = new List<string>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.parent.gameObject.tag == "Playerke")
        {
            leaderboard.Insert(0, collision.gameObject.transform.parent.name);

            collision.gameObject.transform.parent.gameObject.GetComponent<DeadPlayer>().Dead = true;
            Destroy(collision.gameObject.transform.parent.gameObject);

            deadcount++;


            if (GameObject.FindGameObjectsWithTag("Playerke").Length - 1 == deadcount)
            {
                foreach (var item in GameObject.FindGameObjectsWithTag("Playerke"))
                {
                    if (!item.GetComponent<DeadPlayer>().Dead)
                        leaderboard.Insert(0, item.name);
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        else
        {
            Destroy(collision.gameObject);
        }
    }
}
