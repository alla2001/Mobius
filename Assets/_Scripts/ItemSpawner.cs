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
    List<GameObject> createdItems =new List<GameObject>();
    private void Awake()
    {
        if (!instace) instace = this;
        else Destroy(this);
    }
    private void Start()
    {
        StartCoroutine(wait());
        SpawnItems();
    }
    IEnumerator wait()
    {
        shapes = FindObjectsByType<Shape>(FindObjectsSortMode.None).ToList();
        yield return new WaitForSeconds(0.2f);
    }

    public void SpawnItems()
    {
        createdItems.Clear();
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

    public void DestroyItems()
    {
        foreach (var item in createdItems)
        {
            Destroy(item);
        }
    }

    public void SpawnInShape(Shape shape)
    {
       
        List<SplineComputer> computers = new List<SplineComputer>();
        computers.AddRange( shape.GetComponentsInChildren<SplineComputer>());
        foreach (CharacterInfo charInfo in CharacterInfo.characters)
        {
            if (charInfo== null|| charInfo.GetComponent<SplineFollower>().spline==null) continue;
            if (computers.Contains( charInfo.GetComponent<SplineFollower>().spline))
            {
                return;
            }
        }
        print("Found Valid Spline");
        foreach (var splineComputer in computers)
        {
            float chanceReciprocal = 5;
            foreach(Item i in FindObjectsOfType<Item>())
            {
                chanceReciprocal += 3; 
            }
            int chance = Random.Range(0, (int) chanceReciprocal);

            if (chance == 0)
            {
                SplinePoint point = splineComputer.GetPoint(Random.Range(0, splineComputer.pointCount));
                GameObject  temp = Instantiate(itemPrefab, point.position + point.normal*0.5f , Quaternion.identity);
                temp.transform.up = point.normal;
                spawnedItems++;
                createdItems.Add(temp);
            }
        }

    }
}
