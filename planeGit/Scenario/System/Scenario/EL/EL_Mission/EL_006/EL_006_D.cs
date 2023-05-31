using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_006_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_006_D_1;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1); //1번이 1번 미션할때까지 대기
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            //승객들 일부는 안전벨트를 풀고 있음 연출하기
            director_EL_006_D_1.Play();
            
            NPCListModel.Instance.Get(10).Animator.SetFloat(Constants.IdleState, 31);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("기장의 지시가 있을때 까지 기다리세요! Wait!");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            var npc10 = NPCListModel.Instance.Get(10);
            NPCListModel.Instance.Get(10).Animator.SetFloat(Constants.IdleState, 0);
     
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }

    #endregion
}