using System;
using Common;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_029_A : Mission
{
    #region Feilds

    [SerializeField] PlayableDirector director_29_A_1;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구 정상!! 짐 버려!! 이쪽으로!!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            // 헤엄쳐서 raft위로 올라가기까지
            await director_29_A_1.PlayAsync();
            try
            {
                NPCListModel.Instance.Gets(2, 4, 5, 9, 10, 12).ForEach
                (
                    npc => npc.Animator.SetFloat(Constants.IdleState, 52)
                );
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}