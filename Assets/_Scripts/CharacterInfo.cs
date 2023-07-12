using Dreamteck.Splines;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

[SelectionBase]
public class CharacterInfo : MonoBehaviour
{
    public float timeBeforeDeath =  60f;
    public float timeGainedFromItem = 20f;
    public float lowHealthThreshhold=20f;
    public float timetoAddOnReward = 15f;

    private Animator characterAnimator;
    private SplineFollower splineFollower;
    private bool isDead;
    private float timeBeforeDespawn = 10f;

    public void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        characterAnimator = GetComponentInChildren<Animator>();
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
        //characterAnimator.speed = 1 - Mathf.InverseLerp(60f, 0f, timeBeforeDeath);

        //Debug.Log("character anim speed: " + characterAnimator.speed);

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
        if (timeBeforeDeath <= 0 && !isDead)
        {
            splineFollower.followSpeed = 0;
            emitter.Stop();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.characterDeath);
            if (tag == "Player")
            {
                GameManager.Instance.ChangeState(GameState.GodView); 
            }
            characterAnimator.SetTrigger("isDead");
            isDead = true;
            Died();
        }

    }

    private void Died()
    {
         CharacterMovement.characters.Remove(GetComponent<CharacterMovement>()); 
        Destroy(gameObject, timeBeforeDespawn);
    }
}
