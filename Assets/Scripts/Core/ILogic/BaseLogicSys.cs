public class BaseLogicSys<T>:ILogicSys where T : new()
{
    private static T m_Instance;

    public static bool HasInstance
    {
        get { return m_Instance != null; }
    }
    public static T Instance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new T();
            }

            return m_Instance;
        }
    }

    #region virtual fucntion
    public virtual bool OnInit()
    {
        if (null == m_Instance)
        {
            m_Instance = new T();
        }
        return true;
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnLateUpdate()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public virtual void OnPause()
    {
    }

    public virtual void OnResume()
    {
    }

    public virtual void OnDrawGizmos()
    {
    }

    public virtual void OnRoleLogout()
    {
    }

    public virtual void OnMapChanged()
    {
    }
    #endregion
}
