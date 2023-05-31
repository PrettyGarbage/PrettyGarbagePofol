using Common;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class AD_002_A : Mission
{
    #region Fields

    [SerializeField] Interactable handset;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승무원에게 승객탑승 전 방송을 하세요.(Handset)");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("약 3분 후 승객 탑승 하십니다. 승무원 stand by 하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(handset, Dialogues[1], 10, true);
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}
