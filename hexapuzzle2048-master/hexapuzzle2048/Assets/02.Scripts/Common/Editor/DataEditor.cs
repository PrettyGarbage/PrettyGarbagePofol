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
        Debug.Log("GameConstants.SAVEFILE_PATH"+ GameConstants.SAVEFILE_PATH);
        if(File.Exists(GameConstants.SAVEFILE_PATH)){
            File.Delete(GameConstants.SAVEFILE_PATH);
        }
    }
}
