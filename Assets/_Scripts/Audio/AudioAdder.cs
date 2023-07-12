using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAdder : MonoBehaviour
{
    public AudioLayerType audioLayer;
    public FMODUnity.StudioEventEmitter eventEmitter;

    void Start()
    {
        //if (audioLayer == AudioLayerType.HANGDRUM)
        //eventEmitter.EventReference = FMODEvents.instance.shapeSounds.GetRandomElement();
        //else if (audioLayer == AudioLayerType.RHYTHMDRUMS)
        //AudioManager.instance.AddEventEmitter(AudioMode.godMode, audioLayer, eventEmitter, null);
    }

    void Update()
    {
        
    }
}
