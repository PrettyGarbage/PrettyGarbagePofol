using Common;
using UnityEngine;
using System;
using UniRx;

public class TF_003_B : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {          
            Logger.Log("객실 앞쪽 화장실에서  화재가 발생 했습니다. 객실 사무장에게 상황보고 하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo(); 
            
            Logger.Log("객실 앞쪽 화장실에서  화재가 발생 했습니다.흰색 연기와 종이타는 냄새가 나고 있습니다");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("3,4번 승무원에게 도움을 요청하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[2]).AddTo();
            
            Logger.Log("Halon 소화기와 PBE 준비해주세요. 석면장갑을 준비해주세요.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("화장실에 사람이 있는지 확인하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[4]).AddTo();
            
            Logger.Log("안에 누구 계십니까?");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[5], 10).AddTo());
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}