using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PopupDailyRewardState : PopupState
{
	[Header("PopupDailyReward")]
	[SerializeField]
    private DailyReward _pfDailyReward;
    [SerializeField]
    private Transform _contentLayout01;
    [SerializeField]
    private Transform _contentLayout02;
    [SerializeField]
    private GameObject _subTitleObject;

    private RewardInfo[] _rewardInfos;
    private DailyRewardInfo _dailyRewardInfo;

    public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupDailyRewardState));
    }

    public static void Close()
    {
        StateManager.Instance.GetRegisteredState(typeof(PopupDailyRewardState)).OnBack();
       
    }
	
	public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);
        SetRewardInfo();
        _subTitleObject.SetActive(_dailyRewardInfo.type == DailyRewardType.OPEN_EVENT);
    }

    public override IEnumerator OnPostOpen()
    {
        CreateDailyRewards();
        return base.OnPostOpen();       
    }

    private void SetRewardInfo()
    {
        _dailyRewardInfo = DataManager.Instance.GetDailyRewardInfo();
        _rewardInfos = new RewardInfo[GameConstants.DAILY_REWARD_ITEMCOUNT];
        _rewardInfos[0] = _dailyRewardInfo.reward01;
        _rewardInfos[1] = _dailyRewardInfo.reward02;
        _rewardInfos[2] = _dailyRewardInfo.reward03;
        _rewardInfos[3] = _dailyRewardInfo.reward04;
        _rewardInfos[4] = _dailyRewardInfo.reward05;
        _rewardInfos[5] = _dailyRewardInfo.reward06;
        _rewardInfos[6] = _dailyRewardInfo.reward07;
    }

    private  void CreateDailyRewards()
    {
        for (int i = 0; i < GameConstants.DAILY_REWARD_ITEMCOUNT; i++)
        {
            GameObject go = ObjectUtil.InstantiateAtTarget(_pfDailyReward.gameObject, i < 4 ? _contentLayout01 : _contentLayout02) as GameObject;
            DailyReward dailyReward = go.GetComponent<DailyReward>();
            int itemSeq = i + 1;
            dailyReward.SetDailyReward(_rewardInfos[i], itemSeq, _dailyRewardInfo.itemSeq >= itemSeq, _dailyRewardInfo.itemSeq == itemSeq);
        }
    }

    public override void OnBack()
    {
        base.OnBack();
        DataManager.Instance.ClearDailyRewardInfo();
        _dailyRewardInfo = null;
        _rewardInfos = null;

    }

}