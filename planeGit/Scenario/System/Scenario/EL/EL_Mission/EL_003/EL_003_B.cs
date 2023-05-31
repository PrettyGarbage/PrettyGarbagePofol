using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class EL_003_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_003_B;

    /*
    [SerializeField] JumpSeatModel jumpSeatF2;
    */

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("EW_003 시작");
            Logger.Log("착륙할 것을 기다리며 벨트를 매고 자리에 앉아 있음");

            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 3);

            // 2번 승무원의 jumpseat 
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 5));

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_EL_003_B.PlayAsync();
            
            /*jumpSeatF2.Animator.SetTrigger(Constants.jumpSeatOpen);
            jumpSeatF2.Animator.SetBool(Constants.IsEquipFABelt, true);
            await jumpSeatF2.Animator.WaitAnimationCompleteAsync(Constants.JumpSeat_Belt);*/
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}