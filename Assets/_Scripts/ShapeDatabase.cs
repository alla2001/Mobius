using System.Collections.Generic;
using UnityEngine; 

public class ShapeDatabase : MonoBehaviour
{
    //DEBUG
    [ContextMenu("Generate20Shapes")]
    public void Generate20Shapes()
    {
        Awake(); 

        for(int i = 0; i < 20; i++)
        {
            GameObject gO = GenerateRandomShape();
            gO = Instantiate(gO, Vector3.zero, Quaternion.identity);
            gO.transform.position += Vector3.right * i * 10; 
        }
    }

    //PUBLIC STATICS & EVENTS & PUBLIC SUBCLASSES
    public static ShapeDatabase instance;

    //EDITOR REFERENCES
    public List<ShapeGroup> lockedShapeGroups = new List<ShapeGroup>();
    public List<ArchitecturalStyle> lockedArchitectures = new List<ArchitecturalStyle>();
    public List<Material> lockedMaterials = new List<Material>();

    public List<ShapeGroup> unlockedShapeGroups = new List<ShapeGroup>();
    public List<ArchitecturalStyle> unlockedArchitectures = new List<ArchitecturalStyle>();
    public List<Material> unlockedMaterials = new List<Material>();

    //CODE REFERENCES

    //EDITOR VARIABLES

    //CODE VARIABLES

    //PUBLIC STATIC METHODS

    //MONOBEHAVIOUR METHODS
    [ContextMenu("SetThisAsInstance")]
    private void Awake()
    {
        if (instance == this) //needed because I call this in edit mode
        {
            return; 
        }
        if (instance != null) 
        {
            DestroyImmediate(this.gameObject);
            Debug.LogWarning("deleted second ShapeRandomiser on object: " + gameObject.name); 
        }
        instance = this; 
    }

    //IN SCENE METHODS (e.g. things that need to be accessed by unityEvents)

    //PUBLIC CODE METHODS
    public GameObject GenerateRandomShape()
    {
        //ShapeGroup -> Architecture -> MaterialGroup -> Materials
        GameObject toReturn;
        ShapeGroup shapeGroup = unlockedShapeGroups.GetRandomElement();
        ArchitecturalStyle architecture = unlockedArchitectures.GetRandomElement(); 
        toReturn = shapeGroup.getShapeByArchitecture(architecture);
        Material[] materialCombo = toReturn.GetComponent<Shape>().architecturalStyle.getRandomUnlockedMaterialCombo(); //zypernKatze Randomisation / locking&unlocking should be uniformly either here on in the subclasses
        toReturn.GetComponent<Renderer>().materials = materialCombo; 

        //Debug
        return toReturn; 
    }

    public void UnlockObjectFromList<T>(List<T> lockedList, List<T> unlockedList, T toUnlock)
    {
        lockedList.Remove(toUnlock); 
        unlockedList.AddAvoidDuplicate(toUnlock);
    }
    public void UnlockRandomSplineForm()
    {
        ShapeGroup shapeGroup = lockedShapeGroups.GetRandomElement(); 
        lockedShapeGroups.Remove(shapeGroup);
        unlockedShapeGroups.AddAvoidDuplicate(shapeGroup); 
    }

    public void UnlockRandomArchitecture()
    {
        ArchitecturalStyle architecture = lockedArchitectures.GetRandomElement();
        lockedArchitectures.Remove(architecture);
        unlockedArchitectures.AddAvoidDuplicate(architecture);
    }

    public void UnlockRandomMaterial()
    {
        Material material = lockedMaterials.GetRandomElement();
        lockedMaterials.Remove(material);
        unlockedMaterials.AddAvoidDuplicate(material);
    }
}