using UnityEngine; 

public class CharacterAudio : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //EDITOR REFERENCES

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES



    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TakeControl(); 
        }
    }


    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public void TakeControl()
    {
        FMODUnity.StudioEventEmitter emitter = AudioManager.instance.CreateEventEmitter(FMODEvents.instance.characterControlRequest, this.gameObject);
        emitter.OverrideAttenuation = true;
        emitter.OverrideMinDistance = 5;
        emitter.OverrideMaxDistance = 50; 
        emitter.Play(); 
    }

    //PRIVATE CODE METHODS
}