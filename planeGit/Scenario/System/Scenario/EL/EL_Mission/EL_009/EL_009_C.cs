using UnityEngine;
using System;
using UniRx;

public class EL_009_C : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);   //1번이 2번 미션할때까지 대기
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {            
            Logger.Log("지정된 R2 탈출구로 이동하여 승객들을 탈출 시켜주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}