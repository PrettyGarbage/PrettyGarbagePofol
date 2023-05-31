using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class B777_002_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_OpenCover;
    [SerializeField] PlayableDirector director_Move_Manual;
    [SerializeField] PlayableDirector director_Close_Cover;
    
    [SerializeField] PlayableDirector director_Open_Handle;
    [SerializeField] PlayableDirector director_Open_Door;
    
    [SerializeField] PlayableDirector director_GustLock_Pull;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            Logger.Log("Door Mode Select Panel Cover를 올려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("커버 열리는 애니 ");
            await director_OpenCover.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("Door Mode를 Manual Mode로 전환해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("Manual Mode로 전환되는 애니");
            await director_Move_Manual.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            // 커버 닫는 부분 sound 없어서 패스할 가능성 있음
            Logger.Log("커버를 닫아주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(async _ =>
        {
            Logger.Log("커버가 닫히는 애니");
            await director_Close_Cover.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(6).Subscribe(async _ =>
        {
            Logger.Log("Assist Handle을 잡고 Door Operating Handle을 Open 방향으로 완전히 들어올리세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[4], 10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(7).Subscribe(async _ =>
        {
            Logger.Log("화살표 방향으로 핸들 돌아가고 문이 열리는 애니");
            await director_Open_Handle.PlayAsync();
            NextMission();
        }).AddTo();
        
        OnBeginMission(8).Subscribe(async _ =>
        {
            Logger.Log("Door가 동체에 GustLock 될때까지 바깥쪽으로 밀어주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[5], 10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(9).Subscribe(async _ =>
        {
            Logger.Log("문 열리는 애니");
            await director_Open_Door.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(10).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock을 당겨주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[6], 10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(11).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock 당겨지는 애니");
            await director_GustLock_Pull.PlayAsync();          
            NextMission();
        }).AddTo();   
        
        OnBeginMission(12).Subscribe(async _ =>
        {
            Logger.Log("A777 기종의 탈출구를 개방하였습니다.");
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