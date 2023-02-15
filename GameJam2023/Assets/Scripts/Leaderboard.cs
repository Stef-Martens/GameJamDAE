using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public Text[] LeaderboardText;
    void Start()
    {
        LeaderboardText[0].text = "1: " + Dead.leaderboard[0];
        LeaderboardText[1].text = "2: " + Dead.leaderboard[1];
        if (Dead.leaderboard.Count > 2) LeaderboardText[2].text = "3: " + Dead.leaderboard[2];
    }

}
