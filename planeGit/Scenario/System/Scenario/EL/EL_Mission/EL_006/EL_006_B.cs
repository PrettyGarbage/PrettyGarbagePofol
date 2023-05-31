using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_006_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_006_B_1;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("B_0번 미션 진입");
            await WaitOtherCrewMission(1, 1); //1번이 1번 미션할때까지 대기
            Logger.Log("B_0번 미션 종료");
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("EL_006_B의 1번째 미션 진입");
            director_EL_006_B_1.Play();
            NPCListModel.Instance.Get(5).Animator.SetFloat(Constants.IdleState, 31);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("기다리세요! Wait! (반복) 3회");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            NPCListModel.Instance.Get(5).Animator.SetFloat(Constants.IdleState, 31);
            
            /*var npc5 = NPCListModel.Instance.Get(5);
            npc5.Animator.SetFloat(Constants.IdleState, 0);*/
            
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}