using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData.asset", menuName = "gbros/GameData", order = 1)]
public class GameData : ScriptableObject
{

    [SerializeField]
    private int _initUserCoin = 100;
    [SerializeField]
    private int _initItemCost = 100;

    [Header("daily")]
    [SerializeField]
    private int _dailyBonusCoin = 300;

    [Header("coinbox")]
    [SerializeField]
    private int _coinBoxCoin = 100;
    [SerializeField]
    private int _coinBoxDailyLimit = 9;
    [SerializeField]
    private int _coinBoxCoolTimeSeconds = 60 * 10;

    [Header("replay")]
    [SerializeField] int _replayAdIntervalSeconds = 60 * 10;

    [Header("mission")]
    [SerializeField]
    private int _missionRewardCoin = 100;
    [SerializeField]
    private int[] _missionRewardScores = new int[] { 60000, 120000, 180000, 240000, 300000, 450000, 600000, 1200000, 3000000, 6000000 };

    public int InitUserCoin { get { return _initUserCoin; } }
    public int InitItemCost { get { return _initItemCost; } }


    public int MissionRewardCoin { get { return _missionRewardCoin; } }
    public int[] MissionRewardScores { get { return _missionRewardScores; } }

    public int DailyBonusCoin { get { return _dailyBonusCoin; } }

    public int CoinBoxDailyLimit { get { return _coinBoxDailyLimit; } }
    public int CoinBoxCoin { get { return _coinBoxCoin; } }
    public int CoinBoxCoolTimeSeconds { get { return _coinBoxCoolTimeSeconds; } }

    public int ReplayAdIntervalSeconds { get { return _replayAdIntervalSeconds; } }
}