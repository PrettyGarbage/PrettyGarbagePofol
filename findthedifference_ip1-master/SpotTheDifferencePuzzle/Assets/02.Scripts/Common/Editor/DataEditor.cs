using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataEditor : MonoBehaviour {

	[MenuItem("Gbros/Reset Local UserData")]
    public static void ResetUserData()
    {
        PlayerPrefs.DeleteAll();       
    }

    [MenuItem("Gbros/API/Use Local Api ")]
    public static void UseLocalApi()
    {
        PlayerPrefs.SetString(ApiManager.URL_API_SERVER_KEY, "localhost:8080/api");
    }

    [MenuItem("Gbros/API/Use Remote Api ")]
    public static void UseRemoteApi()
    {
        PlayerPrefs.SetString(ApiManager.URL_API_SERVER_KEY, "http://ec2-52-79-233-254.ap-northeast-2.compute.amazonaws.com:8080/api");
    }

    [MenuItem("Gbros/API/Clear Database Cache")]
    static void ClearDatabaseCache()
    {
        ApiManager.Instance.ClearDatabaseCache();
    }
}
