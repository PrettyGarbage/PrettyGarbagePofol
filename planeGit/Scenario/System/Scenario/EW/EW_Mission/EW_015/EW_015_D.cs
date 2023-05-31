using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_015_D : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_015_D_2;
    [SerializeField] GameObject npc8safetyInformation;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        var npc8 = NPCListModel.Instance.Get(8);

        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("승객에게 비상탈출 안내지를 확인시키세요. 승무원을 기내에 분산 배치하여 승무원은 걸어가며 안내지를 확인하십시오.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("8번 모델 안내지 읽는 연출");

            await director_EW_015_D_2.PlayAsync();
            npc8.Animator.SetFloat(Constants.IdleState, 0);
            npc8safetyInformation.SetActive(false);

            /*npc8.Animator.SetInteger(Constants.IdleState, 13);
            await npc8.Animator.WaitAnimationCompleteAsync();*/
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