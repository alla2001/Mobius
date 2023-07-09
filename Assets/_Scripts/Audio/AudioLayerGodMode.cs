using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine; 

public class AudioLayerGodMode : AudioLayer
{
    //PUBLIC STATICS & EVENTS
    private bool isPlaying;

    //REFERENCES

    //OTHER EDITOR VARIABLES 
    [field: SerializeField]
    [Range(0, 32)]
    private int averageBeatsBetweenSounds;
    [field: SerializeField]
    [Range(0, 100)]
    private int skipSoundChance;

    public float minAttenuationRange = 5;
    public float maxAttenuationRange = 30; 

    //CODE VARIABLES
    private int currentAudioNumber = 0;
    private StudioEventEmitter currentEventEmitter => eventEmitters[currentAudioNumber];

    public static int[] lastSound = new int[2];
    private float probabiltyOfSoundOnFirstBeat => 2.0f / (averageBeatsBetweenSounds + 3); 

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        AudioMode.godMode.audioLayers.Add(this.layerType, this); 
    }

    public override void BeatUpdate(int bar, int beat)
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
    public override void Play()
    {
        base.Play();
        /*
        foreach (StudioEventEmitter layerEventInstance in eventEmitters)
        {
            layerEventInstance.Play();
        }
        */
        RhythmManager.instance.beatEvent += BeatUpdate;
    }

    [ContextMenu("TurnOff")]
    public override void Stop()
    {
        base.Stop(); 
        foreach (StudioEventEmitter layerEventInstance in eventEmitters)
        {
            layerEventInstance.AllowFadeout = true;
            layerEventInstance.Stop();
        }
        RhythmManager.instance.beatEvent -= BeatUpdate;
    }

    public override bool AddEventEmitter(StudioEventEmitter eventEmitter, object args)
    {
        return base.AddEventEmitter(eventEmitter, args);
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
        isPlaying = true; 
    }
}