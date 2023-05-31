using Common;
using UnityEngine;
using System;
using UniRx;

public class TF_008_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("손님여러분, 조금 전의 화재는 완전히 진화되었으며 지금 우리 비행기는 예정대로 순항하고 있습니다. 협조해주신 여러분께 진심으로 감사드립니다. 저희 승무원 일동은 목적지에 도착할 때까지 손님 여러분을 안전하고 편안하게 모실 수 있도록 최선을 다하겠습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 15).AddTo());
            
            Logger.Log("Ladies and gentlemen, the fire has completely put out. We are now at a normal cruising altitude, and we will continute to our destination as scheduled. Thank you for your coorperation and please enjoy the rest of your flight");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 15).AddTo());

            NextMission();
        }).AddTo();
        

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}