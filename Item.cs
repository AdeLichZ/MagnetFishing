using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool canTrade;

    public void ChaseQuality()
    {
        int quality = Random.Range(0, 2);
        if(quality == 0)
        {
            canTrade = true;
        }
        if(quality == 1)
        {
            canTrade = false;
        }
    }
}
