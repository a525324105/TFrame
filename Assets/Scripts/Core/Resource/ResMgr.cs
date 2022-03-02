using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResMgr:Singleton<ResMgr>
{

    #region 同步加载
    public T Load<T>(string name) where T : UnityEngine.Object
    {
        T res;
        res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            return Object.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    public UnityEngine.Object Load(string name)
    {
        var res = Resources.Load(name);
        if (res is GameObject)
        {
            return UnityEngine.Object.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    public UnityEngine.Object Load(string path, string name)
    {
        var res = Resources.Load(path + "/" + name);
        if (res is GameObject)
        {
            res.name = name;
            return UnityEngine.Object.Instantiate(res);
        }
        else
        {
            return res;
        }
    }


    #endregion

    #region 异步加载
    public T LoadAsync<T>(string name, UnityAction<T> callback = null) where T : UnityEngine.Object
    {
        T res = null;

        MonoManager.Instance.StartCoroutine(ReallyLoadAsync(name, callback));

        return res;
    }

    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback = null) where T : UnityEngine.Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);

        yield return r;

        if (r.asset is GameObject)
        {
            callback?.Invoke(Object.Instantiate(r.asset) as T);
        }
        else
        {
            callback?.Invoke(r.asset as T);
        }
    }
    #endregion
}
