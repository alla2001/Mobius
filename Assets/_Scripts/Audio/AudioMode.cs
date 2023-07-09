using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioMode : MonoBehaviour //must be Monobehaviour because it can have a startingEvent
{
    public static AudioMode godMode;
    public static AudioMode characterMode;

    public bool isGodMode;
    public bool isCharacterMode;
    public bool isStartingSoundMode = false; 

    [field: SerializeField] public EventReference startingEvent { get; private set; }
    [field: SerializeField] public int deltaBarsToLayerStart { get; private set; }
    
    public Dictionary<AudioLayerType, AudioLayer> audioLayers = new Dictionary<AudioLayerType, AudioLayer>();

    private void Awake()
    {
        if (isGodMode)
        {
            godMode = this; 
        }
        if (isCharacterMode)
        {
            characterMode = this; 
        }
        if (isStartingSoundMode)
        {
            Play();
            AudioManager.instance.soundModeActive = this; 
        }
        else
        {
            Stop();
        }
    }

    public void AddEmitter(AudioLayerType audioLayerType, StudioEventEmitter eventEmitter, object args)
    {
        if (audioLayers[audioLayerType] == null)
        { 
            Debug.LogError("trying to add eventEmitter to non-existent AudioLayer");
            throw new System.Exception();  
        }
        audioLayers[audioLayerType].AddEventEmitter(eventEmitter, args);
    }

    public void RemoveEmitter(AudioLayerType audioLayerType, StudioEventEmitter eventEmitter)
    {
        if (audioLayers[audioLayerType] == null)
        {
            Debug.LogError("trying to add eventEmitter to non-existent AudioLayer");
            throw new System.Exception();
        }
        audioLayers[audioLayerType].RemoveEventEmitter(eventEmitter);
    }

    public void Play()
    {
        if (!startingEvent.IsNull)
        {
            AudioManager.instance.PlayOneShot(startingEvent);
        }
        if (c_StartSoundMode != null) { StopCoroutine(c_StartSoundMode); }
        c_StartSoundMode = StartCoroutine(C_StartSoundMode());
    }

    Coroutine c_StartSoundMode;
    private IEnumerator C_StartSoundMode()
    {
        yield return new WaitForSeconds(deltaBarsToLayerStart * RhythmManager.instance.BPM);
        foreach (AudioLayer audioLayer in audioLayers.Values)
        {
            audioLayer.Play();
        }
    }

    public void Stop()
    {
        foreach (AudioLayer audioLayer in audioLayers.Values)
        {
            audioLayer.Stop();
        }
    }
}