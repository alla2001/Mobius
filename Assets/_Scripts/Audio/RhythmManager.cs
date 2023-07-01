using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    public float BPM;
    public static RhythmManager instance;

    //EDITOR REFERENCES

    //CODE REFERENCES

    //EDITOR VARIABLES
    [System.Serializable]
    public class LoopingSound
    {
        [SerializeField]
        private int lengthInBeats; 
        [SerializeField]
        private AudioClip[] audioClips; 
    }

    //CODE VARIABLES



    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS


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
        InvokeRepeating("timedUpdate", 0, BPM/60); 
    }

    private void timedUpdate()
    {

    }
}
