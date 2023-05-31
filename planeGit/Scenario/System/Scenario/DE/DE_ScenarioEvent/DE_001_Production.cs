using System;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class DE_001_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {       
        Logger.Log("DE_001 시작");
        Logger.Log("비행기 엔진 소리");
        SoundManager.Instance.PlaySoundBGM("airplaneEngine");
        
        Logger.Log("훈련을 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("이륙 후 약 30분 후 비행고도 약 36,000피트");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("DE_001 종료");
    }

    #endregion
}