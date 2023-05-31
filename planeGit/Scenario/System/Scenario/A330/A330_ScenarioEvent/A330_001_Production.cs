using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class A330_001_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("A330_001 시작");
        
        Logger.Log("A330 기종의 탈출구 개방 미션을 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("A330_001 종료");
    }

    #endregion
}