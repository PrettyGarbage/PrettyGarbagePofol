using UniRx;
using UnityEngine;

public class MissionSkip : Mission
{
    #region Fields

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(_ => LastMissionComplete()).AddTo();
    }

    #endregion
}