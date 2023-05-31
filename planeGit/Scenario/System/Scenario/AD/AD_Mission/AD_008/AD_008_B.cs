using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_008_B : Mission
{
    #region Fields

    // [SerializeField] GameObject safetyEquipment;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            //safetyEquipment.gameObject.SetActive(true);
            
            Logger.Log("앞쪽 객실에서 안전 데모 장비를 가지고 stand by 해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            //safetyEquipment.gameObject.SetActive(false);
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}