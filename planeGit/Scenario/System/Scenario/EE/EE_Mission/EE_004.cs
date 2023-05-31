using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_004 : Mission
{
    #region Fields

    [SerializeField] Interactable halonExtinguisher;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Halon소화기를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("Halon소화기: 정위치, seal 상태, 압력 게이지상태(green band)를 확인하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(halonExtinguisher, Dialogues[1], 10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}