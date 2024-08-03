public class MissionInfo
{
    private int _dayBestScore;
    private int _breakScore;
    private int _rewardCoin;

    public MissionInfo(int dayBestScore, int breakScore, int rewardCoin)
    {
        _dayBestScore = dayBestScore;
        _breakScore = breakScore;
        _rewardCoin = rewardCoin;
    }

    public int DayBestScore { get { return _dayBestScore; } }

    public int BreakScore { get { return _breakScore; } }

    public int RewardCoin { get { return _rewardCoin; } }

    public void Print(){
        Debug.LogFormat("DayBestScore: {0}, BreakScore: {1}, RewardCoin : {2}" ,DayBestScore, BreakScore, RewardCoin);
    }
}

