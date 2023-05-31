using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_004_C : Mission
{
    /*#region Fields

    //4번 승객
    [SerializeField] PlayableDirector director_004_C_1;
    [SerializeField] PlayableDirector director_004_C_4;

    #endregion*/

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("4.승객 짐을 겹쳐서 선반에 올린다.");
            //await director_004_C_1.Play();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("선반에 짐을 겹쳐서 올리는 승객의 협조를 구하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("손님 짐은 떨어질 수 있어 옆 선반에 보관해드리겠습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(4, true).Subscribe(async _ =>
        {
            Logger.Log("4 승객이 짐을 옆 선반에 보관한다.");
            //await director_004_C_4.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}