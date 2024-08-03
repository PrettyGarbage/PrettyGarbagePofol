using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ApiCachDataInfo<T>
{
    private List<T> _list;
    private bool _isCach = false;

    public bool IsCach { get { return _isCach; } }

    public List<T> List { get { return _list; } }

    public void SetCach(List<T> list)
    {
        this._list = list;
        _isCach = true;
    }

    public void UnSetCach()
    {
        _list = null;
        _isCach = false;
    }
}

public class ApiCachDataInfoDic<T>
{
    private Dictionary<string, ApiCachDataInfo<T>> _dic = new Dictionary<string, ApiCachDataInfo<T>>();

    public bool IsCach(string key)
    {
        if (_dic.ContainsKey(key))
        {
            return _dic[key].IsCach;
        }
        else
        {
            return false;
        }
    }

    public List<T> GetCachList(string key)
    {
        if (_dic.ContainsKey(key))
        {
            return _dic[key].List;
        }
        else
        {
            return null;
        }
    }

    public void SetCach(string key, List<T> list)
    {
        if (_dic.ContainsKey(key))
        {
            _dic[key].SetCach(list);
        }
        else
        {
            ApiCachDataInfo<T> newApiCachData = new ApiCachDataInfo<T>();
            newApiCachData.SetCach(list);
            _dic.Add(key, newApiCachData);
        }
    }

    public void UnSetCach(string key)
    {
        if (_dic.ContainsKey(key))
        {
            _dic[key].UnSetCach();
        }
    }

    public void UnSetAllCach()
    {
        foreach (string key in _dic.Keys)
        {
            _dic[key].UnSetCach();
        }
    }

}

public class ApiManager : MonoBehaviour {

    private static ApiManager _instance;
    private ApiToken _apiToken;

    public const string URL_API_SERVER_KEY = "URL_API_SERVER";
    public const string API_LOG_PREFIX = "=====API======{  ";

    public static ApiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<ApiManager>("ApiManager");
                _instance.SetApiToken(PlayerPrefs.GetString(GameConstants.PLAYERPREFS_API_TOKEN, ""));
            }

            return _instance;
        }
    }

    public ApiToken ApiToken { get { return _apiToken; } }
    public string API_SERVER_URL { get { return PlayerPrefs.GetString(URL_API_SERVER_KEY, BuildManager.Instance.BuildInfoData.ApiServerUrl); } }

    private ApiCachDataInfo<UserThemeInfo> _userThemeCach = new ApiCachDataInfo<UserThemeInfo>();
    private ApiCachDataInfoDic<UserStageInfo> _userStageCachDic = new ApiCachDataInfoDic<UserStageInfo>();

    public void SetApiToken(string token)
    {
        _apiToken = new ApiToken(token);
        PlayerPrefs.SetString(GameConstants.PLAYERPREFS_API_TOKEN, token);
    }

    public void RemoveApiToken()
    {
        PlayerPrefs.DeleteKey(GameConstants.PLAYERPREFS_API_TOKEN);
    }

    #region API POST, GET
    IEnumerator PostRequest<T>(string uri, object postObject, Action<ApiResult<T>> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(API_SERVER_URL + uri, ""))
        {

            webRequest.SetRequestHeader(GameConstants.API_CONTENT_TYPE_TEXT, GameConstants.API_CONTENT_TYPE_JSON);
            webRequest.SetRequestHeader(GameConstants.API_HEADER_AUTHORIZATION, GameConstants.API_TOKEN_PREFIX + _apiToken.token);

            if (postObject != null)
            {
                string formdata = JsonUtility.ToJson(postObject);
                Debug.Log(API_LOG_PREFIX+"formdata : " + formdata);
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(formdata));
            }

            webRequest.uploadHandler.contentType = GameConstants.API_CONTENT_TYPE_JSON;

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(API_LOG_PREFIX + uri + ": Error: " + webRequest.error);
                ErrorHandle(webRequest.error, callback);
            }
            else
            {
                Debug.Log(uri + ":\nReceived: " + webRequest.downloadHandler.text);
                PostResponse<T>(webRequest.downloadHandler.text, callback);
            }
        }
    }

    IEnumerator GetRequest<T>(string uri, Action<ApiResult<T>> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(API_SERVER_URL + uri))
        {
            webRequest.SetRequestHeader(GameConstants.API_CONTENT_TYPE_TEXT, GameConstants.API_CONTENT_TYPE_JSON);
            webRequest.SetRequestHeader(GameConstants.API_HEADER_AUTHORIZATION, GameConstants.API_TOKEN_PREFIX + _apiToken.token );

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(API_LOG_PREFIX + uri + ": Error: " + webRequest.error);
                ErrorHandle(webRequest.error, callback);
            }
            else
            {
                Debug.Log(API_LOG_PREFIX + uri + ":\nReceived: " + webRequest.downloadHandler.text);
                PostResponse<T>(webRequest.downloadHandler.text, callback);
            }
        }
    }

    private void ErrorHandle<T>(String errorMessge, Action<ApiResult<T>> callback)
    {
        OpenPopupRestartError(new GameErrorInfo(ApiErrorCode.BAD_REQUEST));
        callback = null;
    }

    public void OpenPopupRestartError(GameErrorInfo gameErrorInfo)
    {
        StateManager.Instance.StopStateTransition();
        StateManager.Instance.ActiveBlockScreen(false);
        MessageBoxState.Open(gameErrorInfo, () =>
        {
            GameManager.Instance.Restart();
        });
    }

    private void PostResponse<T>(string responseText, Action<ApiResult<T>> callback)
    {
        ApiResult<T> apiResult = JsonUtility.FromJson<ApiResult<T>>(responseText);
        Debug.Log(API_LOG_PREFIX + "isSuccess  : " + apiResult.isSuccess);
        Debug.Log(API_LOG_PREFIX + "result : " + apiResult.result);
        if(!apiResult.isSuccess)
            Debug.Log(API_LOG_PREFIX + "errorInfo : " + apiResult.errorInfo.ToString());
        if (apiResult.isSuccess && apiResult.user!=null && apiResult.user.id > 0)
        {
            DataManager.Instance.UserInfo.SetUserData(apiResult.user);
        }
        callback(apiResult);
    }
