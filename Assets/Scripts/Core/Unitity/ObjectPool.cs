using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> where T:new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionGet;
    private readonly UnityAction<T> m_ActionRelease;

    public int countAll { get; private set; }
    public int countActive
    {
        get { return countAll - countInActive; }
    }

    public int countInActive
    {
        get { return m_Stack.Count; }
    }

    public ObjectPool()
    {

    }

    public ObjectPool(UnityAction<T> actionGet, UnityAction<T> actionRelease)
    {
        m_ActionGet = actionGet;
        m_ActionRelease = actionRelease;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count <= 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        m_ActionGet?.Invoke(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(),element))
        {
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        }
        m_ActionRelease?.Invoke(element);
        m_Stack.Push(element);
    }
}
