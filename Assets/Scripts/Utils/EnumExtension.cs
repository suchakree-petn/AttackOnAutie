using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnumExtension
{
    public static T GetRandomEnum<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }

    public static List<T> ToList<T>() where T : Enum
    {

        return new List<T>((T[])System.Enum.GetValues(typeof(T)));
    }
}
