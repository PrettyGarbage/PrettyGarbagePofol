using Common;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Playables;

public class EW_019_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_019_All;

    #endregion

    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_019 시작");
        Logger.Log("충격 방지 자세를 취해주세요.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

        await director_EW_019_All.PlayAsync();

        NPCListModel.Instance.NPCList.ForEach(npc =>
        {
            npc.Animator.SetFloat(Constants.IdleState, 6);
        });

        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 5).AddTo();
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 5).AddTo();
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 5).AddTo();

        await UniTask.Delay(TimeSpan.FromMinutes(1));

        // DeltaScore = (result1 && result2 && result3) ? 0 : -1;
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_019 종료");
    }

    #endregion
}