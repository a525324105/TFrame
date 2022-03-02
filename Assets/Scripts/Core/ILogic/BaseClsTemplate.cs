using System;

/// <summary>
/// 函数注册实例
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseClsTemplate<T>
{
    private static T m_imp;

    protected static T Imp
    {
        get
        {
            if (Imp == null)
            {
                throw new NotSupportedException("Imp is semantically invalid, Please call function RegistImp before use It.");
            }
            return m_imp;
        }
    }

    /// <summary>
    /// Unity工程，注册处理函数
    /// </summary>
    /// <param name="imp"></param>
    public static void RegistImp(T imp)
    {
        m_imp = imp;
    }
}