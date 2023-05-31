using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
public class T7_007 : Mission
{
    #region Fields
    [SerializeField] GameObject openJumpseat;
    [SerializeField] GameObject closeJumpseat;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승무원 좌석(JumpSeat)을 열어보세요.");
            openJumpseat.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            openJumpseat.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            await UniTask.Delay(2000);
            NextMission();
        }).AddTo(); 
         
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("승무원 좌석(JumpSeat)을 닫아보세요.");
            closeJumpseat.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            closeJumpseat.gameObject.SetActive(false);

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