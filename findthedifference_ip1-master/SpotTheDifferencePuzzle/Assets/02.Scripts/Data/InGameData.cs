using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InGameData
{
    //theme
    private ThemeDifficulty _themeDifficulty;
    private ThemeInfo _themeInfo;
    private UserThemeInfo _userThemeInfo;

    //Stage
    private List<UserStageInfo> _userStageInfoList;

    //Ready
    private int _stageIndex;

    //InGame
    private int _requestSeq;
    private int _findDifferenceCount;
    private List<string> _itemIdList = new List<string>();

    //종료
    private StagePlayReward _stagePlayReward;

    //인게임 이어하기 
    private int _gameContinueTime;

    public UserStageInfo UserStageInfo { get { return _userStageInfoList[StageIndex]; } }

    public int StageIndex { get { return _stageIndex; } }

    public ThemeDifficulty ThemeDifficulty { get { return _themeDifficulty; } }

    public StageInfo StageInfo { get { return DataManager.Instance.GetStageInfoList(_themeInfo.id)[_stageIndex]; } }

    public ThemeInfo ThemeInfo { get { return _themeInfo; } }

    public UserThemeInfo UserThemeInfo { get { return _userThemeInfo; } }

    public int RequestSeq { get { return _requestSeq; } }
    public int FindDifferenceCount { get { return _findDifferenceCount; } }

    public List<string> ItemIdList { get { return _itemIdList; } }

    public StagePlayReward StagePlayReward { get { return _stagePlayReward; } }

    public int GameContinueTime { get { return _gameContinueTime; } set { _gameContinueTime = value; } }

    public void SetStageListData(ThemeInfo themeInfo, ThemeDifficulty themeDifficulty)
    {
        _themeInfo = themeInfo;
        _themeDifficulty = themeDifficulty;
    }

    public void SetReadyData(int stageIndex)
    {
        _stageIndex = stageIndex;
    }

    public void SetUserStageInfoList(List<UserStageInfo> userStageInfoList)
    {
        _userStageInfoList = userStageInfoList;
    }

    public void SetSelectUserThemeInfo(UserThemeInfo userThemeInfo)
    {
        _userThemeInfo = userThemeInfo;
    }

    #region 게임결과 리워드
    public void SetStagePlayReward(StagePlayReward stagePlayReward)
    {
        _stagePlayReward = stagePlayReward;
        if (_stagePlayReward != null && _stagePlayReward.unlockCollectionId.Length > 0)
        {        
            DataManager.Instance.AddUnlockCollectionId(new CollectionListInfo(_stagePlayReward.unlockCollectionId, false));
        }
        if (_stagePlayReward != null && _stagePlayReward.unlockThemeId.Length > 0)
        {
            DataManager.Instance.AddUnlockThemeId(_stagePlayReward.unlockThemeId);
        }
    }

    public bool IsExistUnLockCollection()
    {
        return _stagePlayReward != null && _stagePlayReward.unlockCollectionId.Length > 0;
    }

    public void ReleaseUnLockCollectionId()
    {
        _stagePlayReward.unlockCollectionId = string.Empty;
    }
    #endregion

    #region 테마선택 액션테스트 부분
    //언락테마 정보가 있는가?
    public bool IsUnLockTheme()
    {
        if(DataManager.Instance.GetUnlockThemeId() == null)
        {
            return false;
        }

        return (DataManager.Instance.GetUnlockThemeId().Length > 0);
    }
    #endregion

    public void SetRequestStageInfo(int requestSeq, List<string> itemIdList, int findDifferenceCount)
    {
        _requestSeq = requestSeq;
        _itemIdList = itemIdList;
        _findDifferenceCount = findDifferenceCount;
    }

    public bool SetNextStage()
    {
        if (_userStageInfoList.Count > _stageIndex + 1)
        {
            _stageIndex++;

            return true;
        }
        return false;
    }

    public UserStageInfo GetUserPreStageInfo(int stageIndex)
    {
        if (stageIndex != 0)
        {
            return _userStageInfoList[stageIndex - 1];
        }
        return null;
    }

    public UserStageInfo GetUserStageInfo(int stageIndex)
    {
        return _userStageInfoList[stageIndex];
    }

    public int GetStageTotalStarCoinAmount()
    {
        return _userStageInfoList.Sum(s => {
            return _themeDifficulty == ThemeDifficulty.THEME_BASIC ? s.stageDetailBasic.userStarCoin : s.stageDetailMaster.userStarCoin;
        });
    }

    public bool IsStageClaimReward(int stageIndex)
    {
        return _themeDifficulty == ThemeDifficulty.THEME_BASIC ? _userStageInfoList[stageIndex].stageDetailBasic.claimReward
            : _userStageInfoList[stageIndex].stageDetailMaster.claimReward;
    }

    public bool IsPreStageClaimReward(int stageIndex)
    {
        if (stageIndex == 0)
        {
            return true;
        }

        return _themeDifficulty == ThemeDifficulty.THEME_BASIC ? _userStageInfoList[stageIndex - 1].stageDetailBasic.claimReward
            : _userStageInfoList[stageIndex - 1].stageDetailMaster.claimReward;
    }

    public bool IsNextStage(int stageIndex)
    {
        return !IsStageClaimReward(stageIndex) && IsPreStageClaimReward(stageIndex);
    }

    public List<int> GetBaseScoreInfos()
    {
        BaseScoreInfo baseScoreInfo = DataManager.Instance.GetBaseScoreInfo(_themeDifficulty);

        List<int> baseScoreInfos = new List<int>();
        baseScoreInfos.Clear();
        baseScoreInfos.Add(baseScoreInfo.starcoin1);
        baseScoreInfos.Add(baseScoreInfo.starcoin2);
        baseScoreInfos.Add(baseScoreInfo.starcoin3);
        baseScoreInfos.Add(baseScoreInfo.starcoin4);
        baseScoreInfos.Add(baseScoreInfo.starcoin5);

        return baseScoreInfos;
    }

    public List<int> GetComboScoreInfos()
    {
        BaseScoreInfo baseScoreInfo = DataManager.Instance.GetBaseScoreInfo(_themeDifficulty);

        List<int> baseComboScoreInfos = new List<int>();
        baseComboScoreInfos.Clear();
        baseComboScoreInfos.Add(baseScoreInfo.basicScore);
        baseComboScoreInfos.Add(baseScoreInfo.comboMultiple1 * baseScoreInfo.basicScore);
        baseComboScoreInfos.Add(baseScoreInfo.comboMultiple2 * baseScoreInfo.basicScore);
        baseComboScoreInfos.Add(baseScoreInfo.comboMultiple3 * baseScoreInfo.basicScore);
        baseComboScoreInfos.Add(baseScoreInfo.comboMultiple4 * baseScoreInfo.basicScore);

        return baseComboScoreInfos;
    }
}