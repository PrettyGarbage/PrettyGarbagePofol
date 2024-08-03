using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.IO;
using System;
using System.Text;


// Output the build size or a failure depending on BuildPlayer.
#if UNITY_EDITOR

[Serializable]
public class BuildInfo
{
    public string title;
    public string[] scenePaths;
    public string keystoreName;
    public string keystorePass;
    public string keyaliasName;
    public string keyaliasPass;

    public void PrintString(){
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < scenePaths.Length; i++)
        {
            sb.AppendLine("ScenePaths :");
            sb.AppendLine(scenePaths[i]);    
        }
        sb.AppendLine("keystoreName :");
        sb.AppendLine(keystoreName);
        sb.AppendLine("keystorePass :");
        sb.AppendLine(keystorePass);
        sb.AppendLine("keyaliasName :");
        sb.AppendLine(keyaliasName);
        sb.AppendLine("keyaliasPass :");
        sb.AppendLine(keyaliasPass);
        Debug.Log(sb);
    }
}

public class BuildPlayer : MonoBehaviour
{

    static BuildInfo buildInfo;
    static string targetPath;
    static string fileName;
    static string buildNumber;

    [MenuItem("Gbros/Build/Test LoadBuildInfo")]
    static void LoadBuildInfo(){
        //string path = "D:/01.Projects/Unity/AuthData/Keystore/hexapuzzle2048/build.json";
        //string path = "/Users/Shared/Jenkins/Home/workspace/authdata/Keystore/hexapuzzle2048/build.json";
        string[] args = System.Environment.GetCommandLineArgs();
        string path = args[args.Length - 4];
        targetPath = args[args.Length - 3];
        fileName = args[args.Length - 2];
        buildNumber = args[args.Length - 1];
        Debug.Log("====" + path + " targetPath: " + targetPath + "| fileName : " + fileName  + "| buildNumber : " + buildNumber);
        string jsonString = File.ReadAllText (path);
        buildInfo = JsonUtility.FromJson<BuildInfo>(jsonString);

        buildInfo.PrintString();
    }

    [MenuItem("Gbros/Build/Test Build Android")]
    public static void TestsBuildAndroid()
    {
        LoadBuildInfo();
        BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions(UpdateBuildInfo(true));
        
        BuildApp(buildPlayerOptions);
    }

    [MenuItem("Gbros/Build/Test Build Android(Development)")]
    public static void TestsBuildAndroidDevelopment()
    {
        LoadBuildInfo();
        BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions(UpdateBuildInfo(true));
        buildPlayerOptions.options = BuildOptions.Development;
        
        BuildApp(buildPlayerOptions);
    }

    [MenuItem("Gbros/Build/Real Build Android")]
    public static void BuildAndroid()
    {
        LoadBuildInfo();
        BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions(UpdateBuildInfo(false));
        
        BuildApp(buildPlayerOptions);
    }

    private static void BuildApp(BuildPlayerOptions buildPlayerOptions){
        
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static BuildInfoData UpdateBuildInfo(bool isDevBuild){
        string version = "0.01";
        
        //BuildInfoData buildInfoData = Resources.Load(BuildManager.BUILDINFO_DATA_PATH) as BuildInfoData;
        BuildInfoData buildInfoData = (BuildInfoData)AssetDatabase.LoadAssetAtPath(BuildInfoData.ASSET_PATH, typeof(BuildInfoData));
        if(buildInfoData!=null){
            Debug.Log("old version : " + buildInfoData.Version);
            version = buildNumber;
        }
        
        buildInfoData = BuildInfoData.CreateAssest(version, isDevBuild);
        PlayerSettings.bundleVersion = buildInfoData.Version.ToString();
        
        PlayerSettings.Android.keystoreName = buildInfo.keystoreName;
        PlayerSettings.Android.keystorePass = buildInfo.keystorePass;
        PlayerSettings.Android.keyaliasName = buildInfo.keyaliasName;
        PlayerSettings.Android.keyaliasPass = buildInfo.keyaliasPass;
        
        
        if(isDevBuild){
            PlayerSettings.Android.useAPKExpansionFiles = false;            
            Debug.Log("====bundleVersion : " + PlayerSettings.bundleVersion);             
            LunarConsoleEditorInternal.Installer.EnablePlugin();
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
        }
        else {
            PlayerSettings.Android.useAPKExpansionFiles = false;
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            PlayerSettings.Android.bundleVersionCode += 1;
            LunarConsoleEditorInternal.Installer.DisablePlugin();
        }

        Debug.Log("====buildInfoData version : "+ buildInfoData.Version + "| IsDevBuild : " + buildInfoData.IsDevBuild);
        return buildInfoData;
    }

    private static BuildPlayerOptions GetBuildPlayerOptions(BuildInfoData buildInfoData){
        string locationPathName;
        string versionString = buildInfoData.Version.ToString().Replace(".","_");
        locationPathName = targetPath + fileName + versionString+".apk";

        Debug.Log("====locationPathName : "+ locationPathName);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = buildInfo.scenePaths;
        buildPlayerOptions.locationPathName = locationPathName;
        buildPlayerOptions.target = BuildTarget.Android;
		buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
        buildPlayerOptions.options = BuildOptions.None;
        return buildPlayerOptions;
    }
}
#endif