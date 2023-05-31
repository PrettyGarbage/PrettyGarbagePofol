using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class MD82_999_Production : ScenarioEventProduction
{
    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("MD82_999 미션 시작");
        Debug.Log("체험을 종료하겠습니다. 수고하셨습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Debug.Log("MD82_999 종료");
    }

    #endregion
    
}