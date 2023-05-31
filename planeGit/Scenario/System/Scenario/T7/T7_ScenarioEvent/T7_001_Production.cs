using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class T7_001_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("T7_001 시작");
        Logger.Log("B737 기종 친숙화 체험을 시작하겠습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();

    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("T7_001 종료");
    }

    #endregion
}