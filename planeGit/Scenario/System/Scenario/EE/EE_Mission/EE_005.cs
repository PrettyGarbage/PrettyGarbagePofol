using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_005 : Mission
{
    #region Fields

    [SerializeField] Interactable Po2bottle;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Po2bottle을 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
         OnBeginMission(1).Subscribe(async _ =>
         {
             Logger.Log("Po2bottle: 정위치, 마스크와 튜브의 상태, 마스크와 Po2outlet 연결상태, 압력 게이지상태(1600PSI)를 확인하세요");
             await HandsetSystem.Instance.AttachMissionAsync(Po2bottle, Dialogues[1], 10).AddTo();

             NextMission();
        }).AddTo(); 
         
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}