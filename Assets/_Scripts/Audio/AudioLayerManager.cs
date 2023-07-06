using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine; 

public class AudioLayerManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    private bool isPlaying;
    private bool canOverlap; 

    //REFERENCES

    //OTHER EDITOR VARIABLES
    [System.Serializable]
    public enum AudioLayerType
    {
        HANGDRUM, RHYTHMDRUMS, REWARDCOLLECTED, CHARACTER, ITEM
    }
    [field: SerializeField]
    private AudioLayerType soundType; 

    [field: SerializeField]
    [Range(0, 32)]
    private int averageBeatsBetweenSounds;
    [field: SerializeField]
    [Range(0, 100)]
    private int skipSoundChance;

    [field: SerializeField]
    public List<EventReference> events { get; private set; }

    public float minAttenuationRange = 5;
    public float maxAttenuationRange = 30; 

    //CODE VARIABLES
    private List<StudioEventEmitter> eventEmitters = new List<StudioEventEmitter>();
    private int currentAudioNumber = 0;
    private StudioEventEmitter currentEventEmitter => eventEmitters[currentAudioNumber];

    public static int[] lastSound = new int[2];
    private float probabiltyOfSoundOnFirstBeat => 2.0f / (averageBeatsBetweenSounds + 3); 

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (AudioManager.instance.audioLayers.ContainsKey(soundType)) //if AudioManager.instance is null at this point, than the ScriptExecution-order was not git-pushed correctly (AudioManager.Awake should run before the LayerManager.Awake)
        {
            Destroy(this);
            Debug.Log("Destroyed secondary AudioLayerManager on: " + gameObject.GetNameIncludingParents());
        }
        else
        {
            AudioManager.instance.audioLayers.Add(soundType, this);
        }
    }

    private void OnEnable()
    {
        SubscribeBeatUpdate();
        GameManager.Instance.onStateChange.AddListener(ReactGameStateChange); 
    }

    private void OnDisable()
    {
        UnsubscribeBeatUpdate();
        GameManager.Instance.onStateChange.RemoveListener(ReactGameStateChange);
    }

    private void beatUpdate(int bar, int beat)
    {
        if (eventEmitters.Count == 0)
        {
            return; 
        }

        if (isPlaying) 
        {
            if (!currentEventEmitter.IsPlaying())
            {
                isPlaying = false;
                lastSound = new int[] {bar, beat };
                currentAudioNumber = (currentAudioNumber + 1) % eventEmitters.Count;
            }
        }
        else
        {
            if(CheckPlayChance(bar, beat))
            {
                PlayCurrentShapeSound(); 
            }
        }
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    [ContextMenu("TurnOn")]
    public void Play()
    {
        foreach (StudioEventEmitter layerEventInstance in eventEmitters)
        {
            layerEventInstance.Play();
        }
    }

    [ContextMenu("TurnOff")]
    public void Stop()
    {
        foreach (StudioEventEmitter layerEventInstance in eventEmitters)
        {
            layerEventInstance.AllowFadeout = true;
            layerEventInstance.Stop();
        }
        /* zypernKatze_? Do I need non-emitter-sounds?
        foreach (EventReference eR in soundMode.fmod.continuousSounds)
        {
            EventInstance eI = CreateEventInstance(eR);
            QueueSound(eI, true, RhythmManager.instance.currentBar + soundMode.fmod.deltaBarsToStart, 1);
            modeEventInstances.Add(eI);
        }
        */
    }

    public void switchMode(bool canOverlap, float minAttenuationRange, float maxAttenuationRange)
    {
        this.canOverlap = canOverlap;
        this.minAttenuationRange = minAttenuationRange;
        this.maxAttenuationRange = maxAttenuationRange; 
    }

    public StudioEventEmitter addEventEmitter(GameObject attachmentObject)
    {
        return addEventEmitter(attachmentObject, minAttenuationRange, maxAttenuationRange); 
    }

    public StudioEventEmitter addEventEmitter(GameObject attachmentObject, float minAttenuationRange, float maxAttenuationRange)
    {
        int randomInt = (int)Random.Range(0, events.Count); //zypernKatze_cleanup rewrite this to use FMODEvents
        EventReference randomSound = events[randomInt];

        StudioEventEmitter eventEmitter = AudioManager.instance.CreateEventEmitter(randomSound, attachmentObject); 
        eventEmitter.OverrideAttenuation = true;
        eventEmitter.OverrideMinDistance = minAttenuationRange;
        eventEmitter.OverrideMaxDistance = maxAttenuationRange;

        eventEmitters.Add(eventEmitter);
        return eventEmitter; 
    }


    //PRIVATE CODE METHODS
    private bool CheckPlayChance(int bar, int beat)
    {
        float soundProbabilityThisFrame = RhythmManager.instance.differenceBetweenBeats(lastSound[0], lastSound[1], bar, beat) * probabiltyOfSoundOnFirstBeat * 100;
        int randomComparison = Random.Range(0, 100); 
        if (soundProbabilityThisFrame > randomComparison)
            //This calculation linearily increases the chance of a sound playing the longer no sound did play (it involves a pity-timer so that there never is a too long time without a sound)
            //also the formula is approximated in  it's calculation by dropping a term linear linear in n that is divided by a term quadratic over n (n = the beats until the next sound is guaranteed to play)
        {
            return true; 
        }
        else if(Random.Range(0, 100) < skipSoundChance)
        {
            Debug.Log("Shape Skipped " + currentEventEmitter);
            currentAudioNumber = (currentAudioNumber + 1)%eventEmitters.Count; 
            return false; 
        }
        return false; 
    }

    private void PlayCurrentShapeSound()
    {
        currentEventEmitter.Play();
        //Debug.Log("now playing: " + currentShapeEventEmitter); 
        isPlaying = true; 
    }


    private void ReactGameStateChange(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GodView:
                SubscribeBeatUpdate();
                Stop();
                break;
            case GameState.CharacterView:
                UnsubscribeBeatUpdate();
                Play();
                break;
            case GameState.ShapePlacement:

                break;
            case GameState.CharacterPlacement:

                break;
            case GameState.RewardMode:

                break;
        }
    }

    private void SubscribeBeatUpdate()
    {

        RhythmManager.instance.beatEvent += beatUpdate;
    }

    private void UnsubscribeBeatUpdate()
    {
        RhythmManager.instance.beatEvent -= beatUpdate;
    }
}