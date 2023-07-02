using System.Collections;
using System.Collections.Generic; 
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    [System.Serializable]
    public class SoundMode
    {
        [field: SerializeField] public EventReference startingEvent { get; private set; }
        [field: SerializeField] public List<EventReference> continuousSounds { get; private set; }
        [field: SerializeField] public int deltaBarsToStart { get; private set; }
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


    [field: Header("Reward")]
    [field: SerializeField] public EventReference collectible { get; private set; }

    [field: Header("SoundModes")]
    [field: SerializeField] public SoundMode rewardChoice { get; private set; }
    [field: SerializeField] public SoundMode godView { get; private set; }
    [field: SerializeField] public SoundMode characterView { get; private set; }
}