using Common;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class FT_004_A : Mission
{

    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            // 벨트사인 sound가 정해지면 대체하기
            SoundManager.Instance.PlaySoundEffect("Airplane_Ding_Dong");
            
            Logger.Log("손님 여러분 저희 비행기는 흔들리고 있습니다. 좌석 벨트를 매주시고 화장실 사용은 삼가하시기 바랍니다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 15);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}