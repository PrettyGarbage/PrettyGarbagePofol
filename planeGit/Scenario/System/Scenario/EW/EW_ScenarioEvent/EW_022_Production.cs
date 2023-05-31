using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UnityEngine;
using UnityEngine.Playables;

public class EW_022_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_022_All;

    #endregion
    
    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_022_Production 시작");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
        
        Logger.Log("기장 방송이 끝나면 안전벨트를 푸는 연출하기");
        await director_EW_022_All.PlayAsync();
        
    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("EW_022 종료");
    }
    #endregion
}