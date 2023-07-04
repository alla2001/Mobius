using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MaterialGroup")]
public class MaterialGroup : ScriptableObject
{
    public List<Material> materials; 

    public Material getRandomMaterial()
    {
        return materials[Random.Range(0, materials.Count)]; 
    }
}