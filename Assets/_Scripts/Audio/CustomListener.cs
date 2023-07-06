using UnityEngine;
using FMODUnity; 

public class CustomListener : MonoBehaviour //This is pretty useless
{
    //PUBLIC STATICS & EVENTS

    //REFERENCES
    [SerializeField]
    private GameObject player, cam;

    [SerializeField]
    private int listener; 

    //VARIABLES
    FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();


    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Update()
    {
        attributes.position = RuntimeUtils.ToFMODVector(player.transform.position);
        attributes.forward = RuntimeUtils.ToFMODVector(cam.transform.forward);
        attributes.up = RuntimeUtils.ToFMODVector(cam.transform.forward);

        RuntimeManager.StudioSystem.setListenerAttributes(listener, attributes); 
    }


    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
}