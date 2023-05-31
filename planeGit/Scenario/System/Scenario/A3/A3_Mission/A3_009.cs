using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class A3_009 : Mission
{
    #region Fields
    
    [SerializeField] GameObject openCart;
    [SerializeField] GameObject closeCart;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("주방 Cart를 열어보세요.");
            openCart.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            openCart.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            await UniTask.Delay(2000);
            NextMission();
        }).AddTo(); 
         
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("주방 Cart를 닫아보세요.");
            closeCart.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            closeCart.gameObject.SetActive(false);

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