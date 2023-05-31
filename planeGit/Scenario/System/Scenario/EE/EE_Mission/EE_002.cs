using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;
using Toggle = UnityEngine.UI.Toggle;

public class EE_002 : Mission
{
    [SerializeField] PlayableDirector director_002;
    [SerializeField] Interactable crewLifeJacket;

    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {           
            // jumpseat 하단 열기
            await director_002.PlayAsync();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("승무원용 구명복을 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0],10).AddTo());
            NextMission();
        }).AddTo();  
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("승무원용 구명복 : 정위치, 장비수량 및 상태 확인하세요.");
            await  HandsetSystem.Instance.AttachMissionAsync(crewLifeJacket ,Dialogues[1],10).AddTo();

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}