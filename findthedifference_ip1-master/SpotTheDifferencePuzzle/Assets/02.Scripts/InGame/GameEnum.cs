public enum ApiCachDataType
{
    THEME, STAGE
}

public enum ImageDifficulty
{
    EASY, NORMAL, HARD
}

public enum ThemeDifficulty 
{
    THEME_BASIC, THEME_MASTER
}

public enum FindMixRule
{
    easy_nomal = 12
     , nomal_hard = 23
      , easy_hard = 13
      , easy_nomal_hard = 123
}

public enum CurtainDirection
{
    LEFT = -1,
    RIGHT = 1,
}

//
public enum LoginType
{
    NONE=0, GOOGLE=1, FACEBOOK=2, TWITTER=3
}


public enum ItemType
{
    SPIN_TICKET = 0, BONUS_TIME = 1, ONE_MISS_DEFENCE = 2, BONUS_SCORE = 3 , HINT= 4, FREEZE_TIME = 5, EXTRA_TIME = 6, UNLIMIT_HEART_30MIN = 7
}

public enum ItemUseType
{
    ACTIVE =1 , PASSIVE=2
}

public enum ProductType
{
    ITEM=0, CURRENCY=1, COLLECTION=2
}

public enum UnlockConditionType
{
    NONE=0, SHOP=1, CLEAR_THEME_BASIC=2, CLEAR_THEME_MASTER=3
}

public enum CollectionGradeType
{
    NONE = -1,  NORMAL = 0, RARE, EPIC, LEGEND
}

public enum CurrencyType
{
    GOLD = 0, STARCOIN=1, HEART=2, FREE_GOLD=3
}

public enum PaymentType
{
    CASH = 0, GOLD = 1, STARCOIN = 2
}

public enum StateType
{
    SCENE, POPUP
}

public enum DailyRewardType
{
    OPEN_EVENT=0, NORMAL=1
}

public enum SPLASH
{
    READY = 0,
    GO,
    CLEAR,
    GAMEOVER,
    BONUSTIME,
    HURRYUP,
};

public enum PopupInstantType
{
    THEME_OPEN_MASTER
    ,THEME_COMPLETE
    ,THEME_UNLOCK
    ,COLLECTION_UNLOCK
    ,PURCHSE_SUCCESS
}

public enum ShopType
{
    MAIN, ITEM
}

public enum ShopMainType
{
    GOLD, HEART, EVENT
}

public enum AssetBundleType
{
    THEME, COLLECTION
}

public enum TestOption
{
    OPTIONA,
    OPTIONB
}
