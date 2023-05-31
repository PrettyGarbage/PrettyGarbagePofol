using Common;
using UnityEngine;
using System;
using UniRx;

public class EW_027_B : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            Logger.Log("외부상황 확인(바람,화재,지면높이 정상)");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("Cover 열리는 애니 ");
            // R1Door.Animator.SetTrigger(Constants.DoorCoverOpen);
            // await R1Door.Animator.WaitAnimationCompleteAsync(Constants.Door_Mode_Cover_Open);
            // R1Door.Animator.SetTrigger(Constants.DoorMoveManual);
            // await R1Door.Animator.WaitAnimationCompleteAsync(Constants.Door_Mode_Move_Manual);
            // R1Door.Animator.SetTrigger(Constants.DoorCoverClose);
            // await R1Door.Animator.WaitAnimationCompleteAsync(Constants.Door_Mode_Cover_Close);
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("도어모드 Armed 확인");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("도어오픈");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("도어 열리는 애니메이션 실행되야 함");
            // R1Door.Animator.SetTrigger(Constants.DoorOpen);
            // await R1Door.Animator.WaitAnimationCompleteAsync(Constants.Door_Operation_Handle_Door_Open);
            
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(async _ =>
        {
            NPCListModel.Instance.NPCList.ForEach(npc =>
            {
                npc.Animator.SetBool(Constants.IsLookAround, false);
            });
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}