using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_004_C : Mission
{
    #region Fields
    [SerializeField] GameObject HalonPBEIcon;
    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 2); 

            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("2번 승무원에게 Halon소화기와 PBE 전달하세요.");
            HalonPBEIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            HalonPBEIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}