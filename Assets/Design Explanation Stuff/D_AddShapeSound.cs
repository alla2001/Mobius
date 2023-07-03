using UnityEngine; 

public class D_AddShapeSound : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //EDITOR REFERENCES

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES



    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        ShapeAudioManager.instance.addEventEmitterToShape(this.gameObject); 
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
}