using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_028_CD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 1,2).AddTo(); // 1,2 번 승무원이 3번 미션을 시작할 때까지 대기

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("overwing window exit로 승객들을 탈출시키세요.");

            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
       
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            /*
             손잡이 당기기?
             */

            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());

            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
