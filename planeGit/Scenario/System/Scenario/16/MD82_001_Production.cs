using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


public class MD82_001_Production : ScenarioEventProduction
{
    #region Variables

    [SerializeField] float delayTime = 30f;

    #endregion
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("MD82_001 시작");

        for(int i = 0; i < Dialogues.Length; i++)
        {
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[i]);
            
            if(i > Dialogues.Length - 7)
                await UniTask.Delay(TimeSpan.FromMilliseconds(delayTime));
        }
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("MD82_001 종료");
    }

    #endregion
    
}