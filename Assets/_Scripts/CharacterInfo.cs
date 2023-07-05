using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public float timeBeforeDeath =  60f;
    public float timeGainedFromItem = 20f;

    public void Start()
    {
     

    }

    public void RewardTime()
    {
        timeBeforeDeath += timeGainedFromItem;
    }
    private void Update()
    {

        timeBeforeDeath -= Time.deltaTime;
        if (timeBeforeDeath <= 0)
        {
            Destroy(gameObject);

        }
    }



}
