using UnityEngine;
using System;
using System.Linq;
using Common;
using UniRx;
using UnityEngine.Playables;

public class EL_011_B : Mission
{

    #region Fields
    
    [SerializeField] PlayableDirector director_EL_011_B_1;

    /*[SerializeField] Transform[] npc1waypoints;
    [SerializeField] Transform[] npc2waypoints;*/
   
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        var npc1 = NPCListModel.Instance.Get(1);
        var npc2 = NPCListModel.Instance.Get(2);

        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(3, 1);   //1번이 3번 미션할때까지 대기
            NextMission();
        }).AddTo();

        OnBeginMission(1,true).Subscribe(async _ =>
        {           
            Logger.Log("1, 2번 npc 지목 후 탈출구를 열고 나가는 애니 연출하기");

            Logger.Log("R1쪽 승객 대피하는 연출하기");
            
            NPCListModel.Instance.Gets(1, 2).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 1);
            });
            
            await director_EL_011_B_1.PlayAsync();

            /*await NPCMovementSystem.Instance.MoveByPath(npc2, 1, npc2waypoints);
            npc2.transform.forward = npc2waypoints.Last().forward;
            
            await NPCMovementSystem.Instance.MoveByPath(npc1, 1, npc1waypoints);
            npc1.transform.forward = npc1waypoints.Last().forward;*/
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}