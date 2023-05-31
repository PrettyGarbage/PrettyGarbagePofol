using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_008_B : Mission
{
    
    #region Fields

    [SerializeField] PlayableDirector director_EW_008_B_2;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            //3번 승객 지목
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            /*npc3.Animator.SetTrigger(Constants.EquipBelt);
            npc3.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);*/
            
            await director_EW_008_B_2.PlayAsync();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
