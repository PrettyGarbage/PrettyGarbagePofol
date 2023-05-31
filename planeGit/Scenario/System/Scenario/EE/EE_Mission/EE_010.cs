using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_010 : Mission
{
    #region Fields

    [SerializeField] Interactable EMK;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("EMK를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("EMK: 정위치, Seal 상태를 확인하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(EMK, Dialogues[1], 10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}