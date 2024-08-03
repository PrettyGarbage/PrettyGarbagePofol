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

    [SerializeField]
    private string _apiServerUrl;
    [SerializeField]
    private string _googleWebClientId;
    
    public string Version { get { return _version; } }
    public bool IsDevBuild { get { return _isDevBuild; } }
    public string GoogleWebClientId { get { return _googleWebClientId; } }
    public string ApiServerUrl { get { return _apiServerUrl; } }
    
#if UNITY_EDITOR
    public static BuildInfoData CreateAssest(string version, bool isDevBuild, string apiServerUrl, string googleWebClientId)
    {
        BuildInfoData buildInfoData = CreateInstance<BuildInfoData>();
        buildInfoData._version = version;
        buildInfoData._isDevBuild = isDevBuild;

        buildInfoData._apiServerUrl = apiServerUrl;
        buildInfoData._googleWebClientId = googleWebClientId;
        
        AssetDatabase.CreateAsset(buildInfoData, ASSET_PATH);
        AssetDatabase.Refresh();

        return buildInfoData;
    }
#endif
}