using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundMode
{
    public string name;

    [field: SerializeField] public EventReference startingEvent { get; private set; }
    [field: SerializeField] public int deltaBarsToLayerStart { get; private set; }
    [field: SerializeField] public List<AudioLayerManager> audioLayers { get; private set; }

    //zypernKatze make sure that the soundModes get used correctly

    public void StopSoundMode()
    {
        //zypernKatze write this for soundModes to work
    }

    public void StartSoundMode()
    {
        //zypernKatze write this for soundModes to work 
    }
}