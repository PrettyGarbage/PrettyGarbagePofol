using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class DE_000_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("DE_000 시작");

        Logger.Log("시작 준비가 완료되셨나요 ?");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 5);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("DE_000 종료");
    }

    #endregion
}