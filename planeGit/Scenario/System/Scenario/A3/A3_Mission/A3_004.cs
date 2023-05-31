using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class A3_004 : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("좌석 배치를 확인하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}