using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonConstants  {

    public static WaitForSeconds WaitLoopSeconds = new WaitForSeconds(0.1f);
    public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

    public const string ZERO_STRING = "0";
    public const string PERCENT_STRING = "%";

    public const int INITEGER_FALSE = 0;
    public const int INITEGER_TRUE = 1;

    public const string DAY_YYYYMMDD = "yyyyMMdd";

    //Android
    public const string ANDROID_PACKAGE_NAME = "com.gbros.hexpuz";
    
    //Share Msg
    public const string SHARE_APPLINK_TITLE = "Hexa Puzzle 2048";
    public const string SHARE_APPLINK_MSG = "Merge 2048 hexa blocks to get a star blocks! Play now \"Hexa Puzzle 2048\"";
    public const string SHARE_APPLINK_ANDROID = " \n\n https://play.google.com/store/apps/details?id=";
    public const string SHARE_APPLINK_IOS = "\n\n https://itunes.apple.com/";
}