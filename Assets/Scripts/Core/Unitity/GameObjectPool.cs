using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectPool
{
    private Dictionary<string, PoolData> m_PoolDic = new Dictionary<string, PoolData>();

    private GameObject poolObject;

    public GameObject Get(string name, UnityAction<GameObject> callback = null)
    {
        GameObject gameObject = null;
        if (m_PoolDic.ContainsKey(name) && m_PoolDic[name].PoolList.Count > 0)
        {
            gameObject = m_PoolDic[name].Get();
        }
        else
        {
            gameObject = Object.Instantiate(Resources.Load<GameObject>(name));
            gameObject.name = name;
        }

        if (callback == null)
        {
            return gameObject;
        }
        else
        {
            callback?.Invoke(gameObject);
        }
        return gameObject;
    }

    public void GetAsync(string name, UnityAction<GameObject> callback)
    {
        if (m_PoolDic.ContainsKey(name) && m_PoolDic[name].PoolList.Count > 0)
        {
            callback(m_PoolDic[name].Get());
        }
        else
        {
            //异步加载资源 创建对象给外部用
            ResMgr.Instance.LoadAsync<GameObject>(name, (obj) =>
            {
                obj.name = name;
                callback(obj);
            });
        }
    }
}


public class PoolData
{
    public GameObject FatherObject { get; private set; }

    public IndexedSet<GameObject> PoolList;

    public PoolData(GameObject obj, GameObject poolObject)
    {
        FatherObject = new GameObject(obj.name);

        FatherObject.transform.parent = poolObject.transform;

        PoolList = new IndexedSet<GameObject>();

        Push(obj);
    }

    public GameObject Get()
    {
        GameObject gameObject = null;

        gameObject = PoolList[0];

        PoolList.RemoveAt(0);

        gameObject.SetActive(true);

        gameObject.transform.parent = null;

        return gameObject;
    }

    public void Push(GameObject gameObject)
    {
        gameObject.SetActive(false);

        PoolList.Add(gameObject);

        gameObject.transform.parent = FatherObject.transform;
    }
}