using Common;
using UniRx;
using UnityEngine;

public class EW_023_ABCD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
