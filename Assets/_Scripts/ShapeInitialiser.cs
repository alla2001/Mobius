using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine; 

public class ShapeInitialiser : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS

    //EDITOR REFERENCES

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES



    //PUBLIC STATIC METHODS
    private void Start()
    {
        Material[] mats = GetComponent<MeshRenderer>().sharedMaterials; 
        foreach(Material mat in mats)
        {

        }
    }

    //MONOBEHAVIOUR METHODS

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS

    //PRIVATE CODE METHODS
}