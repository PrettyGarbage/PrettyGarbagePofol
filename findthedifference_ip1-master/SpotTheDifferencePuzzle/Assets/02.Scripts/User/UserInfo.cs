using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo  {

    private int _id;
    private string _name;
    private string _platformId;
    private LoginType _loginType;
    private int _paidGold;
    private int _freeGold;
    private int _starCoin;
    private long _lastLoginTimestamp;
    private List<UserItemInfo> _itemList;
    private List<UserCollectionInfo> _collectionList;

    //jp service
    private string _locale;
    private string _birthDay;

    //heart
    private int _heart;
    private int _heartRemainSeconds;
    private int _heartUnLimitSeconds;

    //업데이트 된 시간.
    private float _lastUpdateSeconds;

    private ThemeDifficulty _selectThemeDifficulty = ThemeDifficulty.THEME_BASIC;

    public int Id { get { return _id; } }
    public string Name { get { return _name; } }
    public LoginType LoginType { get { return _loginType; } }
    public string PlatformId { get { return _platformId; } }
    public int Gold { get { return _paidGold + _freeGold; } }
    public int PaidGold { get { return _paidGold; } }
    public int FreeGold { get { return _freeGold; } }
    public int StarCoin { get { return _starCoin; } }
    public int Heart { get { return _heart; } }
    public string Locale { get { return _locale; } }
    public string BirthDay { get { return _birthDay; } }
    public long LastLoginTimestamp { get { return _lastLoginTimestamp; } }
    public List<UserItemInfo> ItemList { get { return _itemList; } }
    public List<UserCollectionInfo> CollectionList { get { return _collectionList; } }
    public int HeartRemainSeconds { get { return _heartRemainSeconds; } }
    public int HeartUnLimitSeconds { get { return _heartUnLimitSeconds; } }

    public float LastUpdateSeconds { get { return _lastUpdateSeconds; } }
    public ThemeDifficulty SelectThemeDifficulty { get { return _selectThemeDifficulty; } }

    private int[] _items = new int[6];

    public static bool IsLogined()
    {
        return ApiManager.Instance.ApiToken.token != string.Empty;
    }

    //유저 데이터 갱신.
    public void SetUserData(ApiUserInfo apiUserInfo){
        _id = apiUserInfo.id;
        _name = apiUserInfo.name;
        _loginType = apiUserInfo.loginType;
        _platformId = apiUserInfo.platformId;
        _paidGold = apiUserInfo.paidGold;
        _freeGold = apiUserInfo.freeGold;
        _starCoin = apiUserInfo.starCoin;
        _heart = apiUserInfo.heart;
        _lastLoginTimestamp = apiUserInfo.lastLoginTimestamp;
        _heartRemainSeconds = apiUserInfo.remainHeartChargeSeconds;
        _heartUnLimitSeconds = apiUserInfo.remainUnLimitHeartSeconds;

        _itemList = apiUserInfo.itemList;
        _collectionList = apiUserInfo.collectionList;

        _lastUpdateSeconds = Time.realtimeSinceStartup;

        _locale = apiUserInfo.locale;
        _birthDay = apiUserInfo.birthDay;

        EventPool.Send(EventNames.ON_UPDATE_USERINFO);
    }

    public void UpdateItem(UserItemInfo userItemInfo)
    {
        if (_itemList == null)
        {
            _itemList = new List<UserItemInfo>();
            _itemList.Add(userItemInfo);
        }
        else
        {
            UserItemInfo ownUserItemInfo = _itemList.Find(i => i.itemId.Equals(userItemInfo.itemId));

            Debug.Log("UpdateItem : " + ownUserItemInfo.ToString());

            if (ownUserItemInfo == null)
            {
                _itemList.Add(ownUserItemInfo);
            }
            else
            {
                ownUserItemInfo.count = userItemInfo.count;
            }
        }

        EventPool.Send(EventNames.ON_UPDATE_USERINFO);
    }

    public UserItemInfo GetItem(ItemType itemType)
    {
        return ItemList ==null ? null : ItemList.Find(i => i.itemId.Equals(itemType.ToString()));
    }

    public int GetItemCount(ItemType itemType)
    {
        UserItemInfo userItemInfo = GetItem(itemType);
        return userItemInfo != null ? userItemInfo.count : 0;
    }

    public bool IsUserCollection(string collectionId)
    {
        return _collectionList.Exists(c => c.collectionId.Equals(collectionId));
    }

    public void SaveUserThemeDifficulty(ThemeDifficulty themeDifficulty)
    {
        PlayerPrefs.SetInt(GameConstants.DATA_USERTHEMEDIFFICULTY, (int)themeDifficulty);
        _selectThemeDifficulty = themeDifficulty;
    }

}
