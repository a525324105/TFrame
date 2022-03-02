using System;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    private int m_eventType = 0;
    public List<Delegate> m_listExist = new List<Delegate>();
    private List<Delegate> m_addList = new List<Delegate>();
    private List<Delegate> m_deleteList = new List<Delegate>();
    private bool m_isExcute = false;
    private bool m_dirty = false;

    public EventData(int evnetType)
    {
        m_eventType = evnetType;
    }

    public void Clear()
    {
        m_eventType = 0;
        m_listExist.Clear();
        m_addList.Clear();
        m_deleteList.Clear();
        m_isExcute = false;
        m_dirty = false;
    }

    public bool AddHandler(Delegate handler)
    {
        if (m_listExist.Contains(handler))
        {
            Debug.LogError("repeated add handler");
            return false;
        }

        if (m_isExcute)
        {
            m_dirty = true;
            m_addList.Add(handler);
        }
        else
        {
            m_listExist.Add(handler);
        }

        return true;
    }

    public void RmvHandler(Delegate hander)
    {
        if (m_isExcute)
        {
            m_dirty = true;
            m_deleteList.Add(hander);
        }
        else
        {
            if (!m_listExist.Remove(hander))
            {
                Debug.LogErrorFormat("delete handle failed, not exist, EvntId: {0}", StringId.HashToString(m_eventType));
            }
        }
    }

    private void CheckModify()
    {
        m_isExcute = false;
        if (m_dirty)
        {
            for (int i = 0; i < m_addList.Count; i++)
            {
                m_listExist.Add(m_addList[i]);
            }

            m_addList.Clear();

            for (int i = 0; i < m_deleteList.Count; i++)
            {
                m_listExist.Remove(m_deleteList[i]);
            }

            m_deleteList.Clear();
        }
    }

    public void Callback()
    {
        m_isExcute = true;
        for (var i = 0; i < m_listExist.Count; i++)
        {
            var d = m_listExist[i];
            Action action = d as Action;
            if (action != null)
            {
                action();
            }
        }

        CheckModify();
    }

    public void Callback<T>(T arg1)
    {
        m_isExcute = true;
        for (var i = 0; i < m_listExist.Count; i++)
        {
            var d = m_listExist[i];
            var action = d as Action<T>;
            if (action != null)
            {
                action(arg1);
            }
        }

        CheckModify();
    }

    public void Callback<T, U>(T arg1, U arg2)
    {
        m_isExcute = true;
        for (var i = 0; i < m_listExist.Count; i++)
        {
            var d = m_listExist[i];
            var action = d as Action<T, U>;
            if (action != null)
            {
                action(arg1, arg2);
            }
        }

        CheckModify();
    }

    public void Callback<T, U, V>(T arg1, U arg2, V arg3)
    {
        m_isExcute = true;
        for (var i = 0; i < m_listExist.Count; i++)
        {
            var d = m_listExist[i];
            var action = d as Action<T, U, V>;
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }

        CheckModify();
    }

    public void Callback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4)
    {
        m_isExcute = true;
        for (var i = 0; i < m_listExist.Count; i++)
        {
            var d = m_listExist[i];
            var action = d as Action<T, U, V, W>;
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
            }
        }

        CheckModify();
    }
}