using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioAdder : MonoBehaviour
{
    public AudioLayerType audioLayer;
    public StudioEventEmitter eventEmitter;
    //public float minAttenuation, maxAttenuation;


    void Start()
    {
        if (eventEmitter == null)
        {
            eventEmitter = GetComponent<StudioEventEmitter>();
            if (eventEmitter == null)
            {
                eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
            }
        }
        if (eventEmitter.EventReference.IsNull)
        {
            if (audioLayer == AudioLayerType.RHYTHMDRUMS)
            {
                eventEmitter.EventReference = FMODEvents.instance.bridgeSounds.GetRandomElement();
            }
            else if (audioLayer == AudioLayerType.HANGDRUM)
            {
                eventEmitter.EventReference = FMODEvents.instance.shapeSounds.GetRandomElement();
            }
            eventEmitter.OverrideAttenuation = true;
            eventEmitter.OverrideMinDistance = 5; 
            eventEmitter.OverrideMaxDistance = 15;
        }
        
        AudioManager.instance.AddEventEmitter(AudioMode.godMode, audioLayer, eventEmitter, null); 
    }
}
