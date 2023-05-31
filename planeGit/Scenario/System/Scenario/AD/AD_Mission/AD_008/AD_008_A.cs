using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_008_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("기장에게 인터폰을 통해 Push back 준비 완료를 알리세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("기장님 push back  준비 완료 되었습니다.");
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[1], 10).AddTo();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}