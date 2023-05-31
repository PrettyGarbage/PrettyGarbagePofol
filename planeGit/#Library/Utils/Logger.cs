using System;
using System.Text;
using UnityEngine;

public static class Logger
{
    public enum LogLevel
    {
        Log,
        Warning,
        Error,
        Network,
        Resource,
        Scene,
        Data,
        Scenario,
    }
    
    //스트링빌더 항상 사용할때는 비워두고 사용할 것! StringBuilder.Clear();
    public static StringBuilder Sb = new StringBuilder();

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object str, LogLevel type = LogLevel.Log)
    {
        LogFormat(str, type);

        Debug.Log(Sb.ToString());
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(object str, LogLevel type = LogLevel.Log)
    {
        LogFormat(str, type);

        Debug.LogError(Sb.ToString());
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object str, LogLevel type = LogLevel.Log)
    {
        LogFormat(str, type);

        Debug.LogWarning(Sb.ToString());
    }

    ///<summary>
    ///LogEnter는 씬이나 해당 뷰에 들어갔을 때 들어간 것인지 빠져나온 것인지 판단해서 로그 띄워줌
    ///</summary>
    ///<param name="isEnter"></param>
    ///<typeparam name="T"></typeparam>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogEnter<T>(bool isEnter = true)
    {
        var strEnter = isEnter ? " Enter" : " Leave";
            
        Log(typeof(T).Name + strEnter, LogLevel.Scene);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogEnter(Type type, bool isEnter = true)
    {
        var strEnter = isEnter ? " Enter" : " Leave";

        Log(type.Name + strEnter, LogLevel.Scene);
    }

    private static void LogFormat(object str, LogLevel type)
    {
        Sb.Clear();
        switch (type)
        {
            case LogLevel.Log:
                Sb.Append("<color=white>");
                break;
            case LogLevel.Warning:
                Sb.Append("<color=#FDC95D>");
                break;
            case LogLevel.Error:
                Sb.Append("<color=#E96363>");
                break;
            case LogLevel.Network:
                Sb.Append("<color=#97d5e0>");
                break;
            case LogLevel.Resource:
                Sb.Append("<color=#754100>");
                break;
            case LogLevel.Scene:
                Sb.Append("<color=#754100>");
                break;
            case LogLevel.Data:
                Sb.Append("<color=#93b3b7>");
                break;
            case LogLevel.Scenario:
                Sb.Append("<color=#99ccff>");
                break;
            default:
                Sb.Append("<color=white>");
                break;
        }

        Sb.Append("<b> [").Append(str).Append("] </b></color>");
    }
}