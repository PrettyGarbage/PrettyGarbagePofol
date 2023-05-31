using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class TF_004_D : Mission
{
    #region Fields
    [SerializeField] GameObject gloveIcon;
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
            Logger.Log("2번 승무원에게 석면장갑을 전달하세요.");
            gloveIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            gloveIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}