using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThemeRoundCompletedState : NotBackablePopupState
{
	[Header("Theme Round Completed")]
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] GameObject _nextBtnObj;
    [SerializeField] GameObject _retryBtnObj;
    [SerializeField] GameObject _themeBtnObj;
    [SerializeField] GameObject _rewardObj;
    [SerializeField] Animator _rewardAnim;
    [SerializeField] GameObject _stageBtnObj;
    [SerializeField] GameObject[] _starGroup;
    [SerializeField] TMP_Text _titleText;
    [SerializeField] private Animator[] _starAnimator;

    private bool _isLastStage;
    private int _historyStarVal;

    private InGameData _inGameData;

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

        _inGameData = GameManager.Instance.InGameData;

        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(SystemUtil.GetCommaText(_inGameData.StagePlayReward.basicScore +
            _inGameData.StagePlayReward.bonusScore + _inGameData.StagePlayReward.timeBonusScore));
        _scoreText.text = stringBuilder.ToString();

        _historyStarVal = _inGameData.UserStageInfo.GetUserStageDetailInfo(_inGameData.ThemeDifficulty).userStarCoin;

        InitStar();

        _isLastStage = DataManager.Instance.GetStageInfoList(_inGameData.ThemeInfo.id).Count <= _inGameData.StageIndex+1;

        _nextBtnObj.SetActive(!_isLastStage);

        _themeBtnObj.SetActive(_isLastStage);

        _stageBtnObj.SetActive(!_isLastStage);

        _retryBtnObj.SetActive(!IsRetryBtnExpress());

        _rewardObj.SetActive(!IsGetReward());
        _rewardAnim.gameObject.SetActive(IsGetReward());

        _titleText.text = _inGameData.StageInfo.name + " CLEAR";
        
    }

    public override IEnumerator OnPostOpen()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        yield return SetAttainStarPoint();

        if (IsGetReward())
        {
            yield return new WaitForSeconds(0.5f);
            StateManager.Instance.ActiveBlockScreen(false);
            PopupRewardState.Open(_inGameData.StagePlayReward.stageReward.productItemList);
            _rewardObj.SetActive(true);
            _rewardAnim.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            StateManager.Instance.ActiveBlockScreen(false);
        }

        yield return base.OnPostOpen();
    }

    #region EVENTS
    public void OnNext()
	{
        if (GameManager.Instance.InGameData.SetNextStage())
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), true);
        }
        else
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), true);
            Debug.LogWarning("no next stage");
        }
    }

    public void OnShare()
    {
        NetworkManager.Instance.AppShare();
    }

    public void OnRetry()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), true);        
    }

    public void OnGoToStageList()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectStageState), true);
    }

    public void OnGoToThemeList()
    {
        
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), true);
    }

    public void OnClose()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectStageState), true);
    }
    #endregion EVENTS

    #region FUNCTION
    public bool IsRetryBtnExpress()
    {     
        return _inGameData.StagePlayReward.getStarCoin + _historyStarVal >= 5;
    }

    public bool IsGetReward()
    {
        return (_inGameData.StagePlayReward.stageReward.productItemList.Count == 0)? false: true;
    }

    public void InitStar()
    {
        for(int i = 0; i <_starGroup.Length; i++)
        {
            _starGroup[i].gameObject.SetActive(i < _historyStarVal);
        }
    }


    IEnumerator SetAttainStarPoint()
    {
        yield return GameManager.Instance.GetUserStageInfo();
        int myStarValue = _inGameData.UserStageInfo.GetUserStageDetailInfo(_inGameData.ThemeDifficulty).userStarCoin;

        for (int i = 0; i < _starGroup.Length; i++)
        {
            if(i < myStarValue)
            {
                if(i < _historyStarVal)
                {
                    continue;
                }

                _starGroup[i].gameObject.SetActive(true);
                _starAnimator[i].Play("start",0,0);
                yield return new WaitForSeconds(0.05f);
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_starcoin_pickup);
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            _starGroup[i].gameObject.SetActive(false);
        }
    }

    public List<AudioDataKey> GetAudioSoundKeyList()
    {
        List<AudioDataKey> soundInfos = new List<AudioDataKey>();
        soundInfos.Add(AudioDataKey.ui_result_star_impact_1);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_2);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_3);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_4);
        soundInfos.Add(AudioDataKey.ui_result_star_impact_5);

        return soundInfos;
    }
    #endregion
}