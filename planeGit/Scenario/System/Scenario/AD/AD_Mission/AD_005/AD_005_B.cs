using UnityEngine;
using System;
using DG.Tweening;
using UniRx;
using UnityEngine.Playables;
public class AD_005_B : Mission
{
    #region Fields

    // 선반 닫히는 애니
    [SerializeField] PlayableDirector director_005_B_1;
    // 화장실 X 아이콘
    [SerializeField] PlayableDirector director_005_B_2;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("승객을 착석 시키고 모든 휴대용 수하물 정위치를 확인 후 모든 선반을 닫으세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 선받 닫히는 애니
            await director_005_B_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("화장실 승객 유무를 확인하세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("화장실 문에 X 아이콘 나타난다.");
            await director_005_B_2.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}