using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_001_A : Mission
{
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            SoundManager.Instance.PlaySoundEffect("Airplane_ding_dong");

            Logger.Log("기장 : Cabin crew prepare for landing. ");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            //기본 세팅해야함

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ => { LastMissionComplete(); }).AddTo();
    }

    #endregion
}