using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ArchitecturalStyle")]
public class ArchitecturalStyle : ScriptableObject
{
    /* zypernKatze simplified this
    [System.Serializable]
    public class MaterialGroupArray
    {
        [SerializeField] public MaterialGroup[] materialGroups;

        public MaterialGroup getRandomUnlockedMaterialGroup() //this should not be needed
        {
            MaterialGroup[] unlockedMaterialGroups = materialGroups.Where(materialGroup => ShapeDatabase.instance.unlockedMaterialGroups.Contains(materialGroup)).ToArray(); 
            if (unlockedMaterialGroups.Length == 0)
            {
                Debug.LogWarning("trying to create an architecture, where no materialgroup is unlocked");
                return materialGroups[0]; 
            }
            MaterialGroup materialGroup = unlockedMaterialGroups.GetRandomElement();
            return materialGroup; 
        }
    }

    public MaterialGroupArray[] deprecteadMaterialSlots; //zypernKatze simplifying this to only use MaterialGroups, but no MaterialGroupArrays

    public Material[] deprecatedGetRandomMaterialCombo() //zypernKatze replacing this
    {
        Material[] materialCombo = new Material[deprecteadMaterialSlots.Length]; 
        for (int i = 0; i < materialCombo.Length; i++)
        {
            MaterialGroup matgroup = deprecteadMaterialSlots[i].getRandomUnlockedMaterialGroup(); //zypernKatze remove material 
            materialCombo[i] = matgroup.getRandomUnlockedMaterial(); 
        }
        return materialCombo; 
    }
    */

    public MaterialGroup[] materialSlots; 

    public Material[] getRandomUnlockedMaterialCombo()
    {
        Material[] materialCombo = new Material[materialSlots.Length]; 
        for (int i = 0; i < materialSlots.Length; i++)
        {
            materialCombo[i] = materialSlots[i].getRandomUnlockedMaterial();
        }
        return materialCombo; 
    }
}