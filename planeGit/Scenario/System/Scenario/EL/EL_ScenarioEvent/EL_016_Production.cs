using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EL_016_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EL_016 시작");
        Logger.Log("객실 및 화장실 내 잔류승객을 확인해주세요.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);
        
        Logger.Log("잔류승객 없습니다. 모두 탈출했습니다.");
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10);
        
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EL_016 종료");
    }

    #endregion
}