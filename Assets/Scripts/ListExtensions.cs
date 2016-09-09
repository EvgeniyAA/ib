using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static T Minimum<T>(this IEnumerable<T> list, Func<T, float> selector)
    {
        T min = list.First();
        float minValue = selector(min);
        foreach (T item in list)
        {
            float newValue = selector(item);
            if (newValue < minValue)
            {
                min = item;
                minValue = newValue;
            }
        }
        return min;
    }
}

