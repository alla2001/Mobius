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
    public float timetoAddOnReward = 15f;
    public float warningSoundThreshold = 20f;

    public float energyPerStone = 10f;
    public GameObject[] energyStones; 
    private int currentAmountStones = 0;

    private int shouldAmountStones => (int)(timeBeforeDeath / energyPerStone); 

    private Animator characterAnimator;
    private SplineFollower splineFollower;
    private bool isDead;
    private float timeBeforeDespawn = 10f;

    public static List<CharacterInfo> characters = new List<CharacterInfo>();

    public void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        characterAnimator = GetComponentInChildren<Animator>();
        emitter = AudioManager.instance.AddEventEmitterComponent(FMODEvents.instance.characterDeathWarning, this.gameObject);

        characters.Add(this);
    }

    public void RewardTime()
    {
        timeBeforeDeath += timeGainedFromItem;
        if (timeBeforeDeath > warningSoundThreshold && emitter.IsPlaying())
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
        if (timeBeforeDeath>warningSoundThreshold)
        {
            wentlow = true;
        }
        timeBeforeDeath -= Time.deltaTime;
        if (timeBeforeDeath<warningSoundThreshold && wentlow)
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

            characters.Remove(this);
            isDead = true;
            Died();
        }

        updateEnergyStones(); 
    }

    public void AddEnergyToAllCharacters()
    {
        foreach (var character in characters)
        {
            CharacterInfo characterInfo = character.GetComponent<CharacterInfo>();
            characterInfo.timeBeforeDeath += characterInfo.timetoAddOnReward;
            GameManager.Instance.ChangeState(GameState.GodView);
            ItemSpawner.instace.SpawnItems();
        }
    }

    private void Died()
    {
        characters.Remove(this); 
        Destroy(gameObject, timeBeforeDespawn);
    }

    public void updateEnergyStones()
    {
        if (shouldAmountStones > currentAmountStones)
        {
            currentAmountStones++;
            if (currentAmountStones < energyStones.Length)
            {
                if (currentAmountStones > energyStones.Length-1) { }
                else if(energyStones[currentAmountStones] == null) { Debug.LogWarning("energyStone " + currentAmountStones + "is not setup in: " + gameObject.GetNameIncludingParents()); }
                else if (energyStones[currentAmountStones].GetComponent<ShapeDissolverLerper>() != null)
                {
                    energyStones[currentAmountStones].GetComponent<ShapeDissolverLerper>().shouldDissolveValue = 1.00f; 
                }
                else
                {
                    energyStones[currentAmountStones].SetActive(true);
                }
            }
            updateEnergyStones(); 
        }
        else if (shouldAmountStones < currentAmountStones)
        {
            if (currentAmountStones > energyStones.Length -1) { }
            else if (energyStones[currentAmountStones] == null) { Debug.LogWarning("energyStone " + currentAmountStones + "is not setup in: " + gameObject.GetNameIncludingParents()); }
            else if (energyStones[currentAmountStones].GetComponent<ShapeDissolverLerper>() != null)
            {
                energyStones[currentAmountStones].GetComponent<ShapeDissolverLerper>().shouldDissolveValue = 0.75f;
            }
            else
            {
                energyStones[currentAmountStones].SetActive(false);
            }

            currentAmountStones--;
            updateEnergyStones(); 
        }
    }
}
