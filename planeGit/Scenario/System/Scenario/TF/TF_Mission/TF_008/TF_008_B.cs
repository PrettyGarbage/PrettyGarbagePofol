using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_008_B : Mission
{
    #region Fields

    [SerializeField] GameObject toiletClosedIcon;
    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("화장실을 폐쇄하세요.");
            toiletClosedIcon.gameObject.SetActive(true);
            await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo();
            toiletClosedIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();      
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("화장실 폐쇄 되는 연출하기");
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}