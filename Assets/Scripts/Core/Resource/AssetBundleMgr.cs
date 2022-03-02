using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class AssetBundleMgr : UnitySingleton<AssetBundleMgr>
{
    #region 属性

    private AssetBundle m_MainAB;
    private AssetBundleManifest m_MainFest;
    private Dictionary<string, AssetBundle> m_AssetsDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// AB包存放路径
    /// </summary>
    private string PathUrl
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Application.persistentDataPath + "/AssetsBundle/";
#elif UNITY_IPHONE && !UNITY_EDITOR
        return Application.dataPath + "/Raw" + "/AssetsBundle/";
#else
            return Application.streamingAssetsPath + "/AssetsBundle/";
#endif
        }
    }

    private string MainABName => "AssetsBundle";

    /// <summary>
    /// AB在线地址
    /// </summary>
    private string OnlinePathUrl
    {
        get
        {
            return "https://hotfix-1258327636.cos.ap-guangzhou.myqcloud.com/ab/";
        }
    }
    #endregion

    #region Methods
    public void LoadAB(string abName)
    {
        if (m_MainAB == null)
        {
            m_MainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            m_MainFest = m_MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        AssetBundle ab;
        string[] strs = m_MainFest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!m_AssetsDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                m_AssetsDic.Add(strs[i], ab);
            }
        }

        if (!m_AssetsDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            m_AssetsDic.Add(abName, ab);
        }
    }

    //--------------------------------------调用接口------------------------------------------//

    #region 同步加载
    /// <summary>
    /// 同步加载 不指定类型 
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public UnityEngine.Object GetAssetCache(string abName, string resName)
    {
        LoadAB(abName);

        var ab = m_AssetsDic[abName].LoadAsset(resName);

        if (ab is GameObject)
        {
            Instantiate(ab);

            return ab;
        }
        else
        {
            return ab;
        }
    }

    /// <summary>
    /// 同步加载 根据type指定类型   BECAUSE Lua不支持泛型，传类型
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public UnityEngine.Object GetAssetCache(string abName, string resName, System.Type type)
    {
        LoadAB(abName);

        var ab = m_AssetsDic[abName].LoadAsset(resName, type);

        if (ab is GameObject)
        {
            Instantiate(ab);

            return ab;
        }
        else
        {
            return ab;
        }
    }

    /// <summary>
    /// 同步加载 根据泛型指定类型   //对于C#最方便的泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T GetAssetCache<T>(string abName, string resName) where T : UnityEngine.Object
    {
        LoadAB(abName);

        var ab = m_AssetsDic[abName].LoadAsset<T>(resName);

        if (ab is GameObject)
        {
            Instantiate(ab);

            return ab as T;
        }
        else
        {
            return ab;
        }
    }
    #endregion

    #region 异步加载
    /// <summary>
    /// 异步加载 根据名字异步加载
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    public void GetAssetCacheAsync(string abName, string resName, UnityAction<UnityEngine.Object> callback)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, callback));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<UnityEngine.Object> callback)
    {
        LoadAB(abName);
        AssetBundleRequest abr = m_AssetsDic[abName].LoadAssetAsync(resName);
        yield return abr;
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }

    /// <summary>
    /// 异步加载 根据Type异步加载
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callback"></param>
    public void GetAssetCacheAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callback)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadResAsync(abName, resName, type, callback));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<UnityEngine.Object> callback)
    {
        LoadAB(abName);
        AssetBundleRequest abr = m_AssetsDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset));
        }
        else
        {
            callback(abr.asset);
        }
    }

    /// <summary>
    /// 异步加载 根据泛型异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    public void GetAssetCacheAsync<T>(string abName, string resName, UnityAction<T> callback = null) where T : UnityEngine.Object
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callback));
    }

    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callback) where T : UnityEngine.Object
    {
        LoadAB(abName);
        AssetBundleRequest abr = m_AssetsDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        if (abr.asset is GameObject)
        {
            callback(Instantiate(abr.asset) as T);
        }
        else
        {
            callback(abr.asset as T);
        }
    }
    #endregion


    #endregion

    public T[] LoadAllAssets<T>(string abName) where T : UnityEngine.Object
    {
        if (m_MainAB == null)
        {
            m_MainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            m_MainFest = m_MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        AssetBundle ab;
        string[] strs = m_MainFest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!m_AssetsDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                m_AssetsDic.Add(strs[i], ab);
            }
        }

        if (!m_AssetsDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            m_AssetsDic.Add(abName, ab);
        }

        return m_AssetsDic[abName].LoadAllAssets<T>();
    }

    /// <summary>
    /// 单个AB卸载
    /// </summary>
    /// <param name="abName"></param>
    public void UnLoadAB(string abName)
    {
        if (m_AssetsDic.ContainsKey(abName))
        {
            m_AssetsDic[abName].Unload(false);
            m_AssetsDic.Remove(abName);
        }
    }

    /// <summary>
    /// 所有AB卸载
    /// </summary>
    public void UnLoadALLAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        m_AssetsDic.Clear();
        m_MainAB = null;
        m_MainFest = null;
    }
    //---------------------------------------------------------------------------------------//


    public T GetAssetCache<T>(string resName) where T : UnityEngine.Object
    {
        Debug.Log("<color=#D959B9>LOAD BY ASSETSBUNDLE MANAGER:" + resName + "</color>");

        var abName = "sprite";

        UnityEngine.Object target = GetAssetCache<T>(abName, resName);

        return target as T;
    }

    /// <summary>
    /// 检查是否存在Assetbundle
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public bool CheckAssetsBundle(string bundleName)
    {
        if (Directory.Exists(PathUrl))
        {
            DirectoryInfo direction = new DirectoryInfo(PathUrl);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                if (bundleName == files[i].Name)
                {
                    return true;
                }
            }
        }
        return false;
    }
}