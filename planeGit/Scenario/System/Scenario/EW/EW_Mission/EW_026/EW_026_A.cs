using System;
using Common;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_026_A : Mission
{
    #region Override Methods

    [SerializeField] PlayableDirector director_26_A_1;
    [SerializeField] PlayableDirector director_26_A2_1;
    public override void SetMission()
    {
        var npc4 =NPCListModel.Instance.Get(4);
        var npc5 =NPCListModel.Instance.Get(5);
        var npc7 =NPCListModel.Instance.Get(7);
        var npc8 =NPCListModel.Instance.Get(8);

        OnBeginMission(0).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
         
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            // 5,7번 Raft 연결 끈을 Assist handle에 연결

            await director_26_A_1.PlayAsync();
            try
            {
                npc4.Animator.SetFloat(Constants.IdleState, 41);
                npc5.Animator.SetFloat(Constants.IdleState, 42);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            // 애니 축 때문에 겹치는 버그 발생
            
            await director_26_A2_1.PlayAsync();
            try
            {
                npc7.Animator.SetFloat(Constants.IdleState, 41);
                npc8.Animator.SetFloat(Constants.IdleState, 42);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Logger.Log("밑의 애니들을 연결선 조정후 다시 적용하기");
            
            /*npc4npc5_raft.Animator.SetTrigger(Constants.HangRaft);
            npc7npc8_raft.Animator.SetTrigger(Constants.HangRaft);
            
            npc4.Animator.SetTrigger(Constants.ConnectAssistHandleV2);
            npc8.Animator.SetTrigger(Constants.ConnectAssistHandleV2);
            
            npc5.Animator.SetTrigger(Constants.ConnectAssistHandle);
            npc7.Animator.SetTrigger(Constants.ConnectAssistHandle);
            
            await npc4.Animator.WaitAnimationCompleteAsync(Constants.Man_Carry_Raft_Hang_v02);
            await npc8.Animator.WaitAnimationCompleteAsync(Constants.Man_Carry_Raft_Hang_v02);
            await npc5.Animator.WaitAnimationCompleteAsync(Constants.Man_Carry_Raft_Hang_v01);
            await npc7.Animator.WaitAnimationCompleteAsync(Constants.Man_Carry_Raft_Hang_v01);*/
            NextMission();

        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
