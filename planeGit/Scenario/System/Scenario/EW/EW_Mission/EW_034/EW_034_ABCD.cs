using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_034_ABCD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승객들과 승무원 전원 무사히 탈출하였습니다. 체험을 종료하겠습니다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            LastMissionComplete();
        }).AddTo();
    }
    #endregion
}