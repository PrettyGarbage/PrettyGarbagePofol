using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class FT_001_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("FT_001 시작");

        Logger.Log("훈련을 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("이륙 후 약 5분 후 비행고도 약 3000ft 상공 입니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("FT_001 종료");
    }

    #endregion
}