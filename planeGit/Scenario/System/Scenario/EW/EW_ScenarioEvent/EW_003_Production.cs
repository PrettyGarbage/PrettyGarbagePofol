using System;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Library.Manager;
public class EW_003_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_003 시작");

        if (isObserver)
        {
            Logger.Log("비행기 흔들림 시작 신호 전송");
            
            // CrewTraningRsltController.Instance.SetTrainingRslt(
            //     traningCode: EventCode,
            //     eventCode: EventCode,
            //     testNo: 0,
            //     role: ConfigModel.Instance.Setting.role,
            //     startTime: "",
            //     endTime: DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ"),
            //     result: 1,
            //     description: "",
            //     option: 0,
            //     motionState: default
            // );
        }
        Logger.Log("하드웨어 : 비행기 동체 흔들림");

        Logger.Log("연출 : 창문이 흔들리는 소리");
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_003 종료");
    }

    #endregion
}