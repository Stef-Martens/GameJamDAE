using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool onGround = true;
    bool inHand = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!inHand)
        {
            if (onGround)
            {
                // ball oprapen en inhand op true
            }
        }
    }

}
