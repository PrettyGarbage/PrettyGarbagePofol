using UnityEngine;
using UniRx;
using Valve.VR.InteractionSystem;

public class EE_008 : Mission
{
    #region Fields

    [SerializeField] Interactable spareLifeJacket;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("유아/spare 구명복을 확인하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("유아/spare 구명복: 정위치, 장비수량 및 상태를 확인하세요.");
            await HandsetSystem.Instance.AttachMissionAsync(spareLifeJacket, Dialogues[1], 10).AddTo();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}