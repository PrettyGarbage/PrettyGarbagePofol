using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class A330_999_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("A330_999 미션 시작");
        Debug.Log("체험을 종료하겠습니다. 수고하셨습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Debug.Log("A330_999 종료");
    }

    #endregion
}