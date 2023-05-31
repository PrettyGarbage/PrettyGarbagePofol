using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_014_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_014_B_2;

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
            Logger.Log("사운드 : 비상구 옆에서 승객에게 비상탈출구 위치를 확인시키세요. 승무원을 기내에 분산배치하여 승무원은 걸어가며 가까운 비상구를 확인시키십시오.");

            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("3번 승객이 가까운 비상구 지목하기 연출");
            
            await director_EW_014_B_2.PlayAsync();

            /*npc3.Animator.SetTrigger(Constants.PointRightFront);
            await npc3.Animator.WaitAnimationCompleteAsync(Constants.Point_Right_Front);*/

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}