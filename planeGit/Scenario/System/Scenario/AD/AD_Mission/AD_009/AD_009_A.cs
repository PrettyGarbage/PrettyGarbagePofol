using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_009_A : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("기장님 push back 준비 완료 되었습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}