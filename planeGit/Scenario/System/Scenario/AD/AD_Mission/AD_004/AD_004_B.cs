using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_004_B : Mission
{

    #region Fields
    // 5번 승객
    //[SerializeField] PlayableDirector director_004_B_1;    
    //[SerializeField] PlayableDirector director_004_B_4;
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
            Logger.Log("승객 탑승권을 들고 지나간다.");
            //await director_004_B_1.Play();

            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("탑승권의 날짜, 편명, 이름, 좌석 번호를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("손님 항공 보안 확인으로 탑승권을 보여주시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("승객 멈추고 탑승권을 보여준다.");
            //await director_004_B_4.Play();

            NextMission();
        }).AddTo();
        
        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}