using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour {

	private int _score;
    private int _coin;
    private PlayData _playData;
    private UserRewardInfo _userRewardInfo;

    public UserData(int score, int coin, PlayData playData, UserRewardInfo userRewardInfo){
        _score = score;
        _coin = coin;
        _playData = playData;

        _userRewardInfo = userRewardInfo;
    }

    public int Score { get { return _score; } }

    public int Coin { get { return _coin; } }

    public PlayData PlayData { get { return _playData; } }

    public UserRewardInfo UserRewardInfo { get { return _userRewardInfo; } }

    public void SetPlayData(PlayData playData){
        _playData = playData;
    }

    public void AddCoin(int addCoin){
        _coin += addCoin;
    }

    public bool UpdateScore(int score){
        bool isUpdate = score >_score;
        if(isUpdate)
            _score = score;

        return isUpdate;
    }

    public bool IsGetDailyReward(){
        return _userRewardInfo.IsGetDailyReward();
    }

    public void SetDailyReward(){
        AddCoin(DataManager.Instance.GameData.DailyBonusCoin);
        _userRewardInfo.SetDailyReward();
    }

    public void SetCoinBox(){
        _userRewardInfo.SetCoinBox();
    }

    public void AddCoinBoxReward()
    {
        if(_userRewardInfo.AddCoinBoxReward()){
            _coin += DataManager.Instance.GameData.CoinBoxCoin;
        }        
    }
}

