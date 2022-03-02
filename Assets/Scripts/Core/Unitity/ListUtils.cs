using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListUtils
{
    public static void QuickSort<T>(this List<T> array, Comparison<T> comparison)
    {
        QuickSortIndex(array, 0, array.Count - 1,comparison);
    }

    public static void QuickSortIndex<T>(this List<T> array, int startIndex, int endIndex, Comparison<T> comparison)
    {
        if (startIndex >= endIndex)
        {
            return;
        }

        var pivotIndex = QuickSortOnce(array, startIndex, endIndex, comparison);
        QuickSortOnce(array, startIndex, pivotIndex + 1, comparison);
        QuickSortOnce(array, pivotIndex + 1, endIndex, comparison);
    }

    private static int QuickSortOnce<T>(this List<T> array, int startIndex, int endIndex, Comparison<T> comparison)
    {
        while (startIndex < endIndex)
        {
            var num = array[startIndex];
            if (comparison(num, array[startIndex + 1]) > 0)
            {
                array[startIndex] = array[startIndex + 1];
                array[startIndex + 1] = num;
                startIndex++;
            }
            else
            {
                var temp = array[endIndex];
                array[endIndex] = array[startIndex + 1];
                array[startIndex + 1] = temp;
                endIndex--;
            }
        }

        return startIndex;
    }
}