#if UNITY_EDITOR
    public void ClearDatabaseCache()
    {
        StartCoroutine(GetRequest<bool>("/admin/refreshCache", (apiResult) => {
            
        }));
    }

    public void UploadTheme(PlatformType platformType, AssetThemeInfo assetThemeInfo, Byte[] aseetFileByte, Action<ApiResult<bool>> callback)
    {
        StartCoroutine(UploadThemeCor("/admin/uploadTheme/", platformType, assetThemeInfo.theme.id, assetThemeInfo, aseetFileByte,  callback));
    }
    public void UploadCollection(PlatformType platformType, AssetCollectionInfo assetCollectionInfo, Byte[] aseetFileByte, Action<ApiResult<bool>> callback)
    {
        StartCoroutine(UploadThemeCor("/admin/uploadCollection/", platformType, assetCollectionInfo.collectionId, assetCollectionInfo, aseetFileByte, callback));
    }
    public IEnumerator UploadThemeCor(string apiPath, PlatformType platformType, string fileName ,object assetThemeInfo, Byte[] aseetFileByte, Action<ApiResult<bool>> callback)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormFileSection("assetFile", aseetFileByte, fileName, "multipart/form-data"));
        formData.Add(new MultipartFormDataSection("assetInfo", System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(assetThemeInfo)), GameConstants.API_CONTENT_TYPE_JSON));

        using (UnityWebRequest webRequest = UnityWebRequest.Post(API_SERVER_URL + apiPath + (int)platformType, formData))
        {
            webRequest.SetRequestHeader(GameConstants.API_HEADER_AUTHORIZATION, GameConstants.API_TOKEN_PREFIX + _apiToken.token);
            
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(API_LOG_PREFIX + " Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(API_LOG_PREFIX + ":\nReceived: " + webRequest.downloadHandler.text);
                ApiResult<bool> apiResult = JsonUtility.FromJson<ApiResult<bool>>(webRequest.downloadHandler.text);
                callback(apiResult);
            }
        }

    }
