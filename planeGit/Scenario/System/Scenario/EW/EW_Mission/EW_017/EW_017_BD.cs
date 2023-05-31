using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_017_BD : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_017;

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
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            // 카트 모델 지정
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        //열려있는 서비스카트 닫히는 연출
        //서비스카트 갯수가 하나라면 한명이 벙찜
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_EW_017.PlayAsync();
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
