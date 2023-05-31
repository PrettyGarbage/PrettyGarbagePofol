using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_014_A : Mission
{
    #region Fields

    //[SerializeField] GameObject handSetIcon;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("기장님에게 상황보고를 하세요.");
            // 핸드셋 포인팅
            /*handSetIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            handSetIcon.gameObject.SetActive(false);*/
            
            //임시로 ShoutingMissionAsync으로 대체 
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());


            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("기장님, 승객들은 감압증호소 다수, 저산소증 1명, 타박상 1명 발생하여 응급 조치하였습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            Logger.Log("공항도착 후 엠블런스 필요합니다. 객실내 동체 구멍으로 승객들은 안전한 곳으로 이동하였지만 불안한 상태입니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}