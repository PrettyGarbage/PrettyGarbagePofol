using Common;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class FT_005_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_005_A;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            // 벨트 사인 사운드 정해지면 대체하기
            SoundManager.Instance.PlaySoundEffect("Airplane_Ding_Dong");

            Logger.Log("승무원 착석하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("손님 여러분 저희 비행기가 많이 흔들리고 있습니다. 좌석 벨트를 매주시고 화장실 사용은 삼가하시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 15).AddTo());;
            NextMission();
            
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("즉각적으로 가까운 좌석이나 승무원좌석에 착석하고 좌석벨트를 매세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 15).AddTo());;
            NextMission();
            
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            await director_005_A.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}