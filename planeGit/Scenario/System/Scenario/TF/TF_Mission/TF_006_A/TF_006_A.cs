using Common;
using UnityEngine;
using System;
using UniRx;

public class TF_006_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {          
            Logger.Log("화재가 완전히 진압되었는지 확인해주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo(); 
            
            Logger.Log("다시한번 화재가 완전히 진압되었는지 확인해주세요.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}