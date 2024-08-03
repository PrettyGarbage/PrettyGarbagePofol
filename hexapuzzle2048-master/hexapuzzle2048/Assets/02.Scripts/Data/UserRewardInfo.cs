using System;

public class UserRewardInfo
{
    private string _dailyRewardDay = "";
    private string _coinBoxRewardDay = "";
    private int _coinBoxRewardCount = 0;
    private long _cointBoxRewardTimeStamp = 0;

    private int _missionIndex = 0;

    public UserRewardInfo() { }

    public UserRewardInfo(string dailyRewardDay, string coinBoxRewardDay, int coinBoxRewardCount, long cointBoxRewardTimeStamp)
    {
        _dailyRewardDay = dailyRewardDay;
        _coinBoxRewardDay = coinBoxRewardDay;
        _coinBoxRewardCount = coinBoxRewardCount;
        _cointBoxRewardTimeStamp = cointBoxRewardTimeStamp;

    }

    public string DailyRewardDay { get { return _dailyRewardDay; } }

    public string CoinBoxRewardDay { get { return _coinBoxRewardDay; } }

    public int CoinBoxRewardCount { get { return _coinBoxRewardCount; } }

    public long CointBoxRewardTimeStamp { get { return _cointBoxRewardTimeStamp; } }

    public bool IsGetDailyReward()
    {
        Debug.Log("_dailyRewardDay : " + _dailyRewardDay);
        return _dailyRewardDay.Equals(SystemUtil.GetNowDayFormat());
    }

    public void SetDailyReward()
    {
        _dailyRewardDay = SystemUtil.GetNowDayFormat();
    }

    public void SetCoinBox()
    {
        string yyyyMMdd = SystemUtil.GetNowDayFormat();
        if (!_coinBoxRewardDay.Equals(yyyyMMdd))
        {
            _coinBoxRewardDay = yyyyMMdd;
            _coinBoxRewardCount = 0;
        }

        Debug.Log(" GetCoinBoxCoolTimeSeconds : " + GetCoinBoxCoolTimeSeconds());
    }

    public bool AddCoinBoxReward()
    {
        if (DataManager.Instance.GameData.CoinBoxDailyLimit > _coinBoxRewardCount && GetCoinBoxCoolTimeSeconds() <= 0)
        {
            _coinBoxRewardCount++;
            _cointBoxRewardTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return true;
        }
        return false;
    }

    public long GetCoinBoxCoolTimeSeconds(){
        long remainSeconds;
        if(_cointBoxRewardTimeStamp==0){
            remainSeconds = DataManager.Instance.GameData.CoinBoxCoolTimeSeconds ;
        }else {
            remainSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _cointBoxRewardTimeStamp;
        } 

        if(remainSeconds > 0){
            return DataManager.Instance.GameData.CoinBoxCoolTimeSeconds - remainSeconds;
        }else {
            return DataManager.Instance.GameData.CoinBoxCoolTimeSeconds;
        }
    }
}