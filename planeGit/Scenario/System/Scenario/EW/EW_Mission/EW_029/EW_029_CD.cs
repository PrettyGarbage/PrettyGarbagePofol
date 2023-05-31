using Common;
using UniRx;
using UnityEngine;

public class EW_029_CD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1, 2).AddTo(); //1,2 번 승무원이 1번 미션을 시작할 때까지 대기

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
