using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class B7_010 : Mission
{
    #region Fields

    [SerializeField] GameObject openCompartment;
    [SerializeField] GameObject closeCompartment;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("보관함(컴파트먼트) latch를 열어보세요.");
            openCompartment.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            openCompartment.gameObject.SetActive(false);
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            await UniTask.Delay(2000);
            NextMission();
        }).AddTo(); 
         
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("보관함(컴파트먼트) latch를 닫아보세요.");
            closeCompartment.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            closeCompartment.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
         
        OnBeginMission(3).Subscribe(async _ =>
        {
            await UniTask.Delay(1000);
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}