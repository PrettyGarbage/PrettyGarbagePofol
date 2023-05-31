using UnityEngine;
using System;
using UniRx;

public class EL_002_C : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("갤리 compartment 안에 있는 서비스 용품들이 떨어지지 않게 cart를 닫으세요.");
            
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}
