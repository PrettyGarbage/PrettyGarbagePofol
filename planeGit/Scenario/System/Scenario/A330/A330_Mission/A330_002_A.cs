using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class A330_002_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_OpenCover;
    [SerializeField] PlayableDirector director_Lever;
    [SerializeField] PlayableDirector director_CloseCover;

    [SerializeField] PlayableDirector director_SafetyPin;
    [SerializeField] PlayableDirector director_OpenHandle;
    [SerializeField] PlayableDirector director_OpenDoor;
    [SerializeField] PlayableDirector director_GustLock;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            Logger.Log("커버를 열어주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("Cover 열리는 애니 ");
            await director_OpenCover.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("레버를 Slide Disarmed 모드로 전환해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("레버 움직이는 연출");
            await director_Lever.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("커버를 닫아주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(async _ =>
        {
            Logger.Log("Cover 닫히는 애니");
            await director_CloseCover.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(6).Subscribe(async _ =>
        {
            Logger.Log("Safty Pin을 고정시켜주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[4], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(7).Subscribe(async _ =>
        {
            Logger.Log("Safty Pin을 고정하는 애니");
            await director_SafetyPin.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(8).Subscribe(async _ =>
        {
            Logger.Log("Door의 손잡이를 위로 올려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[5], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(9).Subscribe(async _ =>
        {
            Logger.Log("Door의 손잡이를 위로 올리는 애니");
            await director_OpenHandle.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(10).Subscribe(async _ =>
        {
            Logger.Log("문을 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[6], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(11).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하는 애니.");
            await director_OpenDoor.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(12).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock Realease Button을 눌러주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[7], 10).AddTo());
            NextMission();
        }).AddTo();
        OnBeginMission(13).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock Realease Button 눌리는 애니");
            await director_GustLock.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(14).Subscribe(async _ =>
        {
            Logger.Log("A330 기종의 탈출구를 개방하였습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[8], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(15).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}