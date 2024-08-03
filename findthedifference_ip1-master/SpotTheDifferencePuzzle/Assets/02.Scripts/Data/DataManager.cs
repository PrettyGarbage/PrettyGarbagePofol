
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct CollectionListInfo
{
    public string id;
    public bool isOpen;

    public CollectionListInfo(string id, bool isOpen)
    {
        this.id = id;
        this.isOpen = isOpen;
    }
}

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;

    [SerializeField]
    private PuzzleDataBase _puzzleDataBase;

    [SerializeField]
    private GameDataInfo _gameDataInfo;

    [SerializeField]
    private AssetBundleData _assetBundleData;
    //총 리스트.
    private List<AssetBundleInfo> _assetBundleInfoList; 

    private Dictionary<string, Sprite> _collectionThumbList = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> _collectionImageList = new Dictionary<string, Sprite>();
    
    private AppInfo _appInfo;

    private List<CollectionListInfo> _unlockCollectionIdList;

    private string _unlockthemeId;

    private UserInfo _userInfo;
    public UserInfo UserInfo {
        get {
            if (_userInfo == null)
            {
                _userInfo = new UserInfo();
                _userInfo.SaveUserThemeDifficulty((ThemeDifficulty)PlayerPrefs.GetInt(GameConstants.DATA_USERTHEMEDIFFICULTY, (int)ThemeDifficulty.THEME_BASIC));
            }
            return _userInfo;
        }
    }

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<DataManager>("DataManager");
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public AppInfo AppInfo { get { return _appInfo; } }
    public List<ThemeInfo> ThemeInfoList { get { return _gameDataInfo.themeList; } }
    public List<StageInfo> StageInfoList { get { return _gameDataInfo.stageList; } }
    public List<ItemInfo> ItemInfoList { get { return _gameDataInfo.itemList; } }
    public List<BaseScoreInfo> BaseScoreInfoList { get { return _gameDataInfo.baseScoreList; } }
    public List<ShopInfo> ShopInfoList { get { return _gameDataInfo.shopList; } }

    public List<ProductInfo> ProductInfoList { get { return _gameDataInfo.productList; } }
    public List<CollectionInfo> CollectionList { get { return _gameDataInfo.collectiontList; } }
    public DailyRewardInfo DailyRewardInfo { get { return _gameDataInfo.dailyReward;        } set { _gameDataInfo.dailyReward = value; }  }
    public List<string> LimitProductIdList { get { return _gameDataInfo.limitProductIdList; } }
    public List<RewardAdInfo> RewardAdList { get { return _gameDataInfo.rewardAdList; } }

    public AssetBundleData AssetBundleData { get { return _assetBundleData; } }
    public List<AssetBundleInfo> AssetBundleInfoList { get { return _assetBundleInfoList; } }

    public IEnumerator Init()
    {
        _unlockCollectionIdList = new List<CollectionListInfo>();
        yield return null;
    }

    public void SetAppInfo()
    {
        ApiManager.Instance.GetAppInfo(ApiResult =>
        {
            _appInfo = ApiResult.result;
            Debug.Log(_appInfo.ToString());
        });
    }

    //초기 데이터 세팅.
    public void SetGameData(GameDataInfo gameDataInfo)
    {
        _gameDataInfo = gameDataInfo;
    }

    public void SetThemeDataInitailize()
    {
        int stageNo = 0;
        for (int i = 0; i < ThemeInfoList.Count; i++)
        {
            ThemeInfo themeInfo = ThemeInfoList[i];
            List<StageInfo> stageList = GetStageInfoList(themeInfo.id);
            themeInfo.stageCount = stageList.Count;
            themeInfo.ThemeMainImage = AssetBundleManager.Instance.GetAsset<Sprite>(themeInfo.id, themeInfo.id +GameConstants.ASSETBUNDLE_THUMB);

            for (int j = 0; j < stageList.Count; j++)
            {
                stageList[j].stageNo = ++stageNo;
            }
        }
    }

    public void LoadGameDataInfo(Action callback)
    {
        StateManager.Instance.ShowSpinner();
        ApiManager.Instance.GetGameDataInfo((apiResult) =>
        {
            if (apiResult.isSuccess)
            {
                SetGameData(apiResult.result);
                NetworkManager.Instance.PreLoadRewardAd();
                IAPManager.Instance.InitializePurchasing(DataManager.Instance.GetCashProductInfoList(), () => {
                    StateManager.Instance.HideSpinner();
                    callback();
                });
            }
            else
            {
                StateManager.Instance.HideSpinner();
            }
        });
    }


    #region Asset bundle
    public void SetAssetBundleData(AssetBundleData assetBundleData)
    {
        _assetBundleData = assetBundleData;
        _assetBundleInfoList = new List<AssetBundleInfo>();
        _assetBundleData.themeAssetBundle.ForEach(assetBundleInfo =>
        {
            assetBundleInfo.assetBundleType = AssetBundleType.THEME;
            _assetBundleInfoList.Add(assetBundleInfo);
        });
        _assetBundleData.collectionAssetBundle.ForEach(assetBundleInfo =>
        {
            assetBundleInfo.assetBundleType = AssetBundleType.COLLECTION;
            _assetBundleInfoList.Add(assetBundleInfo);
        });
    }

    public double GetAssetBundleDownloadSize()
    {
        List<AssetBundleInfo> downloadAssetBundleInfoList = AssetBundleManager.Instance.GetUpdateDownloadList(_assetBundleInfoList);
        return AssetBundleManager.Instance.GetTotalFileMbSize(downloadAssetBundleInfoList);
    } 
    #endregion

    #region  ModeData
    public PuzzleModeData GetPuzzleModeData(ThemeDifficulty  playMode)
    {
        switch (playMode)
        {
            case ThemeDifficulty .THEME_BASIC:
                return _puzzleDataBase.ThemeBasicModeData;
            case ThemeDifficulty .THEME_MASTER:
                return _puzzleDataBase.ThemeMasterModeData;
            default:
                return _puzzleDataBase.ThemeBasicModeData;
        }
    }
    #endregion

    #region ThemeData
    public List<ThemeInfo> GetThemaInfoList()
    {
        return ThemeInfoList;
    }

    public ThemeInfo GetApiThemeInfo(string themeId)
    {
        return ThemeInfoList.Find(m => m.id.Equals(themeId));        
    }


    public PuzzleImageData GetThemaPuzzleData(string themeId, int index)
    {

        for (int i = 0; i < ThemeInfoList.Count; i++)
        {
            if (ThemeInfoList[i].id.Equals(themeId))
            {
                return ThemeInfoList[i].GetPuzzleImageData(index);
            }
        }

        Debug.LogErrorFormat("NO ThemaPuzzle Data themeId :  {0}, index : {1}", themeId, index);

        return null;
    }

    public List<StageInfo> GetStageInfoList(string themeId)
    {
        List<StageInfo> stageInfoList =  StageInfoList.FindAll(m => m.themeId.Equals(themeId));
        stageInfoList.Sort((a, b) => { return a.stageId.CompareTo(b.stageId); });
        return stageInfoList;
    }

    public ItemInfo GetItemInfo(string itemId)
    {
        return ItemInfoList.Find(item => item.id.Equals(itemId));
    }
    #endregion

    #region BaseScore
    public BaseScoreInfo GetBaseScoreInfo(ThemeDifficulty themeDifficulty)
    {
        return BaseScoreInfoList.Find(s => s.themeDifficulty == themeDifficulty);
    }
    #endregion

    #region Shop & Product
    public List<ShopInfo> GetShopInfoList(String shopId)
    {
        return ShopInfoList.FindAll(s => s.id.Equals(shopId)); ;
    }

    public ShopInfo GetShopInfo(String shopId, String productId)
    {
        ShopInfo shopInfo = null;
        List<ShopInfo> shopInfos = ShopInfoList.FindAll(s => s.id.Equals(shopId));
        if (shopInfos != null)
        {
            return shopInfos.Find(s => s.productId.Equals(productId));
        }

        return shopInfo;
    }

    public ProductInfo GetProductInfo(String productId)
    {
        return ProductInfoList.Find(p => p.id.Equals(productId));
    }

    public List<ProductInfo> GetCashProductInfoList()
    {
        return ProductInfoList.FindAll(p => p.paymentType == PaymentType.CASH);
    }

    public List<ShopInfo> GetShopInfoList(ShopMainType shopMainType)
    {
        switch (shopMainType)
        {
            case ShopMainType.GOLD:
                return GetShopInfoList(ShopConstants.SHOPID_CURRENCY_GOLD);
            case ShopMainType.HEART:
                return GetShopInfoList(ShopConstants.SHOPID_CURRENCY_HEART);
            case ShopMainType.EVENT:
                return GetShopInfoList(ShopConstants.SHOPID_CURRENCY_EVENT);
            default:
                return null;
        }
    }

    public ProductInfo GetProductInfoByStoreId(string storeProductId)
    {
        return _gameDataInfo.productList.Find(p => p.GetStoreProductId().Equals(storeProductId));
    }

    public ShopInfo GetFirstEventProduct()
    {
        List<ShopInfo> shopInfos = GetShopInfoList(ShopMainType.EVENT);
        for (int i = 0; i < shopInfos.Count; i++)
        {
            if (!IsLimitProduct(shopInfos[i].productId))
            {
                return shopInfos[i];
            }
        }
        return null;
    }

    public List<ShopInfo> GetShopInfoList(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.ONE_MISS_DEFENCE:
                return GetShopInfoList(ShopConstants.SHOPID_ITEM_ONE_MISS_DEFENCE);
            case ItemType.BONUS_SCORE:
                return GetShopInfoList(ShopConstants.SHOPID_ITEM_BOUNS_SCORE);
            case ItemType.HINT:
                return GetShopInfoList(ShopConstants.SHOPID_ITEM_HINT);
            case ItemType.FREEZE_TIME:
                return GetShopInfoList(ShopConstants.SHOPID_ITEM_FREEZE_TIME);
            case ItemType.EXTRA_TIME:
                return GetShopInfoList(ShopConstants.SHOPID_ITEM_EXTRA_TIME);
            default:
                return null;
        }
    }

    public bool IsLimitProduct(string productId)
    {
        return LimitProductIdList != null && LimitProductIdList.Exists(l => l.Equals(productId));
    }
    #endregion

    #region Collection
    public void AddUnlockCollectionId(CollectionListInfo collectionInfo)
    {
        _unlockCollectionIdList.Add(collectionInfo);
    }

    public void RemoveUnlockCollectionId(string collectionId)
    {
        for (int i = 0; i < _unlockCollectionIdList.Count; i++)
        {
            if (_unlockCollectionIdList[i].id.Equals(collectionId))
            {
                _unlockCollectionIdList.Remove(_unlockCollectionIdList[i]);
            }
        }

    }

    public List<CollectionListInfo> GetUnlockCollectionListInfos()
    {
        return _unlockCollectionIdList;
    }

    public List<CollectionInfo> GetCollectionInfoList()
    {
        return CollectionList;
    }

    public bool isLoadedCollectionBundle()
    {
        return _collectionThumbList.Count > 0;
    }

    IEnumerator GetCollectionSpriteAsync(string collectionId, string assetName, Action<Sprite> calback)
    {
        Sprite sprite = null; 
        while (sprite == null)
        {
            sprite = AssetBundleManager.Instance.GetAsset<Sprite>(collectionId, assetName);
            yield return CommonConstants.WaitLoopSeconds;
        }

        calback(sprite);
    }

    public void GetCollectionThumb(string collectionId, Action<Sprite> calback)
    {
        if (_collectionThumbList.ContainsKey(collectionId))
        {
            calback( _collectionThumbList[collectionId] );
        }
        else
        {
            StartCoroutine( GetCollectionSpriteAsync(collectionId, collectionId + GameConstants.ASSETBUNDLE_THUMB, (sprite)=> {
                _collectionThumbList.Add(collectionId, sprite);
                calback(sprite);
            }));            
        }
    }

    public void GetCollectionImage(string collectionId, Action<Sprite> calback)
    {
        if (_collectionImageList.ContainsKey(collectionId))
        {
            calback(_collectionImageList[collectionId]);
        }
        else
        {
            StartCoroutine(GetCollectionSpriteAsync(collectionId, collectionId, (sprite) => {
                _collectionImageList.Add(collectionId, sprite);
                calback(sprite);
            }));
        }      
    }

    public bool IsOpenEnableCollection(string collectionId)
    {
        bool isOpenEnableCollection = false;
        bool isOwn = UserInfo.IsUserCollection(collectionId);
        if (isOwn)
        {
            foreach(CollectionListInfo cli in _unlockCollectionIdList)
            {
                if (cli.id.Equals(collectionId))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            CollectionInfo collectionInfo = CollectionList.Find(c => c.id.Equals(collectionId));
            if (collectionInfo != null)
            {
                if (collectionInfo.unlockCondition.type == UnlockConditionType.SHOP)
                {
                    string productId = collectionInfo.unlockCondition.typeId;
                    ProductInfo p = GetProductInfo(productId);
                    return p.GetBuyEnableCode() == ApiErrorCode.NONE;
                }
            }
        }

        return isOpenEnableCollection;
    }

    public bool IsExistOpenEnableCollection()
    {
        for (int i = 0; i < CollectionList.Count; i++)
        {
            if (IsOpenEnableCollection(CollectionList[i].id))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region UnlockTheme
    public void AddUnlockThemeId(string unlockId)
    {
        _unlockthemeId = unlockId;
    }

    public void RemoveUnlockThemeId()
    {
        _unlockthemeId = string.Empty;
    }
    public string GetUnlockThemeId()
    {
        return _unlockthemeId;
    }
    #endregion

    #region Daily Reward
    public bool IsExistDailyReward()
    {
        return DailyRewardInfo != null && !DailyRewardInfo.id.Equals(string.Empty);
    }

    public DailyRewardInfo GetDailyRewardInfo()
    {
        return DailyRewardInfo;
    }
    public void ClearDailyRewardInfo()
    {
        DailyRewardInfo = null;
    }
    #endregion

    #region reward ad
    public bool IsEnableRewardAd(string adName)
    {
        RewardAdInfo rewardAdInfo = GetRewardAdInfo(adName);
        return rewardAdInfo != null && (rewardAdInfo.dailyLimit == 0 || rewardAdInfo.dailyCount < rewardAdInfo.dailyLimit);
    }

    public void AddRewardAdCount(string adName)
    {
        RewardAdInfo rewardAdInfo = GetRewardAdInfo(adName);
        if (rewardAdInfo != null && rewardAdInfo.dailyLimit != 0)
        {
            rewardAdInfo.dailyCount++;
        }
    }

    public RewardAdInfo GetRewardAdInfo(string adName)
    {
        string adUnitId = NetworkManager.Instance.GetADUnitId(AdType.REWARD, adName);
        Debug.Log("IsEnableRewardAd : " + adUnitId);
        adUnitId = adUnitId.Split('/')[1];
        Debug.Log("IsEnableRewardAd : " + adUnitId);
        return DataManager.Instance.RewardAdList.Find(ad => ad.adUnit.Equals(adUnitId));
    } 
    #endregion
}