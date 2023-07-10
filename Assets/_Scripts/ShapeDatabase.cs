using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShapeDatabase : MonoBehaviour
{
    //DEBUG
    [ContextMenu("NextRandomStartingSetup")]
    public void NextRandomStartingSetup()
    {
        startingPositionNumber++; 
        RegenerateRandomStartingSetup(); 
    }

    [ContextMenu("RegnerateRandomStartingSetup")]
    public void RegenerateRandomStartingSetup()
    {
        //Creating a copy of this object from which I can create the next starting position
        GameObject newManager = Instantiate(managerPrefab);
        newManager.name = "Manager" + (startingPositionNumber + 1);
        this.name = "startingPosition" + startingPositionNumber; 

        Awake();
 
        UnlockForStartingSetup();
        
        //Instantiating the starting Positions
        GameObject[] startingObjects = new GameObject[3];  
        for(int i = 0; i < 3; i++)
        {
            GameObject gO = GenerateRandomShape();
            gO = Instantiate(gO, Vector3.zero, Quaternion.identity, this.transform);
            //Undo.RegisterCreatedObjectUndo(gO, "Created new Shape");
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
        GameObject character = Instantiate(characterPrefab, this.transform);
        //Undo.RegisterCreatedObjectUndo(character, "Created Character");

        /* I have to do this manually
        SplineComputer[] splines = FindObjectsByType<SplineComputer>(FindObjectsSortMode.None);
        characterPrefab.GetComponent<SplineFollower>().spline = splines.GetRandomElement();
        character.transform.parent = fullPosition.transform; 
        */
        //Undo.SetCurrentGroupName("CreatedStartingSetup"); 
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

    //REFERENCES
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject managerPrefab;

    public List<ShapeGroup> lockedShapeGroups = new List<ShapeGroup>();
    public List<ArchitecturalStyle> lockedArchitectures = new List<ArchitecturalStyle>();
    public List<Material> lockedMaterials = new List<Material>();

    public List<ShapeGroup> unlockedShapeGroups = new List<ShapeGroup>();
    public List<ArchitecturalStyle> unlockedArchitectures = new List<ArchitecturalStyle>();
    public List<Material> unlockedMaterials = new List<Material>();


    //EDITOR VARIABLES
    public int startingPositionNumber; 
    public int everyXTimesUnlockShape;
    public int everyXTimesUnlockArchitecture;
    public int everyXTimesUnlockMaterial;



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
            Debug.LogWarning("destroying second ShapeDatabase on object: " + gameObject.GetNameIncludingParents() + "\n first ShapeDatabase is on: " + gameObject.GetNameIncludingParents());
            DestroyImmediate(this.gameObject); 
        }
        instance = this; 
    }

    private void Start()
    {
        List<Material> startingMaterials = new List<Material>();
        SplineComputer[] scs = FindObjectsOfType<SplineComputer>(); 
        foreach (SplineComputer sc in scs)
        {
            foreach(Material material in sc.GetComponent<Renderer>().materials)
            {
                startingMaterials.AddAvoidDuplicate(material);
            }
        }
        foreach(Material material in startingMaterials)
        {
            UnlockSpecificMaterial(material);
        }
    }

    private void LockAllMaterials()
    {
        while (unlockedMaterials.Count > 0) 
        {
            LockSpecificMaterial(unlockedMaterials[0]); 
        }
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

    public void CheckUnlock(int shapesPlaced)
    {
        if (shapesPlaced % everyXTimesUnlockShape == 0)
        {
            UnlockRandomShapeGroup();
        }
        if ( shapesPlaced % everyXTimesUnlockArchitecture ==0)
        {
            UnlockRandomArchitecture();
        }
       
        if (shapesPlaced % everyXTimesUnlockMaterial == 0)
        {
            UnlockRandomMaterial();

        }

    }
    public void UnlockObjectFromList<T>(List<T> lockedList, List<T> unlockedList, T toUnlock)
    {
        lockedList.Remove(toUnlock); 
        unlockedList.Add(toUnlock);
    }
    public ShapeGroup UnlockRandomShapeGroup()
    {
        if (lockedShapeGroups.Count == 0) { return null; }
        ShapeGroup shapeGroup = lockedShapeGroups.GetRandomElement(); 
        lockedShapeGroups.Remove(shapeGroup);
        //Undo.RecordObject(this, ("unlockedShapeGroup: " + shapeGroup.name));

        unlockedShapeGroups.Add(shapeGroup);
        //Undo.RecordObject(this, ("unlockedShapeGroup: " + shapeGroup.name));

        return shapeGroup; 
    }

    public ArchitecturalStyle UnlockRandomArchitecture()
    {
        if (lockedArchitectures.Count == 0) { return null; }
        ArchitecturalStyle architecture = lockedArchitectures.GetRandomElement();
        lockedArchitectures.Remove(architecture);
        unlockedArchitectures.Add(architecture);

        //Undo.RecordObject(this, ("unlockedArchitecturalStyle: " + architecture.name));
        return architecture; 
    }

    public Material UnlockRandomMaterial()
    {
        if (lockedMaterials.Count == 0) { return null; }
        Material material = lockedMaterials.GetRandomElement();
        lockedMaterials.Remove(material);
        unlockedMaterials.Add(material);

        //Undo.RecordObject(this, ("unlockedRandomMaterial: " + material.name));
        return material; 
    }

    public Material UnlockSpecificMaterial(Material material)
    {
        lockedMaterials.Remove(material); 
        unlockedMaterials.Add(material); 
        //Undo.RecordObject(this, ("unlockedMaterial: " + material.name));

        return material; 
    }

    public Material LockSpecificMaterial(Material material)
    {
        lockedMaterials.Add(material);
        unlockedMaterials.Remove(material);
        //Undo.RecordObject(this, ("unlockedMaterial: " + material.name));

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