using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_032_C : Mission
{
    #region Override Methods

    public override void SetMission()
    {

        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("신호장비를 챙겨주세요(Flash light)");

            // SubtitleSystem 포인팅 될 모델로 대체되면 지울것
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            /* await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo();
            
              //flash light 선택하기

             */
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(1).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
