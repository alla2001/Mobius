using UnityEngine; 

public class D_AddShapeSound : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //REFERENCES

    //OTHER EDITOR VARIABLES
    public AudioLayerManager.LayerType layerType; 

    //CODE VARIABLES



    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Start()
    {
        AudioManager.instance.AddEmitterToLayer(layerType, this.gameObject);  
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
}