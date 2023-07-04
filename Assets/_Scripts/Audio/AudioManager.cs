using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class AudioManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    public static AudioManager instance;

    //EDITOR REFERENCES
    public Dictionary<AudioLayerManager.LayerType, AudioLayerManager> audioLayers = new Dictionary<AudioLayerManager.LayerType, AudioLayerManager>(); 

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES

    //zypernKAtze SoundModes

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
    private SoundMode currentSoundMode; 
    private List<SoundQueueElement> soundQueueElements; 
    private List<StudioEventEmitter> eventEmitters;

    private Bus masterBus;
    private Bus ambienceBus;
    private Bus melodyBus;
    private Bus sfxBus;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake() //runs before other awakes
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

    public void ChangeSoundMode(SoundMode soundMode)
    {
        if (!soundMode.startingEvent.IsNull)
        {
            EventInstance eI = CreateEventInstance(soundMode.startingEvent);
            eI.start();
            //QueueSound(eI, false, soundMode.fmod.deltaBarsToStart, 1); zypernKatze this seems like a repetition of the starting Sound
        }

        foreach (AudioLayerManager audioLayer in currentSoundMode.audioLayers)
        {
            audioLayer.turnOff(); 
        }

        currentSoundMode = soundMode;

        foreach (AudioLayerManager audioLayer in currentSoundMode.audioLayers)
        {
            audioLayer.turnOn();
        }
    }

    public void AddEmitterToLayer(AudioLayerManager.LayerType layer, GameObject attachmentObject)
    {
        audioLayers[layer].addEventEmitter(attachmentObject); 
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
