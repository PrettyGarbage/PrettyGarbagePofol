using UnityEngine;
using System;
using UniRx;

public class EL_015_D : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
          // 10번 승객이 캐리어를 가지고 탈출하력 하는 애니 연출하기
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {            
            Logger.Log("캐리어를 가지고 탈출하는 승객을 저지하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 5).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("짐 버려!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
            
        }).AddTo();
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            // 10번 승객이 짐 버리고 탈출하는 애니 연출하기.
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}