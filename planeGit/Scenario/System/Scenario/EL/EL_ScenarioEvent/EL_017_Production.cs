using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EL_017_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EL_017 시작");
        Logger.Log("잔류승객이 없음을 확인했습니다. 승무원들을 탈출하세요.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);
        
        Logger.Log("객실승무원 모두 탈출하세요!");
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10);
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EL_017 종료");
    }

    #endregion
}