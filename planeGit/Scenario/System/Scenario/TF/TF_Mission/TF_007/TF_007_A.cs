using Common;
using UnityEngine;
using System;
using UniRx;

public class TF_007_A : Mission
{
    #region Fields

    [SerializeField] GameObject handsetIcon;

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("신속히 기장에게 상황을 보고하세요.");
            handsetIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            handsetIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(_ =>
        {
            Logger.Log("player가 핸드셋 2번 클릭 하기");

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("기장님 화장실 화재진압 완료했습니다. 다친 승객 혹은 승무원 없습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("화장실을 점검 후 폐쇄 해주세요 ");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[2], 10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}