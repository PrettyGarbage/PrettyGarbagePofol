using UnityEngine;
using System;
using UniRx;

public class EL_014_C : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
           await WaitOtherCrewMission(2, 1);  
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {            
            Logger.Log("R2탈출구로 승객들의 탈출을 유도하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("양팔 앞으로!!  뛰어 내려가!!  멀리 피해!! Arms Straight Ahead! Jump And Slide! Move Away!  ");
            
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