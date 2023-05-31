using Cysharp.Threading.Tasks;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class EW_000_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_000 시작");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]);
        
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_000 종료");
    }

    #endregion
}