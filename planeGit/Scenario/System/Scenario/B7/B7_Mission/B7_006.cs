using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class B7_006 : Mission
{
    #region Fields

    [SerializeField] GameObject openOverhaedbin;
    [SerializeField] GameObject closeOverhaedbin;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("OverHeadbin을 열어보세요.");
            openOverhaedbin.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            openOverhaedbin.gameObject.SetActive(false);
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            await UniTask.Delay(2000);
            NextMission();
        }).AddTo(); 
         
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("OverHeadbin을 닫아보세요.");
            closeOverhaedbin.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            closeOverhaedbin.gameObject.SetActive(false);

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