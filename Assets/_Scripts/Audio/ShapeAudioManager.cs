using FMODUnity;
using System.Collections.Generic;
using UnityEngine; 

public class ShapeAudioManager : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS
    public static ShapeAudioManager instance;
    private static bool isPlaying;

    //EDITOR REFERENCES
    [field: SerializeField]
    [Range(0, 100)]
    private int soundDensity;
    [field: SerializeField]
    [Range(0, 100)]
    private int skipShapeSoundChance; 

    //CODE REFERENCES
    private List<StudioEventEmitter> shapeEventEmitters = new List<StudioEventEmitter>();
    private int currentShapeNumber = 0;
    private StudioEventEmitter currentShapeEventEmitter => shapeEventEmitters[currentShapeNumber];

    public static int[] lastSound = new int[2];

    //EDITOR VARIABLES

    //CODE VARIABLES



    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            //Debug.Log("Destroyed secondary AudioManager on: " + gameObject.name);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        RhythmManager.instance.beatEvent += beatUpdate;
    }

    private void beatUpdate(int bar, int beat)
    {
        if (isPlaying) //zypernKAtze not sure whether the gameSound will be crazy without this condition
        {
            if (!currentShapeEventEmitter.IsPlaying())
            {
                isPlaying = false;
                lastSound = new int[] {bar, beat };
                currentShapeNumber = (currentShapeNumber + 1) % shapeEventEmitters.Count;
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
    public StudioEventEmitter addEventEmitterToShape(GameObject shape)
    {
        int randomInt = (int)Random.Range(0, FMODEvents.instance.shapeSounds.Length);
        EventReference randomSound = FMODEvents.instance.shapeSounds[randomInt];

        StudioEventEmitter eventEmitter = AudioManager.instance.CreateEventEmitter(randomSound, shape); 
        eventEmitter.OverrideAttenuation = true;
        eventEmitter.OverrideMinDistance = 5;
        eventEmitter.OverrideMaxDistance = 20;

        shapeEventEmitters.Add(eventEmitter);
        return eventEmitter; 
    }

    //PRIVATE CODE METHODS
    private int amountRandomFloats; //zypernKatze Debug
    private int randomFloatSum; //zypernKatze Debug
    private bool CheckPlayChance(int bar, int beat) //zypernKatze balancing
    {
        int randomInt = Random.Range(0, 100);

        amountRandomFloats++;
        randomFloatSum += randomInt;
        Debug.Log("randomInt: " + randomInt + "\n randomIntAverage: " + randomFloatSum/amountRandomFloats); 

        if (RhythmManager.instance.differenceBetweenBeats(lastSound[0], lastSound[1], bar, beat) * soundDensity > randomInt)
        {
            return true; 
        }
        else if(Random.Range(0, 100) < skipShapeSoundChance)
        {
            Debug.Log("Shape Skipped " + currentShapeEventEmitter);
            currentShapeNumber = (currentShapeNumber + 1)%shapeEventEmitters.Count; 
            return false; 
        }
        return false; 
    }

    private void PlayCurrentShapeSound()
    {
        currentShapeEventEmitter.Play();
        //Debug.Log("now playing: " + currentShapeEventEmitter); 
        isPlaying = true; 
    }
}