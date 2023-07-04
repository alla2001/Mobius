using System.Collections;
using System.Collections.Generic; 
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    [System.Serializable]
    public class SoundLayerCollection
    {
        AudioLayerManager.LayerType type;
        [field: SerializeField]
        public EventReference[] sounds; 
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.LogError("Secondary Soundmanager destroyed on: " + gameObject.name);
        }
        instance = this;
    }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }


    [field: Header("SoundModes")]
    [field: SerializeField] public SoundMode godMode { get; private set; }
    [field: SerializeField] public SoundMode characterMode { get; private set; }
    [field: SerializeField] public SoundMode rewardMode { get; private set; }


    [field: Header("Events")]
    [field: SerializeField] public EventReference rewardCollected { get; private set; }
    [field: SerializeField] public EventReference characterControlRequest { get; private set; }

    [field: Header("SoundLayers")]
    [field: SerializeField] public EventReference[] shapeSounds { get; private set; }
}