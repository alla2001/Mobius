using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    public List<Shape> shapes = new List<Shape>();
    public GameObject itemPrefab;
    int spawnedItems;
    public static ItemSpawner instace;
    private void Awake()
    {
        if (!instace) instace = this;
        else Destroy(this);
    }
    private void Start()
    {
        shapes = FindObjectsByType<Shape>(FindObjectsSortMode.None).ToList();
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
        spawnedItems = 0;
    }


    public void SpawnInShape(Shape shape)
    {
       
        List<SplineComputer> computers = new List<SplineComputer>();
        computers.AddRange( shape.GetComponentsInChildren<SplineComputer>());
        foreach (CharacterMovement charMove in CharacterMovement.characters)
        {
           
            if (computers.Contains( charMove.GetComponent<SplineFollower>().spline))
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
                GameObject  temp = Instantiate(itemPrefab, point.position + point.normal*0.5f , Quaternion.identity);
                temp.transform.up = point.normal;
                spawnedItems++;
            }
        }

    }
}
