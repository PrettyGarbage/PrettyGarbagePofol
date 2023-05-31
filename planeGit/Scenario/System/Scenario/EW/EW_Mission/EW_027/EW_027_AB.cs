using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EW_027_AB : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_027_AB_1;
    [SerializeField] PlayableDirector director_EW_027_AB_2;
    [SerializeField] PlayableDirector director_EW_027_AB_3;
    [SerializeField] PlayableDirector director_EW_027_AB_4;
    [SerializeField] PlayableDirector director_EW_027_AB_5;
    [SerializeField] PlayableDirector director_EW_027_AB_6;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            Logger.Log("Girt Bar를 해제 하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("Girt Bar 해제 되는 애니 ");
            await director_EW_027_AB_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("Girt Bar를 걸어주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("Girt Bar 걸리는 애니");
            await director_EW_027_AB_2.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("Red Warning Flag를 위로 올려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(5, true).Subscribe(async _ =>
        {
            Logger.Log("Red Warning Flag를 위로 올리는 애니");
            await director_EW_027_AB_3.PlayAsync();
            NextMission();
        }).AddTo();
        
       OnBeginMission(6).Subscribe(async _ =>
        {
            Logger.Log("Handle을 돌려주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[4], 10).AddTo());
            NextMission();
        }).AddTo();  
       
       OnBeginMission(7, true).Subscribe(async _ =>
        {
            Logger.Log("Handle을 돌아가는 애니");
            await director_EW_027_AB_4.PlayAsync();
            NextMission();
        }).AddTo(); 
       
       OnBeginMission(8).Subscribe(async _ =>
        {
            Logger.Log("Door를 열어주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[5], 10).AddTo());
            NextMission();
        }).AddTo(); 
       
       OnBeginMission(9, true).Subscribe(async _ =>
        {
            Logger.Log("Door를 열리는 애니");
            await director_EW_027_AB_5.PlayAsync();
            NextMission();
        }).AddTo();
       
       OnBeginMission(10).Subscribe(async _ =>
        {
            Logger.Log("GustLock을 체크 해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[6], 10).AddTo());
            NextMission();
        }).AddTo();
       
     OnBeginMission(11, true).Subscribe(async _ =>
        {
            Logger.Log("Gust Lock 고정 애니");
            await director_EW_027_AB_6.PlayAsync();
            NextMission();
        }).AddTo();

       OnBeginMission(10, true).Subscribe(async _ =>
       {
           NPCListModel.Instance.NPCList.ForEach(npc =>
           {
               npc.Animator.SetBool(Constants.IsLookAround, false);
           });
           LastMissionComplete();
       }).AddTo();
    }

    #endregion
}