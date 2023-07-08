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

    public static bool InsertAvoidDuplicate<T>(this List<T> list, T toAdd, int index)
    {
        if (!list.Contains(toAdd))
        {
            list.Insert(0, toAdd);
            return true;
        }
        return false;
    }

    public static T GetRandomElement<T>(this HashSet<T> set)
    {

        return set.ElementAt(Random.Range(0, set.Count));
    }

    public static string GetNameIncludingParents(this GameObject gameObject)
    {
        List<string> names = new List<string>();
        Transform transform = gameObject.transform; 
        names.Add(gameObject.name);
        while ((transform = transform.parent) != null) 
        { 
            names.Add(transform.name);
        }
        string fullName = gameObject.scene.name;
        foreach (string name in names)
        {
            fullName += "->" + name;
        }
        return fullName;
    }

    public static Vector3 ClampVector(this Vector3 vector3, Vector3 lowerClamp, Vector3 higherClamp)
    {
        return new Vector3
        (
            Mathf.Clamp(vector3.x, lowerClamp.x, higherClamp.x),
            Mathf.Clamp(vector3.y, lowerClamp.y, higherClamp.y),
            Mathf.Clamp(vector3.z, lowerClamp.z, higherClamp.z)
        ); 
    }

    public static Vector3 ClampVectorComponentWise(this Vector3 vector3, float lowerClamp, float higherClamp)
    {
        return new Vector3
        (
            Mathf.Clamp(vector3.x, lowerClamp, higherClamp),
            Mathf.Clamp(vector3.y, lowerClamp, higherClamp),
            Mathf.Clamp(vector3.z, lowerClamp, higherClamp)
        );
    }

    public static Vector3 ClampVectorMagnitude(this Vector3 vector3, float minMagnitude, float maxMagnitude)
    {
        float magnitude = vector3.magnitude;
        float multiplyValue; 
        if (magnitude > maxMagnitude)
        {
            multiplyValue = maxMagnitude/magnitude;
        }
        else if (magnitude < minMagnitude) 
        { 
            multiplyValue= minMagnitude/magnitude;
        }
        else
        {
            multiplyValue = magnitude; 
        }

        vector3 = new Vector3
        (
            vector3.x * multiplyValue,
            vector3.y * multiplyValue,
            vector3.z * multiplyValue
        );
        return vector3; 
    }
}