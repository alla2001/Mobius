using System.Collections.Generic;
using UnityEngine; 

public class ShapeDatabase : MonoBehaviour
{
    //PUBLIC STATICS & EVENTS & PUBLIC SUBCLASSES
    public static ShapeDatabase instance;
    public enum SPLINEFORM
    {
        BUTTERFLY, CHAIR, FROG, MOEBIUS, PRETZEL, ROUNDABOUT, THROUGHTHELOOP
    }

    public enum ARCHITECTURE
    {
        PILLARS_1, PIPES_2, WINDOWS_3, SKELETON_4
    }

    [System.Serializable]
    public class ArchitectureMaterialMap
    {
        [SerializeField] public ARCHITECTURE architecture;
        [SerializeField] public MaterialGroupArray[] materialSlots; 
    }

    [System.Serializable]
    public class MaterialGroupArray
    {
        [SerializeField] public MaterialGroup[] materialGroups; 
    }

    //EDITOR REFERENCES
    public List<SPLINEFORM> lockedShapes = new List<SPLINEFORM>();
    public List<ARCHITECTURE> lockedArchitectures = new List<ARCHITECTURE>();

    public List<ArchitectureMaterialMap> ArchitectureMaterialsMap = new List<ArchitectureMaterialMap>();
    public List<MaterialGroup> MaterialGroups; //make this a ScriptableObject?

    //CODE REFERENCES
    [HideInInspector]
    public List<SPLINEFORM> unlockedShapes = new List<SPLINEFORM>();
    [HideInInspector]
    public List<ARCHITECTURE> unlockedArchitectures = new List<ARCHITECTURE>();

    //EDITOR VARIABLES

    //CODE VARIABLES

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(this.gameObject);
            Debug.LogWarning("deleted second ShapeRandomiser on object: " + gameObject.name); 
        }
        instance = this; 
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public void AddRandomMaterial()
    {

    }

    //PRIVATE CODE METHODS
}