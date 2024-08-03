using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;

public class LocalizationManager : MonoBehaviour {

    public const string DATA_FILE_NAME = "Local_";

    private static LocalizationManager _instance;
    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<LocalizationManager>("LocalizationManager");
            }
            return _instance;
        }
    }

    [SerializeField]
    private TMP_FontAsset _fontAssetEn;
    [SerializeField]
    private TMP_FontAsset _fontAssetKr;
    [SerializeField]
    private TMP_FontAsset _fontAssetJp;

    private LangType _loadedLangType = LangType.KR;

    public static string GetDataFilePath(LangType langType)
    {
        return Path.Combine(Application.streamingAssetsPath + "/", DATA_FILE_NAME + langType + ".json");
    }

    private Dictionary<string, string> _localizationItemDic;

    public void LoadLocalizationItem(Action onLoaded)
    {
        StartCoroutine(LoadLocalizationItem(SettingManager.Instance.LangType, onLoaded));
    }

    public IEnumerator LoadLocalizationItem(LangType langType, Action onLoaded)
    {

        string dataAsJson;
        string filePath = GetDataFilePath(langType);
        Debug.Log("filePath : " + filePath);
#if UNITY_IOS
        dataAsJson = System.IO.File.ReadAllText(filePath);
        ReadJsonData(dataAsJson);
        _loadedLangType = langType;
        onLoaded();
        yield return null; ;
#else
        WWW www = new WWW(filePath);
        while (!www.isDone) {
            yield return CommonConstants.WaitLoopSeconds;
        }
        
        if (www.error !=null)
        {
            Debug.LogError("filePath : " + filePath + "| error : " + www.error);
        }
        else
        {
            dataAsJson = www.text;
            ReadJsonData(dataAsJson);
            _loadedLangType = langType;
            onLoaded();
        }
#endif
    }

    private void ReadJsonData(string dataAsJson)
    {
        LocalizationSimpleItemData localizationSimpleItemData = JsonUtility.FromJson<LocalizationSimpleItemData>(dataAsJson);

        _localizationItemDic = new Dictionary<string, string>();

        for (int i = 0; i < localizationSimpleItemData.localizationSimpleItems.Count; i++)
        {
            LocalizationSimpleItem localizationSimpleItem = localizationSimpleItemData.localizationSimpleItems[i];
            _localizationItemDic[localizationSimpleItem.key] = localizationSimpleItem.value;
        }
    }

    public bool IsInitailized()
    {
        return _localizationItemDic != null;
    }

    public string GetText(LocalizationTextKey localizationTextKey)
    {
        return _localizationItemDic == null ? "" : _localizationItemDic[localizationTextKey.ToString()];
    }

    public string GetText(string localizationTextKey)
    {
        return _localizationItemDic == null ? "" : _localizationItemDic[localizationTextKey];
    }

    public void GetText(LangType langType, LocalizationTextKey localizationTextKey, Action<String> onLoaded)
    {
#if UNITY_EDITOR
        StartCoroutine(LoadLocalizationItem(langType, ()=>{
            onLoaded(_localizationItemDic == null ? "" : _localizationItemDic[localizationTextKey.ToString()]);
        }));
#endif
    }

    public TMP_FontAsset GetFontAsset()
    {
        switch (SettingManager.Instance.LangType)
        {
            case LangType.KR:
                return _fontAssetKr;
            case LangType.JP:
                return _fontAssetJp;
            case LangType.EN:
                return _fontAssetEn;
            default:
                return _fontAssetEn;
        }
    }

    public TMP_FontAsset GetFontAsset(LangType langType)
    {
        switch (langType)
        {
            case LangType.KR:
                return _fontAssetKr;
            case LangType.JP:
                return _fontAssetJp;
            case LangType.EN:
                return _fontAssetEn;
            default:
                return _fontAssetEn;
        }
    }
}
