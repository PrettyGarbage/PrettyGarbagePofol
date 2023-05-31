using Common;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UnityEngine.Playables;

public class EL_002_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_002_A;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            Logger.Log("좌석 벨트를 매지 않은 승객을 찾아 좌석 벨트를 착용하게 하세요.");
            
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("손님 좌석벨트 착용해 주십시오.");

            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EL_002_A.PlayAsync();

            /*var npc1 = NPCListModel.Instance.Get(1);
            npc1.Animator.SetTrigger(Constants.EquipBelt);
            npc1.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);*/
            
            // NPCListModel.Instance.NPCList.Where(npc => new[]{1}.Contains(npc.id)).ForEach(npc =>
            // {
            //     npc.Animator.SetTrigger(Constants.EquipBelt);
            //     npc.Seat.Belt.Animator.SetBool(Constants.IsEquip, true);
            // });
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}
