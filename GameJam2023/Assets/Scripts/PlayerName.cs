using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    public Text PlayerNameText;
    void Start()
    {
        PlayerNameText.text = gameObject.name;
    }

}
