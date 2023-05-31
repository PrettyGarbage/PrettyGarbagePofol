using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class DE_003_Production : ScenarioEventProduction
{
    #region Fields

    //[SerializeField] PlayableDirector director_DE_003;

    #endregion
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {       
        Logger.Log("DE_003 시작");

        //director_DE_003.PlayAsync();
        
        // 사운드
        Logger.Log("쾅 울리는 효과음");
        Logger.Log("큰 소음");
        Logger.Log("바람 빠지는 효과음");
        //
        
        Logger.Log("1.객실 내 산소마크스 자동 Drop");
        
        Logger.Log("2.객실조명이 최대로 밝아진다.");
        
        Logger.Log("3.급격한 감압으로 객실 내 안개현상,");
        
        Logger.Log("4.휭하는 큰 굉음발생");
        
        Logger.Log("5.객실 내 찬바람 유입");
        
        Logger.Log(" 6.객실온도하락, 객실 내 먼지발생, 귀가 막힘");
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("DE_003 종료");
    }

    #endregion
}