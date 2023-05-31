using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_015_A : Mission
{
    #region Fields

    public GameObject npc12safetyInformation;
    
    [SerializeField] PlayableDirector director_EW_015_A_1;

    #endregion
   
    #region Override Methods

    public override void SetMission()
    {
        var npc12 = NPCListModel.Instance.Get(12);
        
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("여러분 좌석 주머니의 안내지를 꺼내어 좌석벨트 충격방지 자세 구명복 탈출구 위치 탈출구 작동법을 재확인 하십시오.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        //12번 모델이 좌석 주머니의 안내지를 들어서 읽는 연출하기.
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("12번 모델 안내지 읽는 연출");
            
            await director_EW_015_A_1.PlayAsync();

            try
            {
                npc12.Animator.SetFloat(Constants.IdleState, 0);
                npc12safetyInformation.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();

    }

    #endregion
}
