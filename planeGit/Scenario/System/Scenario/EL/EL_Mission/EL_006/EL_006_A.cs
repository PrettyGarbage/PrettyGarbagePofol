using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_006_A : Mission
{
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {  
            
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[0], 15).AddTo();
            
            Logger.Log("신속히 기장에게 상황을 보고하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            Logger.Log("기장님, 객실에 연기가 나고 있으며 앞쪽 좌석이 일부 파손되었습니다 .");
            
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
