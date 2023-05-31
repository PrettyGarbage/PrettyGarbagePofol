using System;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class TF_001_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("TF_001 시작");
        Logger.Log("사운드 : 비행기소리");
        
        SoundManager.Instance.PlaySoundBGM("airplaneEngine");

        Logger.Log("훈련을 곧 시작합니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 5);
        
        Logger.Log("이륙 후 약 2시간 후 비행고도 약 36,000피트");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10);
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("TF_001 종료");
    }

    #endregion
}