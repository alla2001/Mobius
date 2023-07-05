using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShapeGroup")]
public class ShapeGroup : ScriptableObject
{
    public List<GameObject> shapes; 

    public GameObject getShapeByArchitecture(ArchitecturalStyle architecturalStyle)
    {
        for (int i = 0; i< shapes.Count; i++)
        {
            if (shapes[i].GetComponent<Shape>().architecturalStyle == architecturalStyle)
            {
                return shapes[i];
            }
        }
        return shapes.Where(shape => shape.GetComponent<Shape>().architecturalStyle == architecturalStyle).First(); 
    }
}