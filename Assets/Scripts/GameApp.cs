using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : UnitySingleton<GameApp>
{
    void Start()
    {
        var a = UISys.Mgr;
        UISys.Mgr.ShowWindow<LogUI>();
    }
}

public class LogUI : UIWindow
{

}
