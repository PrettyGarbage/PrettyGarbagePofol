using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_006_B : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("R1 Door 에 Stand by 한 다음, 상황을 보고하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("R1 door stand by ");
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