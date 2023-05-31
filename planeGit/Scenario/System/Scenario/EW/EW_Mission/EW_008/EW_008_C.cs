using Common;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_008_C : Mission
{
    
    #region Fields

    [SerializeField] PlayableDirector director_EW_008_C_2;

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            /*npc4.Seat.Animator.SetBool(Constants.IsTable, true);*/
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            /*npc8.Animator.SetTrigger(Constants.EquipTable);
            npc4.Seat.Animator.SetBool(Constants.IsTable, false);*/
            
            await director_EW_008_C_2.PlayAsync();

            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}