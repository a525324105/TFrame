using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    /// <summary>
    /// 切换场景 同步
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction callback = null)
    {
        SceneManager.LoadScene(name);

        callback?.Invoke();
    }

    /// <summary>
    /// 切换场景 异步
    /// </summary>
    /// <param name="name"></param>
    public void LoadSceneAsyn(string name, UnityAction callback = null)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, callback));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction callback = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        while (!ao.isDone)
        {
            //EventCenter.Instance.EventTrigger("Load", ao.progress);

            yield return ao.progress;
        }

        yield return ao;

        callback?.Invoke();
    }
}
