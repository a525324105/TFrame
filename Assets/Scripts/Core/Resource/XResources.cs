using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XResources
{
    public static T Load<T>(string name) where T : UnityEngine.Object
    {
        return ResMgr.Instance.Load<T>(name);
    }

    public static UnityEngine.Object Load(string name)
    {
        return ResMgr.Instance.Load(name);
    }

    public static GameObject AllocGameObject(string name)
    {
        return ResMgr.Instance.Load(name) as GameObject;
    }

    public static GameObject AllocGameObject(string name,Transform parent)
    {
        var obj = ResMgr.Instance.Load(name) as GameObject;

        if (obj!=null)
        {
            obj.transform.SetParent(parent);
        }
        return obj;
    }
}
