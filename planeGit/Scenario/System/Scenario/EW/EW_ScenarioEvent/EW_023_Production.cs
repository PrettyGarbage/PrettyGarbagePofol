using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UnityEngine;

public class EW_023_Production : ScenarioEventProduction
{

    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_023_Production 시작");
        
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo();
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_023 종료");
    }
    #endregion
}