#endif
#endregion

    #region API
    /// <summary>
    /// 로그인 및 유저생성
    /// </summary>
    /// <param name="authLoginForm"></param>
    /// <param name="callback"></param>
    public void AuthLogin(AuthLoginForm authLoginForm, Action<ApiResult<ApiToken>> callback)
    {
        StartCoroutine(PostRequest<ApiToken>("/auth/login", authLoginForm, (apiResult)=> {

            if (apiResult.isSuccess)
            {
                Debug.Log(API_LOG_PREFIX + "token : " + apiResult.result.token);
                SetApiToken(apiResult.result.token);
            }

            callback(apiResult);

        }));
    }

    public void CheckLogin(AuthLoginForm authLoginForm, Action<ApiResult<bool>> callback)
    {
        StartCoroutine(PostRequest<bool>("/auth/checkLogin", authLoginForm, (apiResult) => {
            callback(apiResult);
        }));
    }

    /// <summary>
    /// 게임 기본정보들 
    /// </summary>
    /// <param name="callback"></param>
    public void GetAppInfo(Action<ApiResult<AppInfo>> callback)
    {
        StartCoroutine(GetRequest<AppInfo>("/auth/appInfo",  (apiResult) => {
            if (apiResult.isSuccess)
            {
                callback(apiResult);
            }
            else
            {
                OpenPopupRestartError(apiResult.errorInfo);
            }
        }));
    }

    /// <summary>
    /// Assetbundle 리스트 및 테마정보 리스트
    /// </summary>
    /// <param name="callback"></param>
    public void GetAssetBundleData(Action<ApiResult<AssetBundleData>> callback)
    {
        PlatformType platformType = PlatformType.ANDROID;
#if UNITY_IOS
        platformType = PlatformType.IOS;
#endif

        StartCoroutine(GetRequest<AssetBundleData>("/asset/getAssetBundle/" + (int)platformType, (apiResult)=> {
            if (apiResult.isSuccess)
            {
                callback(apiResult);
            }
            else
            {
                callback = null;
                OpenPopupRestartError(apiResult.errorInfo);
            }
        }));
    }

    /// <summary>
    /// 게임 마스터 데이터
    /// </summary>
    /// <param name="callback"></param>
    public void GetGameDataInfo(Action<ApiResult<GameDataInfo>> callback)
    {
        StartCoroutine(GetRequest<GameDataInfo>("/data/info", (apiResult) => {

            if (apiResult.isSuccess)
            {  
                callback(apiResult);
            }
            else
            {
                OpenPopupRestartError(apiResult.errorInfo);
                callback(apiResult);
                callback = null;
            }

        }));
    }

    /// <summary>
    /// 유저 기본 정보
    /// </summary>
    /// <param name="callback"></param>
    public void GetUserInfo(Action<ApiResult<ApiUserInfo>> callback)
    {
        StartCoroutine(GetRequest<ApiUserInfo>("/user/getUserInfo", (apiResult) => {
            callback(apiResult);
        }));
    }

    /// <summary>
    /// 생년월일 등록.
    /// </summary>
    /// <param name="birthDay"></param>
    /// <param name="callback"></param>
    public void PostBirthDay(string birthDay, Action<ApiResult<bool>> callback)
    {
        StartCoroutine(PostRequest<bool>("/user/birthday/"+ birthDay, null, (apiResult) => {
           
            callback(apiResult);

        }));
    }

    /// <summary>
    /// 유저 테마 정보 리스트
    /// </summary>
    /// <param name="themeId"></param>
    /// <param name="callback"></param>
    public void GetUserThemeList(Action<ApiResult<List<UserThemeInfo>>> callback)
    {
        if (_userThemeCach.IsCach)
        {
            callback(new ApiResult<List<UserThemeInfo>>(true, _userThemeCach.List));
        }
        else
        {
            StartCoroutine(GetRequest<List<UserThemeInfo>>("/theme/getUserThemeList", (apiResult) => {

                if (apiResult.isSuccess)
                {

                    List<UserThemeInfo> userThemeInfos = new List<UserThemeInfo>();
                    List<ThemeInfo> themeInfos = DataManager.Instance.ThemeInfoList;

                    for (int i = 0; i < themeInfos.Count; i++)
                    {
                        userThemeInfos.Add(apiResult.result.Find(ut => ut.themeId.Equals(themeInfos[i].id)));
                    }

                    apiResult.result = userThemeInfos;

                    _userThemeCach.SetCach(userThemeInfos);                    
                }
                callback(apiResult);

            }));
        }        
    }

    /// <summary>
    ///  유저 스테이지 정보 리스트
    /// </summary>
    /// <param name="themeId"></param>
    /// <param name="callback"></param>
    public void GetUserStageList(string themeId, Action<ApiResult<List<UserStageInfo>>> callback)
    {

        if (_userStageCachDic.IsCach(themeId))
        {
            callback(new ApiResult<List<UserStageInfo>>(true, _userStageCachDic.GetCachList(themeId)));
            return;
        }

        StartCoroutine(GetRequest<List<UserStageInfo>>("/stage/getUserStageList/" + themeId, (apiResult) => {

            if (apiResult.isSuccess)
            {
                _userStageCachDic.SetCach(themeId, apiResult.result);
            }

            callback(apiResult);

        }));
    }

    /// <summary>
    /// 스테이지 시작 요청 (하트 차감, 아이템 차감)
    /// return requestId 발급.
    /// </summary>
    /// <param name="reqeustStagePlay"></param>
    /// <param name="callback"></param>
    public void RequestPlayInfo(ReqeustStagePlay reqeustStagePlay, Action<ApiResult<int>> callback)
    {
        StartCoroutine(PostRequest<int>("/stage/requestPlayInfo", reqeustStagePlay, (apiResult) => {

            if (apiResult.isSuccess)
            {
                Debug.Log("RequestPlayInfo Id : " + apiResult.result);
            }

            callback(apiResult);

        }));
    }

    /// <summary>
    /// 게임 결과 보내기.
    /// </summary>
    /// <param name="stagePlayResult"></param>
    /// <param name="callback"></param>
    public void SendPlayResult(StagePlayResult stagePlayResult, Action<ApiResult<StagePlayReward>> callback)
    {
        StartCoroutine(PostRequest<StagePlayReward>("/stage/sendPlayResult", stagePlayResult, (apiResult) => {

            callback(apiResult);

            _userThemeCach.UnSetCach();
            _userStageCachDic.UnSetAllCach();

        }));
    }

    /// <summary>
    /// 인게임 아이템 사용.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="userStageSeq"></param>
    /// <param name="callback"></param>
    public void UseIngameItem(string itemId, int userStageSeq, Action<ApiResult<UserItemInfo>> callback)
    {
        StartCoroutine(PostRequest<UserItemInfo>("/user/useItem/" + itemId + "/" + userStageSeq, null, (apiResult) => {

            if (apiResult.isSuccess)
            {
                DataManager.Instance.UserInfo.UpdateItem(apiResult.result);
            }

            callback(apiResult);

        }));
    }

    #region 상점 & 상품구매.
    private void BuyProduct(string shopId, string productId, Action<ApiResult<List<ProductItemInfo>>> callback, string purchaseToken="")
    {
        RequestBuyInfo requestBuyInfo = new RequestBuyInfo();
        requestBuyInfo.platformType = SettingManager.Instance.PlatformType;
        requestBuyInfo.shopId = shopId;
        requestBuyInfo.productId = productId;
        requestBuyInfo.purchaseToken = purchaseToken;

        StartCoroutine(PostRequest<List<ProductItemInfo>>("/shop/buyProduct", requestBuyInfo, (apiResult) =>
        {
            callback(apiResult);
        }));
    }

    public void BuyByGold(String shopId, string productId, Action<ApiResult<List<ProductItemInfo>>> callback)
    {
        BuyProduct(shopId, productId, callback);
    }

    public void BuyByCash(String shopId, string productId, string purchaseToken, Action<ApiResult<List<ProductItemInfo>>> callback)
    {
        BuyProduct(shopId, productId, callback, purchaseToken);
    }

    public void BuyTimeBonus(Action<ApiResult<List<ProductItemInfo>>> callback)
    {
        BuyProduct(string.Empty, ShopConstants.PRODUCTID_TIME_BONUS, callback);
    }

    public void ValifyJpBuyAmountLimit(Action<ApiResult<bool>> callback)
    {
        StartCoroutine(PostRequest<bool>("/shop/valifyJpBuyAmountLimit", null, (apiResult) => {

            callback(apiResult);

        }));
    }

    #endregion

    /// <summary>
    /// 스핀티켓으로 스핀 돌리기.
    /// </summary>
    /// <param name="callback"></param>
    public void GetLuckySpinResult(Action<ApiResult<SpinBoardResult>> callback)
    {
        StartCoroutine(GetRequest<SpinBoardResult>("/user/luckySpin", (apiResult) => {

            callback(apiResult);

        }));
    }

#endregion

}
