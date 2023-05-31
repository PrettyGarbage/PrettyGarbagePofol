using UnityEngine;
using System;
using UniRx;

public class EL_002_B : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("화장실 변기뚜껑을 닫으세요.");
            Logger.Log("더미 Interactable");
            
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 5).AddTo());
            
            NextMission();
        }).AddTo(); 
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("화장실 변기뚜껑을 닫는 애니");
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}
