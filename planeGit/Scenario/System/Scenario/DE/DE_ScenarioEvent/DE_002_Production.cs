using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;

public class DE_002_Production : ScenarioEventProduction
{
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {      
        Logger.Log("DE_002 시작");

        Logger.Log("비상 강하 상황이 발생하였습니다.");
        await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10);
        
        /*
        1.객실 내 산소마크스 자동 Drop (Icon으로 뺐음 => making해서 띄울건지? position 잡아서 띄울건지?(전체 transform 인지해서 해당 위치에 Icon띄우기))
        2.객실조명이 최대로 밝아진다. (운통쪽에서 조절?, XRCamera 내에 light 조절?))
        3.급격한 감압으로 객실 내 안개현상 (마찬가지로 전체 Transform 인지 후 smog?))
        4.휭하는 큰 굉음발생 
        5.객실 내 찬바람 유입 
        6.객실온도하락, 객실 내 먼지발생, 귀가 막힘
        */

    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("DE_002 종료");
    }

    #endregion
}