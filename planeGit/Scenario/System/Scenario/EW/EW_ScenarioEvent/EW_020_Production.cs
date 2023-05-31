using Common;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Library.Manager;
using System;
using UnityEngine;
using UnityEngine.Playables;
using static DG.Tweening.DOTween;

public class EW_020_Production : ScenarioEventProduction
{
    private Animator anim;
    [SerializeField]
    private OceanMoveUp ocean;

    [SerializeField] LightModel light;
    
    [SerializeField] PlayableDirector director_EW_020_All;

    
    #region Override Methods
    
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("EW_020 시작");
        Logger.Log("연출 : 착수굉음");        
        SoundManager.Instance.PlaySoundEffect("boom");
        light.LightFlicker(5);
        await director_EW_020_All.PlayAsync();
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("물이 차오르기 시작");
        try
        {
            ocean.OceanMove();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        Logger.Log("EW_020 종료");
    }

    #endregion
}