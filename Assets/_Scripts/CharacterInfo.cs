using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public float timeBeforeDeath =  60f;
    public float timeGainedFromItem = 20f;
    public float lowHealthThreshhold=20f;

    public void Start()
    {
     

    }

    public void RewardTime()
    {
        timeBeforeDeath += timeGainedFromItem;
        if (timeBeforeDeath > lowHealthThreshhold && emitter.IsPlaying())
        {
            emitter.Stop();
        }
    }
    StudioEventEmitter emitter;
    private void Update()
    {
        bool wentlow=false;
        if (timeBeforeDeath>lowHealthThreshhold)
        {
            wentlow = true;
        }
        timeBeforeDeath -= Time.deltaTime;
        if (timeBeforeDeath<lowHealthThreshhold && wentlow)
        {
            emitter = AudioManager.instance.CreateEventEmitter(FMODEvents.instance.characterDeathWarning,this.gameObject);
            emitter.Play();
        }
        if (timeBeforeDeath <= 0)
        {
            emitter.Stop();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.characterDeath);
            Destroy(gameObject);

        }
    }



}
