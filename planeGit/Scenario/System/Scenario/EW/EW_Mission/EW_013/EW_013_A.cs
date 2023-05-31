using Common;
using UniRx;

public class EW_013_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            NPCListModel.Instance.NPCList.ForEach(model => model.Animator.SetFloat(Constants.IdleState, 0));
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();  
        
    }
    #endregion
}
