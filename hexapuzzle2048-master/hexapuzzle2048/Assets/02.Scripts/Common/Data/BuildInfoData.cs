using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildInfoData.asset", menuName = "gbros/BuildInfoData", order = 1)]
public class BuildInfoData : ScriptableObject{

    public const string ASSET_PATH = "Assets/Resources/Data/BuildInfoData.asset";

    [SerializeField]
    private string _version;
    [SerializeField]
    private bool _isDevBuild; 

    public string Version
    {
        get
        {
            return _version;
        }
    }

    public bool IsDevBuild
    {
        get
        {
            return _isDevBuild;
        }
        //TODO : 삭제해야함.
        set {
            _isDevBuild = value;
        }
    }

    #if UNITY_EDITOR
    public static BuildInfoData CreateAssest(string version, bool isDevBuild)
    {
        BuildInfoData buildInfoData = CreateInstance<BuildInfoData>();
        buildInfoData._version = version;
        buildInfoData._isDevBuild = isDevBuild;
        AssetDatabase.CreateAsset(buildInfoData, ASSET_PATH);
        AssetDatabase.Refresh();

        return buildInfoData;
    }
#endif
}