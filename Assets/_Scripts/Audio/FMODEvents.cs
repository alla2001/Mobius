using System.Collections;
using System.Collections.Generic; 
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            Debug.LogError("Secondary AudioManager destroyed on: " + gameObject.GetNameIncludingParents());
        }
        instance = this;
    }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Events")]
    [field: SerializeField] public EventReference rewardCollected { get; private set; }
    [field: SerializeField] public EventReference characterControlRequest { get; private set; }
    [field: SerializeField] public EventReference bridgeFirstClick { get; private set; }
    [field: SerializeField] public EventReference bridgeSecondClick { get; private set; }
    [field: SerializeField] public EventReference bridgeNotPossible { get; private set; }
    [field: SerializeField] public EventReference bridgeInConstruction { get; private set; }
    [field: SerializeField] public EventReference characterDeathWarning { get; private set; }
    [field: SerializeField] public EventReference characterDeath { get; private set; }

    [field: Header("SoundLayers")]
    [field: SerializeField] public List<EventReference> shapeSounds { get; private set; }
    [field: SerializeField] public List<EventReference> bridgeSounds { get; private set; }
    [field: SerializeField] public List<EventReference> characterSounds { get; private set; }
    [field: SerializeField] public List<EventReference> itemSounds { get; private set; }

}