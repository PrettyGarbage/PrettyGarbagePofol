using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UnityEngine;

public class EW_999_Production : ScenarioEventProduction
{

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_999 시작");
        
        Logger.Log("승객들과 승무원 전원 무사히 탈출하였습니다. 체험을 종료하겠습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
   
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_999 종료");
    }

    #endregion
}