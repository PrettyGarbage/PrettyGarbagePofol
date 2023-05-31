using Common;
using UniRx;

public class EW_013_B : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            /*NPCListModel.Instance.NPCList.ForEach(model => model.Animator.SetInteger(Constants.IdleState, 0));*/
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}