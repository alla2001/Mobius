using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine; 

public class ShapeDatabase : MonoBehaviour
{
    //DEBUG
    [ContextMenu("GenerateRandomStartingSetup")]
    public void GenerateRandomStartingSetup()
    {
        Awake();
 
        UnlockForStartingSetup();
        
        //Instantiating the starting Positions
        GameObject[] startingObjects = new GameObject[3];  
        for(int i = 0; i < 3; i++)
        {
            GameObject gO = GenerateRandomShape();
            gO = Instantiate(gO, Vector3.zero, Quaternion.identity, transform.parent);
            Undo.RegisterCreatedObjectUndo(gO, "Created new Shape");
            startingObjects[i] = gO;
        }

        //Randomising the position of the three starting shapes
        foreach(GameObject gO in startingObjects)
        {
            gO.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
            gO.transform.eulerAngles = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
        }
        //zypernKatze need to check whether shapes are overlapping

        //Setting Up Character
        GameObject character = Instantiate(characterPrefab, transform.parent);
        SplineComputer[] splines = FindObjectsByType<SplineComputer>(FindObjectsSortMode.None);
        characterPrefab.GetComponent<SplineFollower>().spline = splines.GetRandomElement();
        Undo.RegisterCreatedObjectUndo(character, "Created Character");

        //Spawn Random Item
        List<GameObject> itemsCreated = SpawnItemOnShape(splines.GetRandomElement().gameObject); //zypernKatze this should be replaced with a proper spawning method
        foreach(GameObject gO in itemsCreated)
        {
            Undo.RegisterCreatedObjectUndo(gO, "item spawned");
            gO.transform.parent = transform.parent; 
        }

        Undo.SetCurrentGroupName("CreatedStartingSetup"); 
    }
    

    //zypernKatze This method is copied from itemspawner in order to avoid mergeconflicts
    public List<GameObject> SpawnItemOnShape(GameObject shape)
    {
        List<GameObject> createdItems = new List<GameObject>();

        List<SplineComputer> computers = new List<SplineComputer>();
        computers.AddRange(shape.GetComponentsInChildren<SplineComputer>());

        foreach (var item in computers)
        {
            int chance = Random.Range(0, 5);
            if (chance == 0)
            {
                SplinePoint point = item.GetPoint(Random.Range(0, item.pointCount));
                GameObject gO = Instantiate(itemPrefab, point.position + point.normal, Quaternion.identity);
                createdItems.Add(gO); 
            }
        }

        return createdItems; 
    }

    //PUBLIC STATICS & EVENTS & PUBLIC SUBCLASSES
    public static ShapeDatabase instance;

    //EDITOR REFERENCES
    public GameObject characterPrefab;
    public GameObject itemPrefab;
    public string prefabFolderPath; 

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
            Debug.LogWarning("deleted second ShapeRandomiser on object: " + gameObject.GetNameIncludingParents()); 
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
    public ShapeGroup UnlockRandomShapeGroup()
    {
        ShapeGroup shapeGroup = lockedShapeGroups.GetRandomElement(); 
        lockedShapeGroups.Remove(shapeGroup);
        Undo.RecordObject(this, ("unlockedShapeGroup: " + shapeGroup.name));

        unlockedShapeGroups.AddAvoidDuplicate(shapeGroup);
        Undo.RecordObject(this, ("unlockedShapeGroup: " + shapeGroup.name));

        return shapeGroup; 
    }

    public ArchitecturalStyle UnlockRandomArchitecture()
    {
        ArchitecturalStyle architecture = lockedArchitectures.GetRandomElement();
        lockedArchitectures.Remove(architecture);
        unlockedArchitectures.AddAvoidDuplicate(architecture);

        Undo.RecordObject(this, ("unlockedArchitecturalStyle: " + architecture.name));
        return architecture; 
    }

    public Material UnlockRandomMaterial()
    {
        Material material = lockedMaterials.GetRandomElement();
        lockedMaterials.Remove(material);
        unlockedMaterials.AddAvoidDuplicate(material);

        Undo.RecordObject(this, ("unlockedRandomMaterial: " + material.name));
        return material; 
    }

    public Material UnlockSpecificMaterial(Material material)
    {
        lockedMaterials.Remove(material); 
        unlockedMaterials.AddAvoidDuplicate(material); 
        Undo.RecordObject(this, ("unlockedMaterial: " + material.name));

        return material; 
    }

    public void UnlockForStartingSetup()
    {
        UnlockRandomShapeGroup();

        UnlockRandomArchitecture();
        UnlockRandomArchitecture();

        UnlockRandomMaterial();
        UnlockRandomMaterial();
        UnlockRandomMaterial();
    }
}