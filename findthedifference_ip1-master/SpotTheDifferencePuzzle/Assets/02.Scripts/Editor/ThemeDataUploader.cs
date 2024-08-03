using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

public class ThemeDataUploader
{
    const string ASSET_OUT_PATH = "Assets/AssetsBundle/";

#if UNITY_ANDROID
    const PlatformType platformType = PlatformType.ANDROID;
#elif UNITY_IOS
    const PlatformType platformType = PlatformType.IOS;
#endif

    [MenuItem("Assets/gbros/assetbundle/Theme - BuildAssetBundleAndUpload")]
    static void MThemeBuildAssetBundleAndUpload()
    {
        AssetBundle.UnloadAllAssetBundles(true);

        LoopSelection(Selection.objects, (Object obj) => {

            string path = AssetDatabase.GetAssetPath(obj);
            
            PuzzleThemeData puzzleThemeData = AssetDatabase.LoadAssetAtPath<PuzzleThemeData>(path);
            if (puzzleThemeData == null)
            {
                Debug.Log("not found puzzleThemeData => " + obj.name);
                return;
            }

            Debug.Log("puzzleThemeData" + puzzleThemeData.ThemeId);
            string[] assetPaths = new string[puzzleThemeData.PuzzleImageDatas.Length+1];
            string assetbundlePath = GetAssetBundlePath(puzzleThemeData.ThemeId);
            string themeId = puzzleThemeData.ThemeId;

            //Debug.Log("puzzleThemeData.ThemeImage : " + puzzleThemeData.ThemeImage.name +", path ; "  + ); 

            for (int i = 0; i < puzzleThemeData.PuzzleImageDatas.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GetAssetPath(puzzleThemeData.PuzzleImageDatas[i]);
            }
            assetPaths[assetPaths.Length-1] = AssetDatabase.GetAssetPath(puzzleThemeData.ThemeImage);

            AssetBuild(platformType, themeId, assetPaths);

            AssetThemeInfo assetThemeInfo = new AssetThemeInfo();
            assetThemeInfo = SetAssetThemeInfo(assetThemeInfo, puzzleThemeData, platformType, assetbundlePath);
            Debug.Log("ReadAllBytes");
            byte[] aseetFileByte = File.ReadAllBytes(assetbundlePath);

            //theme
            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.id = themeId;
            themeInfo.name = themeId;

            //stage
            assetThemeInfo.theme = themeInfo;

            UploadThemeToServer(platformType, themeId, assetThemeInfo, aseetFileByte);


        });
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/gbros/assetbundle/Collection - BuildAssetBundleAndUpload")]
    static void MCollectionBuildAssetBundleAndUpload()
    {

        AssetBundle.UnloadAllAssetBundles(true);

        LoopSelection(Selection.objects, (Object obj) => {

            string path = AssetDatabase.GetAssetPath(obj);

            CollectionSpriteData collectionSpriteData = AssetDatabase.LoadAssetAtPath<CollectionSpriteData>(path);
            if (collectionSpriteData == null)
            {
                Debug.Log("not found collectionSpriteData => " + obj.name);
                return;
            }

            Debug.Log("collectionSpriteData = " + collectionSpriteData.CollectionId);
            string[] assetPaths = new string[2];
            string assetbundlePath = GetAssetBundlePath(collectionSpriteData.CollectionId);
            string collectionId = collectionSpriteData.CollectionId;

            assetPaths[0] = AssetDatabase.GetAssetPath(collectionSpriteData.CollectionImage);
            assetPaths[1] = AssetDatabase.GetAssetPath(collectionSpriteData.CollectionBigImage);

            AssetBuild(platformType, collectionId, assetPaths);

            AssetCollectionInfo assetCollectionInfo = CreateAssetCollectionInfo(platformType, collectionSpriteData, assetbundlePath);
            byte[] aseetFileByte = File.ReadAllBytes(assetbundlePath);
            UploadCollectionToServer(platformType, collectionId, assetCollectionInfo, aseetFileByte);


        });
        AssetDatabase.Refresh();
    }

    static void LoopSelection(Object[] objects, Action<Object> callback)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            callback(objects[i]);
        }
    }

    static void AssetBuild(PlatformType platformType, string bundleName, string[] assetPaths)
    {  
        Debug.Log("BuildAssetBundle : " + bundleName + "/ " + assetPaths.ToString());
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild
        {
            assetBundleName = bundleName,
            assetNames = assetPaths
        };

        buildMap[0] = assetBundleBuild;
        BuildTarget buildTarget = BuildTarget.Android;
        if (platformType == PlatformType.IOS)
        {
            buildTarget = BuildTarget.iOS;
        }
        BuildPipeline.BuildAssetBundles(GetAssetBundlePath(), buildMap, BuildAssetBundleOptions.None, buildTarget);
    }

    static AssetThemeInfo SetAssetThemeInfo(AssetThemeInfo assetThemeInfo, PuzzleThemeData puzzleThemeData, PlatformType platformType, string assetBundlePath)
    {
        Debug.Log("SetAssetThemeInfo");
        Debug.Log(" puzzleThemeData : " + puzzleThemeData.name);

        uint crc = 0;
        if (BuildPipeline.GetCRCForAssetBundle(assetBundlePath, out crc))
        {
            Debug.Log("crc : " + crc);
        }

        Hash128 hash;
        if (BuildPipeline.GetHashForAssetBundle(assetBundlePath, out hash))
        {
            Debug.Log("hash : " + hash);
        }

        AssetBundleInfo assetBundleInfo = new AssetBundleInfo();
        assetBundleInfo.rootName = puzzleThemeData.ThemeId;
        assetBundleInfo.crc = crc.ToString();
        assetBundleInfo.hash = hash.ToString();

        assetThemeInfo.stageCount = puzzleThemeData.PuzzleImageDatas.Length;
        assetThemeInfo.assetBundle = assetBundleInfo;
        
        return assetThemeInfo;
    }

    static void UploadThemeToServer(PlatformType platformType, string themeId, AssetThemeInfo assetThemeInfo, byte[] aseetFileByte)
    {
        ApiManager.Instance.UploadTheme(platformType, assetThemeInfo, aseetFileByte, (apiResult =>
        {
            UploadResult(apiResult, themeId);
        }));

    }

    private static void UploadCollectionToServer(PlatformType platformType, string collectionId, AssetCollectionInfo assetCollectionInfo, byte[] aseetFileByte)
    {
        ApiManager.Instance.UploadCollection(platformType, assetCollectionInfo, aseetFileByte, (apiResult => {

            UploadResult(apiResult, collectionId);

        }));
    }

    private static void UploadResult(ApiResult<bool> apiResult, string objectName)
    {
        if (apiResult.isSuccess)
        {
            Debug.Log("==== upload success : " + objectName);
        }
        else
        {
            Debug.Log("==== upload fail : " + objectName);
        }

        AssetBundle.UnloadAllAssetBundles(true);
    }

    private static AssetCollectionInfo CreateAssetCollectionInfo(PlatformType platformType, CollectionSpriteData collectionSpriteData , string assetBundlePath)
    {
        AssetCollectionInfo assetCollectionInfo = new AssetCollectionInfo();
        
        uint crc = 0;
        if (BuildPipeline.GetCRCForAssetBundle(assetBundlePath, out crc))
        {
            Debug.Log("crc : " + crc);
        }

        Hash128 hash;
        if (BuildPipeline.GetHashForAssetBundle(assetBundlePath, out hash))
        {
            Debug.Log("hash : " + hash);
        }

        AssetBundleInfo assetBundleInfo = new AssetBundleInfo();
        assetBundleInfo.rootName = collectionSpriteData.CollectionId;
        assetBundleInfo.crc = crc.ToString();
        assetBundleInfo.hash = hash.ToString();

        assetCollectionInfo.collectionId = collectionSpriteData.CollectionId;
        assetCollectionInfo.assetBundle = assetBundleInfo;

        return assetCollectionInfo;
    }


    static string GetAssetBundlePath()
    {
        return ASSET_OUT_PATH + platformType.ToString();
    }

    static string GetAssetBundlePath(string fileName)
    {
        return ASSET_OUT_PATH + platformType.ToString() + "/" + fileName;
    }

}
#endif