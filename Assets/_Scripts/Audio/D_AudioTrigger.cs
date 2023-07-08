using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using UnityEditor.PackageManager;

public class D_AudioTrigger : MonoBehaviour
{
    public KeyCode playSoundKey; 

    public AudioLayerType audioLayerType;
    public StudioEventEmitter eventEmitter; 

    private void Start()
    {
        eventEmitter = AudioManager.instance.AddEventEmitterComponent(FMODEvents.instance.shapeSounds.GetRandomElement(), this.gameObject); 
        AudioManager.instance.AddEventEmitter(SoundMode.godMode, audioLayerType, GetComponent<StudioEventEmitter>(), null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(playSoundKey))
        {
            AudioManager.instance.AddEventEmitter(SoundMode.characterMode, audioLayerType, GetComponent<StudioEventEmitter>(), null);
        }
    }

    [ContextMenu("SwtichMode")]
    public void SwitchSoundMode()
    {
        if (AudioManager.instance.soundModeActive == SoundMode.godMode) 
        { 
            AudioManager.instance.ChangeSoundMode(SoundMode.characterMode);
        }
        else
        {
            AudioManager.instance.ChangeSoundMode(SoundMode.godMode);
        }
    }


    public List<StudioEventEmitter> playingEventEmitters; 
    [ContextMenu("FindAllEventEmittersPlaying")]
    public void FindAllEventEmittersPlaying()
    {
        StudioEventEmitter[] eventEmitters = FindObjectsOfType<StudioEventEmitter>(); 
        foreach(StudioEventEmitter eventEmitter in eventEmitters)
        {
            if (eventEmitter.IsPlaying())
            {
                playingEventEmitters.Add(eventEmitter);
            }
        }
    }
}
