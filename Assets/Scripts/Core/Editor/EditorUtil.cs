using UnityEditor;
using UnityEngine;
using System.Diagnostics;

class EditorUtil
{
    public static System.Diagnostics.Process CreateShellExProcess(string cmd, string args, string workingDir = "")
    {
        var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = true;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;
        //pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        if (!string.IsNullOrEmpty(workingDir))
        {
            pStartInfo.WorkingDirectory = workingDir;
        }
        UnityEngine.Debug.Log(pStartInfo);
        return System.Diagnostics.Process.Start(pStartInfo);
    }

    public static void RunBat(string batfile, string args, string workingDir = "")
    {
        var p = CreateShellExProcess(batfile, args, workingDir);
        p.Close();
    }

    public static string FormatPath(string path)
    {
        path = path.Replace("/", "\\");
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            path = path.Replace("\\", "/");
        }

        return path;
    }
}

public static class EditorConfigUtils
{
    private static void RunMyBat(string batFile, string workingDir)
    {
        var path = EditorUtil.FormatPath(workingDir);
        UnityEngine.Debug.Log(path);
        if (!System.IO.File.Exists(path))
        {
            UnityEngine.Debug.LogError("bat文件不存在：" + path);
        }
        else
        {
            EditorUtil.RunBat(batFile, "", path);
        }
    }

    [MenuItem("TFrame/执行gen_client_cfg.bat")]
    public static void RunBuldBat()
    {
        var path = EditorUtil.FormatPath(Application.dataPath + "/../Produce/");
        UnityEngine.Debug.Log(path);
        if (!System.IO.File.Exists(path+ "gen_client_cfg.bat"))
        {
            UnityEngine.Debug.LogError("bat文件不存在：" + path);
        }
        else
        {
            EditorUtil.RunBat("gen_client_cfg.bat", "", path);
        }
        // 执行bat脚本
    }
}