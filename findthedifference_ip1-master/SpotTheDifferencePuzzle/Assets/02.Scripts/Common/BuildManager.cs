using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

    public const string BUILDINFO_DATA_PATH = "Data/BuildInfoData";

	private static BuildManager _instance;
	public static BuildManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<BuildManager>("BuildManager");
            }

            return _instance;
        }
    }

    public BuildInfoData BuildInfoData
    {
        get
        {
            if (_buildInfoData == null)
            {
                _buildInfoData = Resources.Load(BUILDINFO_DATA_PATH) as BuildInfoData;
            }
            return _buildInfoData;
        }
    }

    private BuildInfoData _buildInfoData;
    private string buildVersionString;

    public IEnumerator Init(){
        

        if(IsDevBuild()){
            Debug.logLevel = Debug.LogLevel.DEBUG;
        }
        else {
            Debug.logLevel = Debug.LogLevel.ERROR;
        }

        buildVersionString = "version " + GetBuildPhase()+ "_" + BuildInfoData.Version.ToString();

         yield return null;
    }

    public string GetVersion(){
        return buildVersionString;
    }

    public BUILD_PHASE GetBuildPhase(){

#if UNITY_EDITOR
        return BUILD_PHASE.DEV;
#else
        if (UnityEngine.Debug.isDebugBuild || BuildInfoData.IsDevBuild)
        {
            return BUILD_PHASE.DEV;
        }
        return BUILD_PHASE.REAL;
#endif
    }

    public bool IsDevBuild(){
        return GetBuildPhase() == BUILD_PHASE.DEV;   
    }

}
