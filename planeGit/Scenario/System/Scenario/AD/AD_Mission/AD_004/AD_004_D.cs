using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_004_D : Mission
{

    #region Fields
    //6번 승객
    [SerializeField] PlayableDirector director_004_D_1;
    [SerializeField] PlayableDirector director_004_D_2;
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("6. 승객 짐을 비상구 좌석에 보관한다.");
            await director_004_D_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("비상구 좌석 앞 짐을 방치 하는 승객에게 짐 보관 협조를 구하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("손님 비상구 좌석의 짐은 모두 선반 속에 보관해 주셔야 합니다. 이륙 후에 다시 꺼내셔도 됩니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();   
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("승객이 짐을 선반에 보관한다.");
            await director_004_D_2.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}