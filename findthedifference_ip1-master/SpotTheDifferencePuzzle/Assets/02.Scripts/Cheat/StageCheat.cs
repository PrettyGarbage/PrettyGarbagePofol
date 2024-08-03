using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCheat : MonoBehaviour {

    public InGameData _inGameData = GameManager.Instance.InGameData;

    #region COROUTINE
    public IEnumerator StageResultSend()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        bool isEnd = false;

        List<StageInfo> stageInfos = DataManager.Instance.GetStageInfoList(_inGameData.ThemeInfo.id);

        if (DataManager.Instance.UserInfo.Heart < stageInfos.Count)
        {
            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.ALERT_NOT_ENOUGH_HEART);
            yield break;
        }

        for (int i = 0; i < stageInfos.Count; i++)
        {
            ReqeustStagePlay reqeustStagePlay = new ReqeustStagePlay();
            reqeustStagePlay.difficultyType = _inGameData.ThemeDifficulty;
            reqeustStagePlay.stageId = DataManager.Instance.GetStageInfoList(_inGameData.ThemeInfo.id)[i].stageId;
            reqeustStagePlay.themeId = _inGameData.ThemeInfo.id;
            reqeustStagePlay.userId = DataManager.Instance.UserInfo.Id;

            List<string> itemIdList = new List<string>();

            ApiManager.Instance.RequestPlayInfo(reqeustStagePlay, (result) =>
            {
                if (result.isSuccess)
                {
                    GameManager.Instance.InGameData.SetRequestStageInfo(result.result, itemIdList, 5);

                    StagePlayResult stagePlayResult = new StagePlayResult();

                    stagePlayResult.seq = result.result;
                    stagePlayResult.score = 200000;
                    stagePlayResult.remainTimeSeconds = 60;
                    stagePlayResult.isContinue = false;

                    ApiManager.Instance.SendPlayResult(stagePlayResult, (playResult) =>
                    {
                        if (playResult.isSuccess)
                        {
                            GameManager.Instance.InGameData.SetStagePlayReward(playResult.result);
                            isEnd = true;
                        }
                        else
                        {
                            MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.INTRO_LOGIN_FAIL_ALERT);
                        }
                    });
                }
                else
                {
                    Debug.LogError("네트워크 확인");
                }
            });

            while (!isEnd)
            {
                yield return CommonConstants.WaitLoopSeconds;
            }
            isEnd = false;
        }
        MessageBoxState.Open(LocalizationTextKey.COMMON_ALERT_TITLE, LocalizationTextKey.LOBBY_ACTIONTXT_ALLCLEAR_TITLE);
        StateManager.Instance.ActiveBlockScreen(false);
        
    }
    #endregion
}
