using System;
using Common;
using Library.Manager;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_028_A : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_28_A_4_ThrowRaft;
    [SerializeField] PlayableDirector director_28_A_1_Slide;
    [SerializeField] PlayableDirector director_28_A_1_Inflation;
    [SerializeField] PlayableDirector director_28_A_8;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        var npc4 = NPCListModel.Instance.Get(4);
        var npc5 = NPCListModel.Instance.Get(5);

        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당겨");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_28_A_1_Inflation.PlayAsync();
            
            Logger.Log("슬라이드 팽창되는 애니");
            await director_28_A_1_Slide.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("슬라이드 팽창 확인");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("4, 5번 협력자들이 Raft를 물위로 던지도록 하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());

            Logger.Log("Raft를 물 위로 던져");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("4번 위치 변경");

            Logger.Log("4, 5번 raft 물 위로 던지기 및 raft 모델 던져지는 애니");

            try
            {
                npc4.Animator.SetFloat(Constants.IdleState, 1);
                npc5.Animator.SetFloat(Constants.IdleState, 1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            await director_28_A_4_ThrowRaft.PlayAsync();
            
            NextMission();
        }).AddTo();

        // 4/8. 번 승객 L1에서 Raft 물밖으로 던짐 연출 필요

        OnBeginMission(5).Subscribe(async _ =>
        {
            Logger.Log("연결선 당겨");

            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[4], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(6).Subscribe(async _ =>
        {
            Logger.Log("탈출구정상!! 짐버려!! 이쪽으로!!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[5], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(7).Subscribe(async _ =>
        {
            Logger.Log("구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[6], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(8, true).Subscribe(async _ =>
        {
            Logger.Log("물로 뛰어는 연출하기");
            
            // np4 npc5 뛰어 내리는 포지션 세팅 후 뛰기 
            //npc4.Animator.SetTrigger(Constants.JumpOcean);
            /*await NPCMovementSystem.Instance.OceanJumpMoveByPath(npc4, 1, npc4jumpWaypoints);

            await NPCMovementSystem.Instance.SwimMoveByPath(npc4, 1, npc4Swimpoints);*/
            
            await director_28_A_8.PlayAsync();
            try
            {
                npc4.Animator.SetFloat(Constants.IdleState, 52);
                npc5.Animator.SetFloat(Constants.IdleState, 52);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(9).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}