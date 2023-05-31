using UnityEngine;
using System;
using UniRx;
using Valve.VR.InteractionSystem;

public class EE_011 : Mission
{
    #region Fields

    [SerializeField] Interactable AED;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("AED를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("AED : 정위치, Battery 상태를 확인 (모래시계) 하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(AED, Dialogues[1], 10).AddTo();
            
            NextMission();
        }).AddTo(); 

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}