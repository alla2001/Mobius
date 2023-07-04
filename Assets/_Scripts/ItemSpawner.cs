using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> shapes = new List<GameObject>();
    public GameObject itemPrefab;
    int spawnedItems;

    private void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            SpawnItems();
            if (spawnedItems > 0) break;
        }
          
        
    }
    public void SpawnItems()
    {
        foreach (var shape in shapes)
        {
            SpawnInShape(shape);
        }
    }


    public void SpawnInShape(GameObject shape)
    {
        
        List<SplineComputer> computers = new List<SplineComputer>();
        computers.AddRange( shape.GetComponentsInChildren<SplineComputer>());

        foreach (var item in computers)
        {
            int chance = Random.Range(0, 5);
            if (chance == 0)
            {
                SplinePoint point = item.GetPoint(Random.Range(0, item.pointCount));
                Instantiate(itemPrefab, point.position + point.normal , Quaternion.identity);
                spawnedItems++;
            }
        }

    }
}
