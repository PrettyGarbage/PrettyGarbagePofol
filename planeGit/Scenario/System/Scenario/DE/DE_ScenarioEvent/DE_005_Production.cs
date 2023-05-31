using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class DE_005_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {        
        Logger.Log("DE_005 시작");

        Logger.Log("마스크를 당겨 쓰세요!!  벨트 매세요!! Put on the Mask!! Fasten your seatbelt!!");
        await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo();
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("DE_005 종료");
    }

    #endregion
}