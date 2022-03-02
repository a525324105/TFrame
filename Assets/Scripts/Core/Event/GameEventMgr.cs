using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总观察者 - 事件中心系统
/// </summary>
public class GameEventMgr:Singleton<GameEventMgr>
{
    private bool m_isInit = false;
    private Dictionary<int, Delegate> m_eventDic = new Dictionary<int, Delegate>();
    private Dictionary<int, EventData> m_eventTable = new Dictionary<int, EventData>();

    public void OnInit()
    {
        if (m_isInit)
        {
            return;
        }

        m_isInit = true;
    }

    public void Destroy()
    {
        if (!m_isInit)
        {
            return;
        }

        var element = m_eventDic.GetEnumerator();
        while (element.MoveNext())
        {
            var m_event = element.Current.Value;
            RemoveListener(element.Current.Key, m_event);
        }
        m_eventDic.Clear();
        m_eventTable.Clear();
    }


    #region 接口
    #endregion
    private void AddEvent(int eventType, Delegate handler)
    {
        m_eventDic.Add(eventType,handler);
    }

    public void AddUIEvent(int eventType, Action handler)
    {
        if (AddListener(eventType, handler))
        {
            AddEvent(eventType, handler);
        }
    }
    public void AddUIEvent<T>(int eventType, Action<T> handler)
    {
        if (AddListener(eventType, handler))
        {
            AddEvent(eventType, handler);
        }
    }

    public void AddUIEvent<T, U>(int eventType, Action<T, U> handler)
    {
        if (AddListener(eventType, handler))
        {
            AddEvent(eventType, handler);
        }
    }

    public void AddUIEvent<T, U, V>(int eventType, Action<T, U, V> handler)
    {
        if (AddListener(eventType, handler))
        {
            AddEvent(eventType, handler);
        }
    }

    public void AddUIEvent<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
    {
        if (AddListener(eventType, handler))
        {
            AddEvent(eventType, handler);
        }
    }

    #region 监听接口
    public bool AddListener(int eventType, Delegate handler)
    {
        EventData data;
        if (!m_eventTable.TryGetValue(eventType, out data))
        {
            data = new EventData(eventType);
            m_eventTable.Add(eventType, data);
        }

        return data.AddHandler(handler);
    }


    public void RemoveListener(int eventId, Delegate handler)
    {
        if (handler == null)
        {
            return;
        }

        EventData data;
        if (m_eventTable.TryGetValue(eventId, out data))
        {
            data.RmvHandler(handler);
        }
    }


    #endregion

    #region 事件分发接口

    public void Send(int eventType)
    {
        EventData d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            d.Callback();
        }
    }

    public void Send<T>(int eventType, T arg1)
    {
        EventData d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            d.Callback(arg1);
        }
    }

    public void Send<T, U>(int eventType, T arg1, U arg2)
    {
        EventData d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            d.Callback(arg1, arg2);
        }
    }

    public void Send<T, U, V>(int eventType, T arg1, U arg2, V arg3)
    {
        EventData d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            d.Callback(arg1, arg2, arg3);
        }
    }

    public void Send<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
    {
        EventData d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            d.Callback(arg1, arg2, arg3, arg4);
        }
    }

    #endregion
}