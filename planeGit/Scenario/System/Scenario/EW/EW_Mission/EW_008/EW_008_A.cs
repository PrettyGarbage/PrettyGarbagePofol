using UniRx;

public class EW_008_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {         
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
