using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_016_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_016_B_2;

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
            Logger.Log("창문 쪽 비상탈출구에 배치할 협조자를 선정하고 임무를 부여하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            Logger.Log("승무원들의 지시에 따라 창문탈출구를 개방하시고 승객들의 탈출을 도와주십시요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EW_016_B_2.PlayAsync();

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
