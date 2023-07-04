using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FMODUnity; 

public class RhythmManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    public static RhythmManager instance;
    public float BPM;
    public delegate void beatDelegate(int bar, int beat);
    public event beatDelegate beatEvent; 

    //EDITOR REFERENCES
    [SerializeField]
    FMODUnity.EventReference beatReference; 

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES
    public int currentBeat = 1;
    public int currentBar = 1;

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("Secondary RhythmManager Destroyed, name: " + gameObject.name + ", from scene: " + gameObject.scene);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        InvokeRepeating("timedUpdate", 0, 60/BPM);
        Invoke("printBarAndBeat", 60);
        beatEvent += StartAmbience;
    }

    private void timedUpdate()
    {
        currentBeat = currentBeat + 1;
        if (currentBeat > 4)
        {
            currentBeat = currentBeat - 4;
            currentBar++;
        }

        if (beatEvent != null)
        {
            beatEvent.Invoke(currentBar, currentBeat);
        }

        RuntimeManager.StudioSystem.setParameterByName("currentBeat", currentBeat);
        RuntimeManager.StudioSystem.setParameterByName("IsBeat", 1);

        StartCoroutine(stopBeat());
    }

    IEnumerator stopBeat()
    {
        yield return null;
        //RuntimeManager.StudioSystem.setParameterByName("IsBeat", 0);
    }

    private void OnDestroy()
    {

    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public int differenceBetweenBeats(int earlyBar, int earlyBeat, int lateBar, int lateBeat)
    {
        return (lateBar - earlyBar) * 4 + lateBeat - earlyBeat; 
    }

    //PRIVATE CODE METHODS
    private void printBarAndBeat()
    {
        Debug.Log("Bar: " + currentBar + "; Beat: " + currentBeat); 
    }

    private void StartAmbience(int bar, int beat)
    {
        if (beat == 1)
        {
            AudioManager.instance.PlayAmbience(FMODEvents.instance.ambience);
            beatEvent -= StartAmbience;
        }
    }

    private void PrintBeat(int bar, int beat)
    {
        Debug.Log("Bar: " + bar + "; Beat: " + beat); 
    }
}
