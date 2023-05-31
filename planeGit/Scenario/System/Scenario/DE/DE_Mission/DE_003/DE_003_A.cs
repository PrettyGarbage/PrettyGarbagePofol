using UnityEngine;
using System;
using UniRx;

public class DE_003_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 2); //1번이 1번 미션할때까지 대기

            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("신속히 기장에게 상황을 보고하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            // 핸드셋 2번 누르는 미션 필요
        
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("기장님, 지금 객실 전방 화장실에서 기내화재가 발생하였습니다. 흰 연기와 종이 타는 냄새가 나고 있으며 현재 승무원이 Halon소화기를 이용하여 진압중 입니다. 승객들이 일부 동요하고 있습니다. 화재 진압 후 다시 보고 드리겠습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
       
            NextMission();
        }).AddTo();
       
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}