using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class AudioManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    public static AudioManager instance;

    //EDITOR REFERENCES

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES
    public class SoundMode
    {
        public string name;
        public FMODEvents.SoundMode fmod; 

        private SoundMode(string name, FMODEvents.SoundMode sounds)
        {
            this.name = name;
            this.fmod = sounds; 
        }

        public static SoundMode GodMode = new SoundMode("Godmode", FMODEvents.instance.godMode); 
        public static SoundMode CharacterMode = new SoundMode("Godmode", FMODEvents.instance.characterMode); 
        public static SoundMode RewardMode = new SoundMode("RewardMode", FMODEvents.instance.rewardMode); 
    }

    private class SoundQueueElement
    {
        public SoundQueueElement(EventInstance eI, bool start, int bar, int beat)
        {
            eventInstance = eI; 
            this.start = start;
            this.bar = bar;
            this.beat = beat;
        }

        public EventInstance eventInstance;
        public bool start;
        public int bar;
        public int beat; 
    }

    private List<EventInstance> eventInstances;
    private HashSet<EventInstance> modeEventInstances;
    private List<SoundQueueElement> soundQueueElements; 
    private List<StudioEventEmitter> eventEmitters;

    private Bus masterBus;
    private Bus ambienceBus;
    private Bus melodyBus;
    private Bus sfxBus;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("Destroyed secondary AudioManager on: " + gameObject.name); 
        }
        else
        {
            instance = this; 
        }

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        modeEventInstances = new HashSet<EventInstance>();
        soundQueueElements = new List<SoundQueueElement>(); 

        /*
        masterBus = RuntimeManager.GetBus("bus:/");
        melodyBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        */
    }

    private void Start()
    {
        RhythmManager.instance.beatEvent += OnBeatUpdate;
    }

    private void OnBeatUpdate(int bar, int beat)
    {
        CheckSoundQueue(bar, beat); 
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        CleanUp();
        RhythmManager.instance.beatEvent -= OnBeatUpdate;
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public void PlayAmbience(EventReference ambience)
    {
        RuntimeManager.PlayOneShot(ambience); 
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound); 
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void ChangeMode(SoundMode soundMode)
    {
        foreach (EventInstance modeEventInstance in modeEventInstances)
        {
            modeEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            modeEventInstance.release(); 
        }
        if (!soundMode.fmod.startingEvent.IsNull)
        {
            EventInstance eI = CreateEventInstance(soundMode.fmod.startingEvent); 
            eI.start();
            QueueSound(eI, false, soundMode.fmod.deltaBarsToStart, 1); 
        }
        foreach (EventReference eR in soundMode.fmod.continuousSounds)
        {
            EventInstance eI = CreateEventInstance(eR);
            QueueSound(eI, true, RhythmManager.instance.currentBar + soundMode.fmod.deltaBarsToStart, 1); 
            modeEventInstances.Add(eI);
        }
    }

    public void QueueSound(EventInstance eI, bool start, int bar, int beat)
    {
        soundQueueElements.Add(new SoundQueueElement(eI, start, bar, beat));
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter CreateEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    //PRIVATE CODE METHODS
    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void CheckSoundQueue(int bar, int beat)
    {
        for (int i = soundQueueElements.Count- 1; i >= 0; i--)
        {
            if (soundQueueElements[i].bar == bar && soundQueueElements[i].beat == beat)
            {
                if (soundQueueElements[i].start)
                {
                    soundQueueElements[i].eventInstance.start();
                }
                else
                {
                    soundQueueElements[i].eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                soundQueueElements.RemoveAt(i); 
            }
        }
    }
}
