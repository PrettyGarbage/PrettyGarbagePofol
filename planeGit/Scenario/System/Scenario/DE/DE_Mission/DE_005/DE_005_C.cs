using UnityEngine;
using System;
using UniRx;

public class DE_005_C : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("2번 승무원의 근처의 산소통을 화재진압지로부터 멀리 치워 주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("산소통을 치운는 연출");

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("승객들에게 젖은 타월을 나눠주세요 : point : 젖은 타월 9번 승객");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();   
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("9. 승객이 젖은 타월을 받아 코와 입을 막는 연출하기");
            
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}