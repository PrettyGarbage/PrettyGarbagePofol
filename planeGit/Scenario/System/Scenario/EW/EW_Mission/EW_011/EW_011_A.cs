using Common;
using Library.Manager;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_011_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_011_A_1;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {

            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_EW_011_A_1.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            // 구명복 모델 등록하기
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            // Crew Seat 하단의 구명복 착용하기 애니메이션 넣기

            LastMissionComplete();
        }).AddTo(); 
        
    }

    #endregion
}