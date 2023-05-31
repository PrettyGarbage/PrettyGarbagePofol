using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class AD_001_A : Mission
{
  
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            Logger.Log("기내 방송장비 위치에 서서 장비를 터치(이용)해서, 승무원들에게 지시 사항을 알려주세요.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10));

            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}