using Common;
using UnityEngine;
using System;
using UniRx;
using Valve.VR.InteractionSystem;

public class DE_011_A : Mission
{
    #region Fields

    [SerializeField] GameObject po2Bottle;
    //[SerializeField] Interactable po2Bottle;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("저산소증이 발생한 환자에게 Po2 bottle을 사용하여 응급조치해 주세요");
            po2Bottle.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            po2Bottle.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("응급조치 해주세요");
            // 그럼 그 보틀의 연출 효과들은 ??
            //await HandsetSystem.Instance.AttachMissionAsync(po2Bottle, Dialogues[1], 10);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe( _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}