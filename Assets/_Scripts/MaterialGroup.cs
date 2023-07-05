using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MaterialGroup")]
public class MaterialGroup : ScriptableObject
{
    public Material[] materials;  

    public Material getRandomUnlockedMaterial()
    {
        Material result;

        Material[] unlockedMaterials = materials.Where(material => ShapeDatabase.instance.unlockedMaterials.Contains(material)).ToArray();

        if (unlockedMaterials.Length == 0)
        {
            Debug.LogWarning("Trying to create Material from MaterialGroup with no unlocked Materials");
            unlockedMaterials = materials;
        }
        result = unlockedMaterials.GetRandomElement();

        return result;
    }
}