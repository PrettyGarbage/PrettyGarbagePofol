using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class FT_006_B : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("벨트 매세요");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {            
            // A 미션에서 승객들 애니 처리하기

            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}