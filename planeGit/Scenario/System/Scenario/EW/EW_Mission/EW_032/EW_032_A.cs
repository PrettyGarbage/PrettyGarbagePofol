using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_032_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("ELT를 챙겨주세요.");
            
            // SubtitleSystem 포인팅 될 모델로 대체되면 지울것
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            //var result = await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo();
            //SendTrainingResult(0, result);
            //ELT 표시 장치 선택하기

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        { 
            LastMissionComplete();
        }).AddTo();

    }
    #endregion
}
