using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EE_001_A : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("방송을 통해, 승무원들에게 비행 전 장비 점검을 실시하게 하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0],10).AddTo(); 
            
            Logger.Log("승무원들은 비행 전 장비 점검을 실시하시고 이상 유무를 보고 하시기 바랍니다");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1],10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}