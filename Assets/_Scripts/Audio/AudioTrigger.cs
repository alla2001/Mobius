using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.EventReference soundEvent;
    private FMOD.Studio.EventInstance soundEventInstance;
    private FMOD.Studio.PLAYBACK_STATE soundEventState; 

    private void Start()
    {
        soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(soundEvent);

    }

    private void Update()
    { 
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioManager.instance.ChangeMode(AudioManager.SoundMode.RewardMode);  
        }
    }

    public void PlaySoundEvent()
    {
        RuntimeManager.PlayOneShot(soundEvent); 
    }
}
