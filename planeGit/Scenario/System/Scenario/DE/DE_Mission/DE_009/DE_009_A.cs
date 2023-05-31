using Common;
using UnityEngine;
using System;
using UniRx;

public class DE_009_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("각 담당구역을 순회하며 환자발생유무를 확인하고 승객들을 안심시키세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            
            Logger.Log("좌석에 앉아 마스크 쓴 채로 승객 상태 (마스크 착용, 좌석 착석) 확인 하세요.");
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