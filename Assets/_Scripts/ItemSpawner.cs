using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> shapes = new List<GameObject>();
    public GameObject itemPrefab;
    int spawnedItems;
    public static ItemSpawner instace;
    private void Awake()
    {
        if (!instace) instace = this;
        else Destroy(this);

        SpawnItems();


    }
    public void SpawnItems()
    {
        print("Trying Spawn");
        for (int i = 0; i < 100; i++)
        {
            foreach (var shape in shapes)
            {
                SpawnInShape(shape);
            }
            if (spawnedItems > 0) break;
        }
      
    }


    public void SpawnInShape(GameObject shape)
    {
       
        List<SplineComputer> computers = new List<SplineComputer>();
        computers.AddRange( shape.GetComponentsInChildren<SplineComputer>());
        foreach (CharacterMovement charMove in CharacterMovement.characters)
        {
            if (charMove.GetComponent<SplineFollower>().spline.transform.parent.gameObject == shape)
            {
                return;
            }
        }
        print("Found Valid Spline");
        foreach (var splineComputer in computers)
        {
            int chance = Random.Range(0, 5);
           
            if (chance == 0)
            {
                SplinePoint point = splineComputer.GetPoint(Random.Range(0, splineComputer.pointCount));
                Instantiate(itemPrefab, point.position + point.normal , Quaternion.identity);
                spawnedItems++;
            }
        }

    }
}
