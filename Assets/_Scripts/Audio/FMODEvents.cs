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

    [field: SerializeField] public EventReference emptyEvent { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Events")]
    [field: SerializeField] public EventReference rewardCollected { get; private set; }
    [field: SerializeField] public EventReference bridgeFirstClick { get; private set; }
    [field: SerializeField] public EventReference bridgeSecondClick { get; private set; }
    [field: SerializeField] public EventReference bridgeNotPossible { get; private set; }
    [field: SerializeField] public EventReference bridgeInConstruction { get; private set; }
    [field: SerializeField] public EventReference characterHoverOver { get; private set; }
    [field: SerializeField] public EventReference characterTakeControl { get; private set; }
    [field: SerializeField] public EventReference characterDeathWarning { get; private set; }
    [field: SerializeField] public EventReference characterDeath { get; private set; }

    [field: Header("SoundLayers")]
    [field: SerializeField] public List<EventReference> shapeSounds { get; private set; }
    [field: SerializeField] public List<EventReference> bridgeSounds { get; private set; }
    [field: SerializeField] public List<EventReference> characterSounds { get; private set; }
    [field: SerializeField] public List<EventReference> itemSounds { get; private set; }

    public EventReference GetRandomEventReferenceByAudioLayer(AudioLayerType audioLayerType)
    {
        switch (audioLayerType)
        {
            case AudioLayerType.HANGDRUM:
                return shapeSounds.GetRandomElement();
            case AudioLayerType.RHYTHMDRUMS:
                return bridgeSounds.GetRandomElement();
            case AudioLayerType.CHARACTER:
                return characterSounds.GetRandomElement();
            case AudioLayerType.ITEM:
                return itemSounds.GetRandomElement();
            default:
                Debug.LogError("trying to create a sound from an AudioLayerType with no sounds");
                return emptyEvent; 
        }
    }
}