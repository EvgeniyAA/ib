using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T Minimum<T>(this List<T> list, Func<T, float> selector)
    {
        T min = list[0];
        foreach (T item in list)
        {
            float value = selector(item);
            if (selector(item) < selector(min))
            {
                min = item;
            }
        }
        return min;
    }
}

