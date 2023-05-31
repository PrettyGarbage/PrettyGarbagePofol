using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_004_B : Mission
{
    #region Fields
    [SerializeField] GameObject toiletHandIcon;
    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {          
            Logger.Log("문을 열지 말고 열이 감지되는지 확인하십시오.");
            toiletHandIcon.gameObject.SetActive(true);
            await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo(); 
            toiletHandIcon.gameObject.SetActive(false);
            
            NextMission();
        }).AddTo();
     
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("열이 감지됩니다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]).AddTo();
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}