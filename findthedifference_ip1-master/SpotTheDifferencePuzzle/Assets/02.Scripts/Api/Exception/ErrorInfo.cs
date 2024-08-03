using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApiErrorCode
{
    NONE = -1
    ,BAD_REQUEST = 0
    , NOT_AUTHENTICATION_LOGIN = 1
    , DATA_ERROR = 2
    , LOCK_THEME = 10
    , NOT_ENOUGH_HEART = 11
    , NOT_ENOUGH_GOLD = 12
    , NOT_ENOUGH_STARCOIN = 13
    , NOT_ENOUGH_ITEMCOUNT = 14
    , ALREADY_COMPLETED = 20
    , PURCHASE_FAIL = 30

    , AD_REWARD_VERIFY_FAIL = 40
    , AD_REWARD_LIMIT = 41

    , SHOP_JP_NO_BIRTHDAY = 50
    , SHOP_JP_EXCEED_MONTH = 51
}

[System.Serializable]
public class GameErrorInfo
{

    public ApiErrorCode errorCode;
    public string errorMsg;

    public GameErrorInfo(ApiErrorCode errorCode)
    {
        this.errorCode = errorCode;
    }

    public override string ToString()
    {
        return "errorCode: " + errorCode + " | errorMsg : " + errorMsg;
    }

    public string GetErrorMessage()
    {
        return "errorCode: " + errorCode + " | errorMsg : " + errorMsg;
    }

    public LocalizationTextKey GetMessageLocalizationTextKey()
    {
        switch (errorCode)
        {
            case ApiErrorCode.BAD_REQUEST:
                return LocalizationTextKey.ERROR_BAD_REQUEST;
            case ApiErrorCode.NOT_AUTHENTICATION_LOGIN:
                return LocalizationTextKey.ERROR_NOT_AUTHENTICATION_LOGIN;
            case ApiErrorCode.DATA_ERROR:
                return LocalizationTextKey.ERROR_DATA_ERROR;
            case ApiErrorCode.LOCK_THEME:
                return LocalizationTextKey.ALERT_LOCK_THEME;
            case ApiErrorCode.NOT_ENOUGH_HEART:
                return LocalizationTextKey.ALERT_NOT_ENOUGH_HEART;
            case ApiErrorCode.NOT_ENOUGH_GOLD:
                return LocalizationTextKey.ALERT_NOT_ENOUGH_GOLD;
            case ApiErrorCode.NOT_ENOUGH_STARCOIN:
                return LocalizationTextKey.ALERT_NOT_ENOUGH_STARCOIN;
            case ApiErrorCode.NOT_ENOUGH_ITEMCOUNT:
                return LocalizationTextKey.ALERT_NOT_ENOUGH_ITEMCOUNT;
            case ApiErrorCode.ALREADY_COMPLETED:
                return LocalizationTextKey.ALERT_ALREADY_COMPLETED;
            case ApiErrorCode.PURCHASE_FAIL:
                return LocalizationTextKey.STORE_PURCHASEPOPUP_FAIL_COMMONDESC;
            case ApiErrorCode.SHOP_JP_EXCEED_MONTH:
                return LocalizationTextKey.SHOP_JP_EXCEED_MONTH;
            case ApiErrorCode.SHOP_JP_NO_BIRTHDAY:
                return LocalizationTextKey.SHOP_JP_NO_BIRTHDAY;
            case ApiErrorCode.AD_REWARD_LIMIT:
                return LocalizationTextKey.AD_REWARD_LIMIT;

            case ApiErrorCode.AD_REWARD_VERIFY_FAIL:
            default:
                return LocalizationTextKey.ERROR_BAD_REQUEST;
        }
    }

    public LocalizationTextKey GetTitleLocalizationTextKey()
    {
        switch (errorCode)
        {
            case ApiErrorCode.BAD_REQUEST:
            case ApiErrorCode.NOT_AUTHENTICATION_LOGIN:
            case ApiErrorCode.DATA_ERROR:
                return LocalizationTextKey.COMMON_ERROR_TITLE;                
            case ApiErrorCode.PURCHASE_FAIL:
                return LocalizationTextKey.STORE_PURCHASEPOPUP_FAIL_TITLE;
            case ApiErrorCode.LOCK_THEME:
            case ApiErrorCode.NOT_ENOUGH_HEART:
            case ApiErrorCode.NOT_ENOUGH_GOLD:
            case ApiErrorCode.NOT_ENOUGH_STARCOIN:
            case ApiErrorCode.NOT_ENOUGH_ITEMCOUNT:
            case ApiErrorCode.ALREADY_COMPLETED:
                return LocalizationTextKey.COMMON_ALERT_TITLE;                
            default:
                return LocalizationTextKey.COMMON_ERROR_TITLE;                
        }
    }

}
