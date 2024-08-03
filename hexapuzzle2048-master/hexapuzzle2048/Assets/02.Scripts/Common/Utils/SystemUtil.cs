using UnityEngine;
using System.Collections;
using System;

public delegate void Callback(params object[] args);
public delegate void ActionParams(params object[] args);
public delegate void ActionString(string param);
public delegate void ActionInt(int param);
public delegate void ActionObject<T>(T param);
public delegate void ActionBool(bool param);

public static class SystemUtil
{

    public static int targetFrameRate
    {
        set
        {
            //TODO : 기기별 분기 필요할 수도...
            Application.targetFrameRate = value;
        }

        get
        {
            return Application.targetFrameRate;
        }
    }

    public static void SetIgnoreLayerCollision()
    {
        //Physics.IgnoreLayerCollision(GameConstants.LAYER_INGAME_IGNORE_PLAYER, GameConstants.LAYER_INGAME_PLAYER);
        //Physics.IgnoreLayerCollision(GameConstants.LAYER_INGAME, GameConstants.LAYER_BG);
        //Physics.IgnoreLayerCollision(GameConstants.LAYER_PLAYER_CONTROLLER, GameConstants.LAYER_BG);
        //Physics.IgnoreLayerCollision(GameConstants.LAYER_INGAME, GameConstants.LAYRT_INGAME_RAYCAST);
    }

    public static Color HexToColor(string hexString)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hexString, out color);
        return color;
    }

    public static string GetCommaText(int data)
    {

        if (data == 0)
        {
            return CommonConstants.ZERO_STRING;
        }

        return string.Format("{0:#,###}", data);
    }

    public static bool HasCommandLineArg(string argumentName)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(argumentName))
            {
                return true;
            }
        }

        return false;
    }

    public static int GetCommandLineArgValue(string argumentName, int nDefaultValue)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(argumentName))
            {
                if (i == (args.Length - 1)) // Last arg, return default
                {
                    return nDefaultValue;
                }

                return Int32.Parse(args[i + 1]);
            }
        }

        return nDefaultValue;
    }

    public static float GetCommandLineArgValue(string argumentName, float flDefaultValue)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(argumentName))
            {
                if (i == (args.Length - 1)) // Last arg, return default
                {
                    return flDefaultValue;
                }

                return (float)Double.Parse(args[i + 1]);
            }
        }

        return flDefaultValue;
    }

    public static string GetNowDayFormat(){
        return DateTime.Now.ToString(CommonConstants.DAY_YYYYMMDD);
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}