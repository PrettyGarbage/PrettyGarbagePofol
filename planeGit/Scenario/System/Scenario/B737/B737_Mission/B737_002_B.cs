using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class B737_002_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_GirtBarUnlock;
    [SerializeField] PlayableDirector director_GirtBarHang;
    [SerializeField] PlayableDirector director_RedWarningFlag;
    
    [SerializeField] PlayableDirector director_OpenHandle;
    [SerializeField] PlayableDirector director_OpenDoor;
    [SerializeField] PlayableDirector director_GustLockReleaseLeverPush;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            Logger.Log("문 아래에 있는 GirtBar를 Unlock 하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("문 아래에 있는 GirtBarUnlock 애니 ");
            await director_GirtBarUnlock.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("GirtBar를 걸어주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("GirtBar 걸리는 애니");
            await director_GirtBarHang.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("RedWarningFlag를 위로 올려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(async _ =>
        {
            Logger.Log("RedWarningFlag_Move_Up애니");
            await director_RedWarningFlag.PlayAsync();
            NextMission();
        }).AddTo();
        
        
        OnBeginMission(6).Subscribe(async _ =>
        {
            Logger.Log("화살표 방향으로 핸들을 돌려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[4], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(7).Subscribe(async _ =>
        {
            Logger.Log("화살표 방향으로 핸들 돌아가는 애니");
            await director_OpenHandle.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(8).Subscribe(async _ =>
        {
            Logger.Log("문을 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[5], 10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(9).Subscribe(async _ =>
        {
            Logger.Log("Door의 손잡이를 위로 올리는 애니");
            await director_OpenDoor.PlayAsync();          
            NextMission();
        }).AddTo();   
        
        OnBeginMission(10).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock Realease Button을 눌러주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[6], 10).AddTo());
            NextMission();
        }).AddTo();   
        
        OnBeginMission(11).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock Realease Button 눌리는 애니");
            await director_GustLockReleaseLeverPush.PlayAsync();   
            NextMission();
        }).AddTo();   
        
        OnBeginMission(12).Subscribe(async _ =>
        {
            Logger.Log("A737 기종의 탈출구를 개방하였습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[7], 10).AddTo());   
            NextMission();
        }).AddTo();

        OnBeginMission(13).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }
    #endregion
}