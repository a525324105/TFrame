using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListPool<T>
{
    private static readonly ObjectPool<List<T>> m_ListPool = new ObjectPool<List<T>>(null, Clear);

    static void Clear(List<T> list)
    {
        list.Clear();
    }

    public static List<T> Get()
    {
        return m_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        m_ListPool.Release(toRelease);
    }
}
