using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_006 : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_006;
    [SerializeField] Interactable megaPhone;

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await director_006.PlayAsync();
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("메가폰을 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("메가폰: 정위치, 작동여부, 고정상태를 확인하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(megaPhone, Dialogues[1], 10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}