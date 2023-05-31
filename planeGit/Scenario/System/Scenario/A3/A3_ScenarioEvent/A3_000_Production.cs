using Cysharp.Threading.Tasks;
using UnityEngine;

public class A3_000_Production : ScenarioEventProduction
{
    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("A3_000 시작");

        Logger.Log("시작 준비가 완료되셨나요 ?");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
        
        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10).AddTo();
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("A3_000 종료");
    }

    #endregion
}