using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserCurrencyBar : MonoBehaviour
{
    [Header("User Status")]
    [SerializeField] UserHeartCtrl _userHeartCtrl;
    [SerializeField] TMP_Text _goldText;
    [SerializeField] TMP_Text _starcoinText;

    [Header("Dummy")]
    [SerializeField] Text _userNameText;

    private void OnEnable()
    {
        Debug.Log("UserCurrencyBar OnEnable");
        UpdateCurrency(null);
        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);
    }

    private void OnDisable()
    {        
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);
    } 

    public void UpdateCurrency(object[] args)
    {
        UserInfo userInfo = DataManager.Instance.UserInfo;
        if (userInfo.Id == 0) return;

        int timeSinceUpdateUser = (int)(Time.realtimeSinceStartup - userInfo.LastUpdateSeconds);

        int heartRemainSeconds  = userInfo.HeartRemainSeconds - timeSinceUpdateUser;
        int heartUnLimitSeconds = userInfo.HeartUnLimitSeconds- timeSinceUpdateUser;

        if (BuildManager.Instance.IsDevBuild())
        {
            _userNameText.text = userInfo.Id + "-" + userInfo.Name;
        }
        else
        {
            _userNameText.text = string.Empty;
        }

        _userHeartCtrl.SetHeartInfo(userInfo.Heart, heartRemainSeconds, heartUnLimitSeconds);
        _goldText.text = SystemUtil.GetCommaText(userInfo.Gold);
        if (_starcoinText) _starcoinText.text = userInfo.StarCoin.ToString();
    }

    #region Event
    public void OnOpenHeartShop()
    {
        ShopState.Open(ShopMainType.HEART);
    }

    public void OnOpenGoldShop()
    {
        ShopState.Open(ShopMainType.GOLD);
    }

    public void OnSetting()
    {
        SettingsState.Open();
    }
    #endregion

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

}
