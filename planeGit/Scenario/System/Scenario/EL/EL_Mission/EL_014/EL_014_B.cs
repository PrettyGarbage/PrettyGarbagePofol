using UnityEngine;
using System;
using UniRx;

public class EL_014_B : Mission
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
            Logger.Log("창문쪽 탈출구로 승객을 탈출 시키세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("바깥으로! 날개 뒤로 내려! 멀리 피해! Step Through!Slide Off The Back Of The Wing Move Away!");
            
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
            
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}