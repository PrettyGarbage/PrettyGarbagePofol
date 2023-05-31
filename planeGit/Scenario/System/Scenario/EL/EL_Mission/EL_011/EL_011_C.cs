using Common;
using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_011_C : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_011_C_1;
    [SerializeField] PlayableDirector director_EL_011_C_3;
    [SerializeField] PlayableDirector director_EL_011_C_5;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당겨");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당기는 애니 연출");
            Logger.Log("슬라이드 팽창되는 애니");
            await director_EL_011_C_1.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("슬라이드 팽창 확인");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("5번 9번 승객 R2 탈출구 방향으로 이동한다. ");
            
            NPCListModel.Instance.Gets(5, 9).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 1);
            });
            
            await director_EL_011_C_3.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("탈출구 정상!! 짐버려!! 이쪽으로!!Good Exit! Leave Everything! Come This Way!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(5, true).Subscribe(async _ =>
        {
            Logger.Log("일부 승객이 R2 탈출구 방향으로 이동한다. ");
            
            await director_EL_011_C_5.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(6).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}