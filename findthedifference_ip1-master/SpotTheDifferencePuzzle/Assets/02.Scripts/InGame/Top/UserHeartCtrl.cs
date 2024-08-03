using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserHeartCtrl : MonoBehaviour {

    [Header("Object")]
    [SerializeField]
    private GameObject _limitObject;
    [SerializeField]
    private GameObject _unlimitObject;

    [Header("Limit")]
    [SerializeField]
    private TMP_Text _heartAmountText;
    [SerializeField]
    private LocalizationUIText _heartFullText;

    [Header("Timer")]
    [SerializeField]
    private Timer _timer;
    [SerializeField]
    private TMP_Text _heartRemainTimeText;
    private int _heartRemainTime;


    private void Update()
    {
        if (!_timer.IsStop)
        {
            _heartRemainTimeText.text = _timer.getRemainTimeText();
        }
    }


    public void SetHeartInfo(int heartAmount, int heartRemainSeconds, int heartUnLimitSeconds)
    {
        bool isUnlimited = heartUnLimitSeconds > 0;
        _limitObject.SetActive(!isUnlimited);
        _unlimitObject.SetActive(isUnlimited);

        if (isUnlimited)
        {
            _heartRemainTime = heartUnLimitSeconds;
        }
        else
        {
            _heartRemainTime = heartRemainSeconds;
            _heartAmountText.text = heartAmount.ToString();
        }
        UpdateRemainHeart(isUnlimited, heartAmount, _heartRemainTime);
    }

    private void UpdateRemainHeart(bool isUnlimited, int heartAmount, int remainTime)
    {
        if (!_timer.IsStop)
        {
            _timer.StopTimer();
        }

        if (isUnlimited || (!isUnlimited && heartAmount < GameConstants.USER_HEART_MAX_COUNT))
        {
            _timer.Set(remainTime, UpdateUserInfo, true);
            _timer.StartTimer(true);
            _heartRemainTimeText.gameObject.SetActive(true);
            _heartFullText.gameObject.SetActive(false);
        }
        else
        {
            _heartRemainTimeText.gameObject.SetActive(false);
            _heartFullText.gameObject.SetActive(true);
            _timer.StopTimer();
        }
    }

    private void UpdateUserInfo()
    {
        Debug.Log("UpdateUserInfo");
        ApiManager.Instance.GetUserInfo(apiResult => {

        });
    }

}
