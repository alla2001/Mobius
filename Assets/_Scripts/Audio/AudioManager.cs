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
    //PUBLIC STATICS & EVENTS & private classes
    public static AudioManager instance;
    private class SoundQueueElement
    {
        public SoundQueueElement(EventInstance eI, bool start, int bar, int beat)
        {
            eventInstance = eI;
            this.start = start;
            this.bar = bar;
            this.beat = beat;
        }

        public SoundQueueElement(FMODUnity.StudioEventEmitter eventEmitter, bool start, int bar, int beat)
        {
            this.eventEmitter = eventEmitter;
            this.start = start;
            this.bar = bar;
            this.beat = beat;
        }

        public EventInstance eventInstance;
        public FMODUnity.StudioEventEmitter eventEmitter;
        public bool start;
        public int bar;
        public int beat;
    }

    //REFERENCES

    //PUBLIC VARIABLES
    //public List<SoundMode> soundModesList = new List<SoundMode>() { SoundMode.godMode, SoundMode.characterMode };  
    public AudioMode soundModeActive = AudioMode.godMode;

    //PRIVATE VARIABLES
    private List<EventInstance> eventInstances;
    private List<SoundQueueElement> soundQueueElements; 
    private List<FMODUnity.StudioEventEmitter> eventEmitters;

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
            Debug.Log("Destroyed secondary AudioManager on: " + gameObject.GetNameIncludingParents()); 
        }
        else
        {
            instance = this; 
        }

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<FMODUnity.StudioEventEmitter>();
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
        GameManager.Instance.onStateChange.AddListener(OnGameStateChange); 
    }

    private void OnBeatUpdate(int bar, int beat)
    {
        CheckSoundQueue(bar, beat); 
    }
    private void OnDestroy()
    {
        CleanUp();
        RhythmManager.instance.beatEvent -= OnBeatUpdate;
        GameManager.Instance.onStateChange.RemoveListener(OnGameStateChange);
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

    public void ChangeAudioMode(AudioMode soundMode)
    {
        this.soundModeActive.Stop();
        this.soundModeActive = soundMode;
        this.soundModeActive.Play(); 
    }

    public void AddAudioLayerToSoundMode(AudioMode soundMode, AudioLayer audioLayer)
    {
        soundMode.audioLayers.Add(audioLayer.layerType, audioLayer); 
    }

    public void AddEventEmitter(AudioMode soundMode, AudioLayerType layerType, StudioEventEmitter eventEmitter, object args)
    {
        soundMode.AddEmitter(layerType, eventEmitter, args);  
    }

    public void QueueSound(EventInstance eventInstance, bool start, int bar, int beat)
    {
        soundQueueElements.Add(new SoundQueueElement(eventInstance, start, bar, beat));
    }

    public void QueueSound(StudioEventEmitter eventEmitter, bool start, int bar, int beat)
    {
        soundQueueElements.Add(new SoundQueueElement(eventEmitter, start, bar, beat));
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter AddEventEmitterComponent(EventReference eventReference, GameObject emitterGameObject)
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
        foreach (FMODUnity.StudioEventEmitter emitter in eventEmitters)
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
                    if (soundQueueElements[i] != null)
                    {
                        soundQueueElements[i].eventEmitter.Play(); 
                    }
                    else
                    {
                        soundQueueElements[i].eventInstance.Play();
                    }
                }
                else
                {
                    if (soundQueueElements[i] != null)
                    {
                        soundQueueElements[i].eventEmitter.Stop();
                    }
                    else
                    {
                        soundQueueElements[i].eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    }
                }
                soundQueueElements.RemoveAt(i); 
            }
        }
    }

    private void OnGameStateChange(GameState state)
    {
        switch(state)
        {
            case GameState.GodView:
                ChangeAudioMode(AudioMode.godMode); 
                break;
            case GameState.CharacterView:
                ChangeAudioMode(AudioMode.characterMode);
                break;
            case GameState.ShapePlacement:
                ChangeAudioMode(AudioMode.godMode);
                break;
            case GameState.CharacterPlacement:
                ChangeAudioMode(AudioMode.godMode);
                break;
            case GameState.RewardMode:
                ChangeAudioMode(AudioMode.godMode);
                break;
        }
    }
}
