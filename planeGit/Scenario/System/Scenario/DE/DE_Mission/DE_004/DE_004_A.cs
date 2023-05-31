using Common;
using UnityEngine;
using System;
using Library.Manager;
using UniRx;

public class DE_004_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {              
            // todo: 추후 사운드 교체 필요
            Logger.Log("SeatbeltSign On 띵 효과음");
            SoundManager.Instance.PlaySoundEffect("Airplane_Ding_Dong");
            
            Logger.Log("비상 강하 중! Attention, Emergency Descent!  벨트를 매고 마스크를 코와 입에 대십시오! Fasten Your Seatbelt And Put The Mask Over Your Nose And Mouth!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 15).AddTo()); 
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("안전벨트 매세요!!");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]).AddTo();
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}