using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionMethods
{
    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)]; 
    }

    public static T GetRandomElement<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static bool AddAvoidDuplicate<T>(this List<T> list, T toAdd)
    {
        if (!list.Contains(toAdd))
        {
            list.Add(toAdd);
            return true; 
        }
        return false; 
    }

    public static T GetRandomElement<T>(this HashSet<T> set)
    {

        return set.ElementAt(Random.Range(0, set.Count));
    }
}