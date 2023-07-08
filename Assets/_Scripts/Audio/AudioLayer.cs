using FMODUnity;
using System;
using System.Collections.Generic; 
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public enum AudioLayerType
{
    HANGDRUM, RHYTHMDRUMS, REWARDCOLLECTED, CHARACTER, ITEM
}
public abstract class AudioLayer : MonoBehaviour
{
    public AudioLayerType layerType;
    public List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>(); 
    public bool isActive = false; 

    public virtual void Play()
    {
        isActive = true; 
    }

    public virtual void Stop()
    {
        isActive= false;
    }
    
    public abstract void BeatUpdate(int bar, int beat); 

    public virtual bool AddEventEmitter(StudioEventEmitter eventEmitter, object args)
    {
        return eventEmitters.AddAvoidDuplicate(eventEmitter); 
    }

    public virtual bool RemoveEventEmitter(StudioEventEmitter eventEmitter)
    {
        return eventEmitters.Remove(eventEmitter); 
    }
}