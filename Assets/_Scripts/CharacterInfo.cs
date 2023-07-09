using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CharacterInfo : MonoBehaviour
{
    public float timeBeforeDeath =  60f;
    public float timeGainedFromItem = 20f;
    public float lowHealthThreshhold=20f;

    public void Start()
    {
        emitter = AudioManager.instance.AddEventEmitterComponent(FMODEvents.instance.characterDeathWarning, this.gameObject);
    }

    public void RewardTime()
    {
        timeBeforeDeath += timeGainedFromItem;
        if (timeBeforeDeath > lowHealthThreshhold && emitter.IsPlaying())
        {
            emitter.Stop();
        }
    }
    FMODUnity.StudioEventEmitter emitter;
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
            
            emitter.Play();
        }
        if (timeBeforeDeath <= 0)
        {
            emitter.Stop();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.characterDeath);
            if (tag == "Player")
            {
                GameManager.Instance.ChangeState(GameState.GodView); 
            }
            Destroy(gameObject); //should play deathAnimation here

        }
    }



}
