using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_014 : Mission
{
    #region Fields

    [SerializeField] Interactable blackPouch;
    [SerializeField] PlayableDirector director_014_1;
    [SerializeField] PlayableDirector director_014_2;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승객 브리핑 장비: 장비수량 및 상태를 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            await director_014_1.PlayAsync();
            NextMission();
        }).AddTo();     
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("물품을 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            AttachSystem.Instance.Attach(blackPouch, Valve.VR.InteractionSystem.Player.instance.rightHand);
            await director_014_2.PlayAsync();
            AttachSystem.Instance.Detach(blackPouch, Valve.VR.InteractionSystem.Player.instance.rightHand);

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}