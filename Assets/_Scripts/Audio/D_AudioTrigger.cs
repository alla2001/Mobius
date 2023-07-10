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
        eventEmitter = AudioManager.instance.AddEventEmitterComponent(FMODEvents.instance.GetRandomEventReferenceByAudioLayer(audioLayerType), this.gameObject); 
        AudioManager.instance.AddEventEmitter(AudioMode.godMode, audioLayerType, GetComponent<StudioEventEmitter>(), null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GameObject gO = new GameObject();
            gO.name = "newGameObject"; 
        }
        if (Input.GetKeyDown(playSoundKey))
        {
            AudioManager.instance.AddEventEmitter(AudioMode.characterMode, audioLayerType, GetComponent<StudioEventEmitter>(), null);
        }
    }

    [ContextMenu("SwtichMode")]
    public void SwitchSoundMode()
    {
        if (AudioManager.instance.soundModeActive == AudioMode.godMode) 
        { 
            AudioManager.instance.ChangeAudioMode(AudioMode.characterMode);
        }
        else
        {
            AudioManager.instance.ChangeAudioMode(AudioMode.godMode);
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
