using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//putting in new elements at the end of the list; 
public class AudioLayerCharacterMode : AudioLayer
{
    //REFERENCES

    //PUBLIC VARIABLES
    public List<StudioEventEmitter> toPlayEventEmitters; 
    [SerializeField]
    private float fadeLowDuration = 1.0f;
    [SerializeField]
    private float[] volumeDecreaseArray = new float[0] { }; 

    //PRIVATE VARIABLES
    private float volume = 1.0f;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        AudioMode.characterMode.audioLayers.Add(this.layerType, this);
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public override void Play()
    {
        base.Play(); 
        RhythmManager.instance.beatEvent += BeatUpdate; 
    }

    public override void Stop()
    {
        base.Play(); 
        foreach (StudioEventEmitter eventEmitter in eventEmitters)
        {
            eventEmitter.Stop();
        }
    }

    public override void BeatUpdate(int bar, int beat)
    {
        foreach(StudioEventEmitter eventEmitter in toPlayEventEmitters)
        {
            eventEmitters.Insert(0, eventEmitter); 
            eventEmitter.Play();
        }
        toPlayEventEmitters.Clear(); 
    }
    public override bool AddEventEmitter(StudioEventEmitter eventEmitter, object args)
    {
        if (!isActive)
        {
            return false; 
        }
        //the following if-condition shouldn't happen, but for safety reasons it's implemented
        if (AudioManager.instance.soundModeActive != AudioMode.characterMode)
        {
            throw new System.Exception("trying to add sound to characterMode while not in characterMode"); 
        }
        if (eventEmitters.Contains(eventEmitter))
        {
            eventEmitter.Stop();
            eventEmitters.Remove(eventEmitter); 
        }
        eventEmitter.SetParameter("Volume", 1.0f);
        bool didAdd = toPlayEventEmitters.AddAvoidDuplicate(eventEmitter);
        DecreaseVolumes();
        return didAdd; 
    }

    public void DecreaseVolumes()
    {
        if (eventEmitters.Count >= volumeDecreaseArray.Length+1)
        {
            eventEmitters.RemoveAt(eventEmitters.Count - 1);  
        }
        for (int i = 0; i < eventEmitters.Count; i++)
        {
            eventEmitters[i].Stop(); 
            /*
            if (c_decreaseVolumes[i] != null) { StopCoroutine(c_decreaseVolumes[i]); }
            c_decreaseVolumes[i] = StartCoroutine(C_DecreaseVolume(eventEmitters[i], volumeDecreaseArray[i])); 
            */
        }
    }

    Coroutine[] c_decreaseVolumes = new Coroutine[4]; 
    private IEnumerator C_DecreaseVolume(StudioEventEmitter eventEmitter, float ToVolume)
    {
        float fromVolume = volume; 
        for (float time = 0; time < fadeLowDuration; time += Time.deltaTime)
        {
            volume = Mathf.Lerp(fromVolume, ToVolume, time/fadeLowDuration); 
            eventEmitter.SetParameter("Volume", volume); 
            yield return null; 
        }
        yield return null; 
    }

    //PRIVATE CODE METHODS
}