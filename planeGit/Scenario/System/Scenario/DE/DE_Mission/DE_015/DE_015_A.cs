using Common;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class DE_015_A : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("손님여러분, 주목해 주십시오. 저희 비행기는 비상상황이 발생하여 약 20분 후에 비상착륙할 예정입니다. ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());
            
            Logger.Log("여러분의 안전을 위해 다시 한 번 좌석벨트를 매셨는지 확인하시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            
            Logger.Log("저희 승무원들은 이러한 상황에 대비해 충분히 훈련을 받았습니다. 침착해 주시고 객실승무원들의 지시에 따라주십시오. ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}