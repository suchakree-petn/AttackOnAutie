using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class IEnumerableExtension
{
    public static T GetRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0) return default;

        return list[Random.Range(0, list.Count)];
    }

    public static List<T> Replace<T>(this List<T> list, T oldElement, T newElement)
    {
        list[list.IndexOf(oldElement)] = newElement;
        return list;
    }



    public static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
