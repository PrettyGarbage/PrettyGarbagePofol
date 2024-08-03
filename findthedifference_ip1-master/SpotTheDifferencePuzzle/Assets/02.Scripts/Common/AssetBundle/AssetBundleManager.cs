using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

using Object = UnityEngine.Object;

public struct AssetbundlePregressInfo
{
    public float totalProgress;
    public float partsProgress;
    public double loadedFileByteSize;
    public double totalFileMbSize;    
}

public class AssetBundleManager : MonoBehaviour
{
	#region SINGLETON
	public static AssetBundleManager _instance;

    private bool _isLoaded;

	public static AssetBundleManager Instance
	{
		get
		{
			if (_instance == null)
			{
                _instance = ObjectUtil.CreateInstance<AssetBundleManager>("AssetBundleManager");                
				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;
		}
	}

    public bool IsLoaded { get { return _isLoaded; } set { _isLoaded = value; } }
    #endregion SINGLETON

    private Dictionary<string, AssetBundle> _assetBundleDic = new Dictionary<string, AssetBundle>();

    public void LoadAsset(AssetBundleInfo assetBundleInfo, bool isOnlyDownload, Action<float> onUpdate, Action onLoaded) 
	{
        Debug.Assert(!string.IsNullOrEmpty(assetBundleInfo.url));
		Debug.Assert(!string.IsNullOrEmpty(assetBundleInfo.rootName));
		Debug.Assert(onLoaded != null);

		Debug.Log("LoadAsset : " + assetBundleInfo.url);
        StartCoroutine(ProcessLoadBundle(assetBundleInfo, isOnlyDownload, onUpdate, onLoaded));        
    }

    public double GetTotalFileMbSize(List<AssetBundleInfo> assetBundleInfoList)
    {
        double totalFileMbSize = 0;
        assetBundleInfoList.ForEach(bundleinfo => {
            totalFileMbSize += SystemUtil.ConvertBytesToMegabytes(bundleinfo.fileSize);
         });
        return Math.Round(totalFileMbSize, 2); ;
    }

    public void LoadAssetBundles(List<AssetBundleInfo> assetBundleInfoList, bool isOnlyDownload, Action<AssetbundlePregressInfo> onUpdate, Action onLoaded)
    {
        StartCoroutine(LoadAssetBundlesCor(assetBundleInfoList, isOnlyDownload, onUpdate, onLoaded));
    }

    #region COROUTINES    
    public IEnumerator LoadAssetBundlesCor(List<AssetBundleInfo> assetBundleInfoList, bool isOnlyDownload, Action<AssetbundlePregressInfo> onUpdate, Action onLoaded)
    {
        AssetbundlePregressInfo assetbundlePregressInfo = new AssetbundlePregressInfo();
        assetbundlePregressInfo.totalFileMbSize = GetTotalFileMbSize(assetBundleInfoList);

        int bundleCount = assetBundleInfoList.Count;
        for (int i = 0; i < assetBundleInfoList.Count; i++)
        {
            bool isLoaded = false;
            AssetBundleInfo assetBundleInfo = assetBundleInfoList[i];
            Debug.Log("LoadAssetBundles assetBundleInfo => " + assetBundleInfo.rootName);
            LoadAsset(assetBundleInfo, isOnlyDownload
               , (progress) => {
                   assetbundlePregressInfo.partsProgress = progress;
                   if(onUpdate!=null)
                       onUpdate(assetbundlePregressInfo);
               }
               , () =>
               {
                   assetbundlePregressInfo.loadedFileByteSize += assetBundleInfo.fileSize;
                   assetbundlePregressInfo.totalProgress = ((float)(i) / (float)bundleCount);
                   if (onUpdate != null)
                       onUpdate(assetbundlePregressInfo);
                   isLoaded = true;
               }
           );

            while (!isLoaded)
            {
                yield return CommonConstants.WaitLoopSeconds;
            }
        }

        onLoaded();

    }


    IEnumerator ProcessLoadBundle(AssetBundleInfo assetBundleInfo, bool isOnlyDownload, Action<float> onUpdate,  Action onLoaded) 
    {
        string bundlePath =  DataManager.Instance.AppInfo.cdnUrl + assetBundleInfo.url;
        string assetName = assetBundleInfo.rootName;

        if (_assetBundleDic.ContainsKey(assetName))
        {
            onUpdate(1f);
            onLoaded();
            yield break;
        }


        Hash128 hash = Hash128.Parse(assetBundleInfo.hash);
        uint crc = uint.Parse(assetBundleInfo.crc);
        UnityWebRequest uwr;

        string fileName = GetFileName(assetBundleInfo);
        string hashVersion = GetBundleVersion(fileName);

        if (!IsCachedHash(assetBundleInfo))
        {
            if (Caching.ClearAllCachedVersions(fileName))
            {
                Debug.Log(" == clear cache :  " + fileName);
            }
        }
        
        Debug.Log(" == fileName :  " + fileName);
        Debug.Log(" == Caching IsVersionCached " + assetBundleInfo.rootName + " : "+ Caching.IsVersionCached(bundlePath, hash));
        
        using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath, hash, crc))
        {
            uwr.SendWebRequest();
            
            while (!uwr.isDone)
            {
                onUpdate(uwr.downloadProgress);
                yield return CommonConstants.WaitLoopSeconds;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                if (isOnlyDownload)
                {
                    SaveBundleVersion(fileName, assetBundleInfo.hash);
                }else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                    _assetBundleDic.Add(assetName, bundle);
                }

                
                onLoaded();
            }
        }
        
    }
	#endregion COROUTINES

    public T GetAsset<T>(string bundleName, string assetName) where T : Object
    {  
        if (_assetBundleDic.ContainsKey(bundleName))
        {  
            return _assetBundleDic[bundleName].LoadAsset<T>(assetName);
        }

        return null;
    }

    private void SaveBundleVersion(string id, string hashString)
    {
        PlayerPrefs.SetString(id, hashString);
    }

    private string GetBundleVersion(string id)
    {
        return PlayerPrefs.GetString(id, string.Empty);
    }

    public bool IsCachedHash(AssetBundleInfo assetBundleInfo)
    {
        string fileName = GetFileName(assetBundleInfo);
        string hashVersion = GetBundleVersion(fileName);
        return hashVersion.Equals(assetBundleInfo.hash);
    }

    private string GetFileName(AssetBundleInfo assetBundleInfo)
    {
        string[] fileNames = assetBundleInfo.url.Split('/');
        return fileNames[fileNames.Length - 1];
    }

    public List<AssetBundleInfo> GetUpdateDownloadList(List<AssetBundleInfo> assetBundleInfoList)
    {
        List<AssetBundleInfo> downloadSssetBundleInfoList = new List<AssetBundleInfo>();
        
        assetBundleInfoList.ForEach(assetBundleInfo =>
        {
            if (!IsCachedHash(assetBundleInfo))
            {
                Debug.Log("GetUpdateDownloadMbSize = >" + assetBundleInfo.ToString());
                downloadSssetBundleInfoList.Add(assetBundleInfo);
            }
        });

        return downloadSssetBundleInfoList;        
    }

}