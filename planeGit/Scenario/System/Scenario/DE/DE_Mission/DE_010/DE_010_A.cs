using Common;
using UnityEngine;
using System;
using UniRx;

public class DE_010_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("이동에 방해가 되는 물건들을 정리해 주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            
            Logger.Log("걱정하지 마십시오. 저희 승무원들은 비상상황을 대비해 전문적인 훈련을 받았습니다. 걱정하지 마십시오.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo()); 
            
            Logger.Log("다치신 분 계십니까? 불편하신 분 승무원에게 알려주십시오.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}