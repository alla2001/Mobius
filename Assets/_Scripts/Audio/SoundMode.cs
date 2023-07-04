using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundMode
{
    public string name;

    [field: SerializeField] public EventReference startingEvent { get; private set; }
    [field: SerializeField] public int deltaBarsToLayerStart { get; private set; }
    [field: SerializeField] public List<AudioLayerManager> audioLayers { get; private set; }
}